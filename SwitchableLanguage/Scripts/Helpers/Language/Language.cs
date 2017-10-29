/*  MIT License

    Copyright(c) 2016 Jiří Nenáhlo

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE. */

using System.Collections;
using System.Xml;
using UnityEngine;

/// <summary>
/// The Language class adds easy to use multiple language support to any Unity project by parsing an XML file
/// containing numerous strings translated into any languages of your choice.
/// 
/// Created by Adam T. Ryder
/// C# version by O. Glorieux
/// Static class rework & code cleanup by Jiří Nenáhlo
/// </summary>
public static class Language {

    /// <summary>
    /// Fallback language if the requested system language is not found.
    /// </summary>
    const SystemLanguage FallbackLanguage = SystemLanguage.English;

    const string ParamStringId = "name";

    static bool init = false;
    public static SystemLanguage currLang = FallbackLanguage;

    /// <summary>
    /// All the XML strings are in here.
    /// </summary>
    private static Hashtable fallbackStrings;
    private static Hashtable strings;

    static string GetLanguageResourcesPath(SystemLanguage language) { return "Language/" + language.ToString(); }

    /// <summary>
    /// Changes the language of strings that GetString returns.
    /// </summary>
    public static void Change(SystemLanguage language) {
        init = true;
        SetLanguage(language);
    }

    /// <summary>
    /// Used to get the language strings from a file.
    /// </summary>
    /// <param name="language">Language.</param>
    static void SetLanguage(SystemLanguage language) {
        currLang = language;
        strings = ParseStrings(language);
        if (language != FallbackLanguage) {
            fallbackStrings = ParseStrings(FallbackLanguage);
        } else {
            Debug.LogWarning("Using fallback language as main language, there will be no fallback support.");
        }
    }

    static Hashtable ParseStrings(SystemLanguage language) {
        XmlDocument xml = new XmlDocument();
        TextAsset textAsset = Resources.Load<TextAsset>(GetLanguageResourcesPath(language));
        if (textAsset == null) {
            if (language == FallbackLanguage) {
                throw new System.ArgumentNullException("Failed to load fallback translations!");
            } else {
                Debug.LogWarning("Loading language XML for '" + language.ToString() + "' failed, will be using fallback '"
                    + FallbackLanguage + "' translations.");
                return null;
            }
        }

        xml.LoadXml(textAsset.text);
        Hashtable table = new Hashtable();
        var element = xml.DocumentElement[language.ToString()];
        if (element != null) {
            var elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext()) {
                try {
                    var xmlItem = (XmlElement) elemEnum.Current;
                    table.Add(xmlItem.GetAttribute(ParamStringId), xmlItem.InnerText);
                } catch (System.InvalidCastException) {
                    // Probably a comment or whatever, ignore.
                }
            }
        } else {
            Debug.LogError("The specified language tag does not exist in the XML file for language " + language);
            return null;
        }
        return table;
    }
    
    /// <summary>
    /// Access strings in the currently selected language by supplying this function with
    /// the name identifier for the string used in the XML resource.
    /// 
    /// Example:
    /// XML file:
    /// <languages>
    ///     <English>
    ///         <string name="app_name"> Unity Multiple Language Support</string>
    ///         <string name="description"> This script provides convenient multiple language support.</string>
    ///     </English>
    ///     <French>
    ///         <string name="app_name"> Unité Langue Soutien Multiple</string>
    ///         <string name="description"> Ce script fournit un soutien multilingue pratique.</string>
    ///     </French>
    /// </languages>
    /// </summary>
    /// 
    /// <param name="name">Name of the string as defined in the language XML.</param>
    /// <returns>String in the xml file.</returns>
    public static string GetString(string name) {
        if (!init) {
            Debug.LogWarning("No language & xml file provided, string set is empty!");
            return string.Empty;
        }

        if (name == null) {
            Debug.LogWarning("GetString() with null argument called!");
            return string.Empty;
        }

        if (strings == null || !strings.ContainsKey(name)) {
            if (fallbackStrings == null || !fallbackStrings.ContainsKey(name)) {
                Debug.LogWarning("Required string '" + name + "' does not exist even in the fallback translations!");
                return name;
            } else {
                return (string) fallbackStrings[name];
            }
        }

        return (string) strings[name];
    }

}

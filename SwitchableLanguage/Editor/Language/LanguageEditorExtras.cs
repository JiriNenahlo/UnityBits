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

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LanguageEditorExtras {

    /// <summary>
    /// To prevent placeholder strings from showing accidentally in the scene,
    /// use this editor menu option to match XML strings with the text components' text values
    /// across the loaded scene in the editor.
    /// </summary>
    [MenuItem("Language/Hardcode XML strings to scene")]
    static void SyncLanguageSettersValues() {
        Language.Change(SystemLanguage.English);
        LanguageTextSetter[] setters = GameObject.FindObjectsOfType<LanguageTextSetter>();
        Debug.Log("Found " + setters.Length + " setters.");
        foreach (LanguageTextSetter setter in setters) {
            setter.Awake();
        }
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Hardcoding done.");
    }

}

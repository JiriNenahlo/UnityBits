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

using System;
using System.Collections;
using System.Text;

/// <summary>
/// Outputs more user-friendly indented ToString output for complicated objects.
/// Supports nesting custom objects thanks to the provided interface.
/// </summary>
public struct ToStringBuilder {

    /// <summary>
    /// How many spaces large is the indentation of each object level
    /// </summary>
    const int PRINT_PROPERTIES_INDENT_SIZE = 4;

    /// <summary>
    /// If the indent offset is 0, this string is not for a nested object.
    /// </summary>
    int initialIndentOffset;

    /// <summary>
    /// How many indentation steps are there in the most recent line?
    /// </summary>
    int currIndentOffset;

    StringBuilder sb;

    /// <summary>
    /// Creates the whole ToString process.
    /// </summary>
    /// <param name="type">Object type, to show the object title.</param>
    /// <param name="obj">Object implementing the interface, the object itself.</param>
    /// <param name="indentOffset">For nested child objects to keep proper indentation.
    /// Pass the indentOffset from the interface here.</param>
    /// <returns>ToStringBuilder object for adding parameters in a chain.</returns>
    public static ToStringBuilder Create(Type type, IToStringBuildable obj, int indentOffset = 0) {
        ToStringBuilder tsb = new ToStringBuilder();
        tsb.initialIndentOffset = indentOffset;
        tsb.currIndentOffset = indentOffset;
        tsb.sb = new StringBuilder();
        tsb.sb.Append(type);
        tsb.AppendSeparator();
        tsb.currIndentOffset++;
        return tsb;
    }

    /// <summary>
    /// Adds a new variable to the object's string representation.
    /// </summary>
    /// <param name="variableName">How the variable is called</param>
    /// <param name="value">The variable itself</param>
    /// <returns>ToStringBuilder object for adding parameters in a chain.</returns>
    public ToStringBuilder Add(string variableName, object value) {
        AppendNewLine();
        AppendObject(variableName, true);
        AppendSeparator();
        AppendObject(value, true);
        return this;
    }

    /// <summary>
    /// Appends a value to the StringBuilder.
    /// Can be variable name (Dictionaries) or values themselves.
    /// </summary>
    /// <param name="value">What to append</param>
    /// <param name="skipFormatting">Strings get special quote treatment so in case they are empty,
    /// the string is still shown as "". Object titles are not supposed to be quoted, so pass true here when passing object title.</param>
    void AppendObject(object value, bool skipFormatting = false) {
        if (value == null) {
            sb.Append("null");
        }

        if (value is IList) {
            currIndentOffset++;
            IList list = (IList) value;
            foreach (object listValue in list) {
                AppendNewLine();
                AppendObject(listValue);
            }
            currIndentOffset--;
        } else if (value is IDictionary) {
            currIndentOffset++;
            IDictionary dictionary = (IDictionary) value;
            IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext()) {
                AppendNewLine();
                AppendObject(enumerator.Key);
                AppendSeparator();
                AppendObject(enumerator.Value);
            }
            currIndentOffset--;
        } else if (value is IToStringBuildable) {
            sb.Append(((IToStringBuildable) value).ToIndentedString(currIndentOffset));
        } else {
            if (skipFormatting) {
                // For variable names, don't modify the variable in any way ("", etc.)
                sb.Append(value);
                return;
            }

            if (value is string) {
                sb.Append("\"");
                sb.Append(value);
                sb.Append("\"");
            } else {
                sb.Append(value);
            }
        }
    }

    void AppendSeparator() {
        sb.Append(": ");
    }

    void AppendNewLine() {
        sb.Append("\n");
        sb.Append(new string(' ', currIndentOffset * PRINT_PROPERTIES_INDENT_SIZE));
    }

    /// <summary>
    /// Completes the string building process.
    /// </summary>
    /// <returns>Object's string representation for debugging purposes.</returns>
    public string End() {
        if (initialIndentOffset == 0) {
            // Don't create an empty line between nested objects in the final string,
            // show newline separator only when the parent builder finishes.
            sb.Append("\n");
        }
        return sb.ToString();
    }

}

/// <summary>
/// Every object wanting to use this builder must implement this interface.
/// </summary>
public interface IToStringBuildable {
    string ToIndentedString(int indentOffset);
}

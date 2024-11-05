using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    public class JavaScriptFormatter
    {
        public static string FormatJavaScriptCode(string code)
        {
            // Remove leading and trailing whitespace
            code = code.Trim();

            // Remove multi-line comments
            code = Regex.Replace(code, @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/", "");

            // Add indentation
            int indentationLevel = 0;
            string[] lines = code.Split('\n');
            string formattedCode = "";
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("}"))
                {
                    indentationLevel--;
                }
                formattedCode += new string('\t', indentationLevel) + trimmedLine + "\n";
                if (trimmedLine.EndsWith("{"))
                {
                    indentationLevel++;
                }
            }

            // Remove extra spaces around commas
            formattedCode = Regex.Replace(formattedCode, @"\s*,\s*", ", ");

            return formattedCode;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    /// <summary>
    /// 字符串转换成C#标准格式
    /// </summary>
    public class ConvertToCSharpFormat
    {
        public static string FormatCSharpCode(string code)
        {
            int indentationLevel = 0;
            string[] lines = code.Split('\n');
            string formattedCode = "";

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.EndsWith("{"))
                {
                    formattedCode += GetIndentation(indentationLevel) + trimmedLine + "\n";
                    indentationLevel++;
                }
                else if (trimmedLine.StartsWith("}"))
                {
                    indentationLevel = Math.Max(indentationLevel - 1, 0);
                    formattedCode += GetIndentation(indentationLevel) + trimmedLine + "\n";
                }
                else
                {
                    formattedCode += GetIndentation(indentationLevel) + trimmedLine + "\n";
                }
            }

            return formattedCode;
        }

        public static string GetIndentation(int level)
        {
            return new string('\t', level);
        }
    }
}
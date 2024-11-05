using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    public class ConvertToCamelCase
    {
        /// <summary>
        /// 首字母小写驼峰命名法
        /// </summary>
        /// <param name="csharpCode"></param>
        /// <returns></returns>
        public static string ConvertToLowerCamelCase(string csharpCode)
        {
            // Match class names and properties
            Regex classNameRegex = new Regex(@"\bclass\s+(\w+)", RegexOptions.Multiline);
            Regex propertyNameRegex = new Regex(@"\bpublic\s+\w+\s+(\w+)\s+{\s+get;\s+set;\s+}", RegexOptions.Multiline);

            // Convert class names to upper case and properties to camel case
            csharpCode = classNameRegex.Replace(csharpCode, match =>
            {
                string className = match.Groups[1].Value;
                return char.ToUpper(className[0]) + className.Substring(1);
            });
            csharpCode = propertyNameRegex.Replace(csharpCode, match =>
            {
                string propertyName = match.Groups[1].Value;
                return $"public string {Char.ToLower(propertyName[0])}{propertyName.Substring(1)} {{ get; set; }}";
            });

            return csharpCode;
        }

        /// <summary>
        /// 首字母大写驼峰命名法
        /// </summary>
        /// <param name="csharpCode"></param>
        /// <returns></returns>
        public static string ConvertToUpperCamelCase(string csharpCode)
        {
            // 将类名的首字母大写
            csharpCode = Regex.Replace(csharpCode, @"\bclass\s+(\w+)", match =>
            {
                string className = match.Groups[1].Value;
                return "class " + char.ToUpper(className[0]) + className.Substring(1);
            });

            // 将属性名的首字母大写，将下划线后的字符改为大写（不删除下划线）
            csharpCode = Regex.Replace(csharpCode, @"public\s+(\w+)\s+(\w+)\s+{\s*get;\s*set;\s*}", match =>
            {
                string propertyName = match.Groups[2].Value;
                propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                propertyName = Regex.Replace(propertyName, @"_(\w)", m => "_" + m.Groups[1].Value.ToUpper());
                return "public " + match.Groups[1].Value + " " + propertyName + " { get; set; }";
            });

            return csharpCode;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    
    public class StringHelper
    {
        /// <summary>
        /// 比较两个字符串是否相等
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool CompareStrings(string str1, string str2)
        {
            string pattern = @"\s+";
            Regex regex = new Regex(pattern);

            string cleanStr1 = regex.Replace(str1, "");
            string cleanStr2 = regex.Replace(str2, "");

            return string.Equals(cleanStr1, cleanStr2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 删除代码注释
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string RemoveComments(string code)
        {
            // 使用正则表达式删除 JavaScript 代码中的注释
            // 匹配 /* ... */ 样式的多行注释
            string pattern = @"/\*(.*?)\*/";
            string uncommentedCode = Regex.Replace(code, pattern, "", RegexOptions.Singleline);

            // 匹配 // 样式的单行注释
            pattern = @"//.*?$";
            uncommentedCode = Regex.Replace(uncommentedCode, pattern, "", RegexOptions.Multiline);

            return uncommentedCode;
        }
    }
}

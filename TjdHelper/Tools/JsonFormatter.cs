using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    public class JsonFormatter
    {
        public static string FormatJson(string jsonString)
        {
            try
            {
                // 将 JSON 字符串解析为 JToken 对象
                JToken token = JToken.Parse(jsonString);

                // 序列化为格式化的字符串并返回
                return token.ToString(Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                // JSON 格式错误，直接返回原始字符串
                return jsonString;
            }
        }
    }
}

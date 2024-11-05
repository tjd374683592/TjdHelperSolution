using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TjdHelper.Tools
{
    /// <summary>
    /// Json字符串转换C#类
    /// </summary>
    public class ConvertToCSharpClassHelper
    {
        public static string GetCSharpStrByJson(string jsonStr)
        {
            var jObject = JObject.Parse(jsonStr);

            Dictionary<string, string> classDicts = new Dictionary<string, string>();
            classDicts.Add("Root", GetClassDefinion(jObject));
            foreach (var item in jObject.Properties())
            {
                string key = item.Name;
                JToken value = item.Value;
                if (value.Type == JTokenType.Object)
                {

                    classDicts.Add(item.Name, GetClassDefinion(item.Value));
                    GetClasses(item.Value, classDicts);
                }
            }
            StringBuilder sb = new StringBuilder(1024);
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("namespace MyNameSpace");
            sb.AppendLine("{");
            foreach (var item in classDicts)
            {
                sb.Append($"public class {item.Key}" + Environment.NewLine);
                sb.Append("{" + Environment.NewLine);
                sb.Append(item.Value);
                sb.Append("}" + Environment.NewLine);
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static void GetClasses(JToken jToken, Dictionary<string, string> classes)
        {
            if (jToken is JValue)
            {
                return;
            }
            var childToken = jToken.First;
            while (childToken != null)
            {
                if (childToken.Type == JTokenType.Property)
                {
                    var p = (JProperty)childToken;
                    var valueType = p.Value.Type;

                    if (valueType == JTokenType.Object)
                    {
                        if (!classes.ContainsKey(p.Name))
                        {
                            classes.Add(p.Name, GetClassDefinion(p.Value));
                        }
                        
                        GetClasses(p.Value, classes);
                    }
                    else if (valueType == JTokenType.Array)
                    {
                        foreach (var item in (JArray)p.Value)
                        {
                            if (item.Type == JTokenType.Object)
                            {
                                if (!classes.ContainsKey(p.Name))
                                {
                                    classes.Add(p.Name, GetClassDefinion(item));
                                }

                                GetClasses(item, classes);
                            }
                        }
                    }
                }

                childToken = childToken.Next;
            }
        }

        public static string GetClassDefinion(JToken jToken)
        {
            StringBuilder sb = new(256);
            var subValueToken = jToken.First();
            while (subValueToken != null)
            {
                if (subValueToken.Type == JTokenType.Property)
                {
                    var p = (JProperty)subValueToken;
                    var valueType = p.Value.Type;
                    if (valueType == JTokenType.Object)
                    {
                        sb.Append("public " + p.Name + " " + p.Name + " {get;set;}" + Environment.NewLine);
                    }
                    else if (valueType == JTokenType.Array)
                    {
                        var arr = (JArray)p.Value;
                        //a.First

                        switch (arr.First().Type)
                        {
                            case JTokenType.Object:
                                sb.Append($"public List<{p.Name}> " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            case JTokenType.Integer:
                                sb.Append($"public List<int> " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            case JTokenType.Float:
                                sb.Append($"public List<float> " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            case JTokenType.String:
                                sb.Append($"public List<string> " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            case JTokenType.Boolean:
                                sb.Append($"public List<bool> " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (valueType)
                        {
                            case JTokenType.Integer:
                                sb.Append($"public int " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            case JTokenType.Float:
                                sb.Append($"public float " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            case JTokenType.String:
                                sb.Append($"public string " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            case JTokenType.Boolean:
                                sb.Append($"public bool " + p.Name + " {get;set;}" + Environment.NewLine);
                                break;
                            default:
                                break;
                        }
                    }
                }

                subValueToken = subValueToken.Next;
            }

            return sb.ToString();
        }
    }
}
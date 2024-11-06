using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TjdHelper.Tools;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TjdHelper.ViewModels
{
    public class YamlControlViewModel : INotifyPropertyChanged
    {

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// YAML字符串
        /// </summary>
        private string _strYaml;

        public string StrYaml
        {
            get { return _strYaml; }
            set
            {
                if (_strYaml != value)
                {
                    _strYaml = value;
                    OnPropertyChanged(nameof(StrYaml));
                }
            }
        }

        /// <summary>
        /// Json字符串
        /// </summary>
        private string _strJson;

        public string StrJson
        {
            get { return _strJson; }
            set
            {
                if (_strJson != value)
                {
                    _strJson = value;
                    OnPropertyChanged(nameof(StrJson));
                }
            }
        }

        /// <summary>
        /// Json转YAML字符串
        /// </summary>
        private string _strYamlConverted;

        public string StrYamlConverted
        {
            get { return _strYamlConverted; }
            set
            {
                if (_strYamlConverted != value)
                {
                    _strYamlConverted = value;
                    OnPropertyChanged(nameof(StrYamlConverted));
                }
            }
        }

        public ICommand FormatYamlCommand { get; set; }
        public ICommand Json2YAMLCommand { get; set; }

        public YamlControlViewModel()
        {
            FormatYamlCommand = new RelayCommand(FormatYamlCommandExecute);
            Json2YAMLCommand = new RelayCommand(Json2YAMLCommandExecute);
        }

        /// <summary>
        /// 检测/格式化YAML
        /// </summary>
        /// <param name="obj"></param>
        private void FormatYamlCommandExecute(object obj)
        {
            if (!string.IsNullOrEmpty(StrYaml))
            {
                try
                {
                    // 自动修正缩进和格式问题
                    StrYaml = FormatYaml(StrYaml);
                }
                catch (Exception ex)
                {
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "格式化失败，errorMsg:" + ex.Message;
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                }
            }
            else
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "YAML字符串为空";
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
            }
        }

        /// <summary>
        /// Json转YAML
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Json2YAMLCommandExecute(object obj)
        {
            if (!string.IsNullOrEmpty(StrJson))
            {
                try
                {
                    StrYamlConverted = ConvertJsonToYaml(StrJson);



                }
                catch (Exception ex)
                {
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "转换失败，errorMsg:" + ex.Message;
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                }
            }
            else
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "Json字符串为空";
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
            }
        }

        // 递归地将 JToken 转换为 YamlNode
        private static YamlNode ConvertJTokenToYamlNode(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var mappingNode = new YamlMappingNode();
                    foreach (var prop in token.Children<JProperty>())
                    {
                        mappingNode.Add(new YamlScalarNode(prop.Name), ConvertJTokenToYamlNode(prop.Value));
                    }
                    return mappingNode;

                case JTokenType.Array:
                    var sequenceNode = new YamlSequenceNode();
                    foreach (var item in token.Children())
                    {
                        sequenceNode.Add(ConvertJTokenToYamlNode(item));
                    }
                    return sequenceNode;

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                    return new YamlScalarNode(token.ToString());

                default:
                    return new YamlScalarNode(token.ToString());
            }
        }

        public static string ConvertJsonToYaml(string json)
        {
            // 使用 JObject 解析 JSON 字符串
            var jsonObject = JsonConvert.DeserializeObject<JToken>(json);

            // 将 JSON 转换为 YAML 节点
            var yamlRootNode = ConvertJTokenToYamlNode(jsonObject);

            // 使用 YamlStream 进行序列化
            var yamlStream = new YamlStream(new YamlDocument(yamlRootNode));
            using (var writer = new System.IO.StringWriter())
            {
                yamlStream.Save(writer);
                return writer.ToString();
            }
        }

        public static string FormatYaml(string yaml)
        {
            // 反序列化 YAML 字符串为动态对象
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) // 使用驼峰命名
                .Build();

            var yamlObject = deserializer.Deserialize<dynamic>(yaml);

            // 重新序列化为格式化的 YAML 字符串
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) // 使用驼峰命名
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();

            return serializer.Serialize(yamlObject);
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TjdHelper.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace TjdHelper.ViewModels
{
    public class JsonHelperUserControlViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        /// <summary>
        /// Json节点
        /// </summary>
        private BindingList<JsonNode> _nodes;
        public BindingList<JsonNode> Nodes
        {
            get { return _nodes; }
            set
            {
                if (_nodes != value)
                {
                    _nodes = value;
                    OnPropertyChanged(nameof(Nodes));
                }
            }
        }

        /// <summary>
        /// StrJson 转义等操作
        /// </summary>
        private string _trJsonPrase;

        public string StrJsonPrase
        {
            get { return _trJsonPrase; }
            set
            {
                if (_trJsonPrase != value)
                {
                    _trJsonPrase = value;
                    OnPropertyChanged(nameof(StrJsonPrase));
                }
            }
        }

        public ICommand CheckAndFormatJsonCommand { get; set; }
        public ICommand CompresseJsonCommand { get; set; }
        public ICommand SerializeJsonCommand { get; set; }
        public ICommand DeserializeJsonCommand { get; set; }
        public ICommand ClearJsonCommand { get; set; }
        public ICommand ShowJsonTreeCommand { get; set; }


        public JsonHelperUserControlViewModel()
        {
            CheckAndFormatJsonCommand = new RelayCommand(CheckAndFormatJsonCommandExecute);
            CompresseJsonCommand = new RelayCommand(CompresseJsonCommandExecute);
            SerializeJsonCommand = new RelayCommand(SerializeJsonCommandExecute);
            DeserializeJsonCommand = new RelayCommand(DeserializeJsonCommandExecute);
            ClearJsonCommand = new RelayCommand(ClearJsonCommandExecute);
            ShowJsonTreeCommand = new RelayCommand(ShowJsonTreeCommandExecute);
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="obj"></param>
        private void CheckAndFormatJsonCommandExecute(object obj)
        {
            if (!string.IsNullOrEmpty(StrJsonPrase))
            {
                try
                {
                    // 解析 JSON 字符串为 JToken 对象
                    JToken token = JToken.Parse(StrJsonPrase);
                    // 使用 Formatting.Indented 参数来实现缩进格式化
                    StrJsonPrase = token.ToString(Formatting.Indented);

                    //不是json
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "Json格式校验成功";
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                }
                catch (Exception ex)
                {
                    //不是json
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "不是Json格式，errorMsg:" + ex.Message;
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                }
            }
        }

        /// <summary>
        /// 压缩Json
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CompresseJsonCommandExecute(object obj)
        {
            if (!string.IsNullOrEmpty(StrJsonPrase))
            {
                try
                {
                    // 解析 JSON 字符串为 JToken 对象
                    JToken token = JToken.Parse(StrJsonPrase);
                    // 使用 Formatting.Indented 参数来实现缩进格式化
                    StrJsonPrase = token.ToString(Formatting.None);
                }
                catch (Exception ex)
                {
                    //不是json
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "errorMsg:" + ex.Message;
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                }
            }
        }

        /// <summary>
        /// 转义Json
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SerializeJsonCommandExecute(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(StrJsonPrase))
                {
                    StrJsonPrase = JsonConvert.SerializeObject(StrJsonPrase);
                }
            }
            catch (Exception ex)
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "转义失败，errorMsg:" + ex.Message;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
            }
        }

        /// <summary>
        /// 去转义Json
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DeserializeJsonCommandExecute(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(StrJsonPrase))
                {
                    if (!StrJsonPrase.StartsWith("\""))
                    {
                        StrJsonPrase = "\"" + StrJsonPrase;
                    }
                    if (!StrJsonPrase.EndsWith("\""))
                    {
                        StrJsonPrase = StrJsonPrase + "\"";
                    }
                    // 反序列化为对象
                    string jsonObj = JsonConvert.DeserializeObject(StrJsonPrase).ToString();
                    // 解析 JSON 字符串为 JToken 对象
                    JToken token = JToken.Parse(jsonObj);
                    // 使用 Formatting.Indented 参数来实现缩进格式化
                    StrJsonPrase = token.ToString(Formatting.None);
                }
            }
            catch (Exception ex)
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "去转义失败，errorMsg:" + ex.Message;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
            }
        }

        /// <summary>
        /// 清空json
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ClearJsonCommandExecute(object obj)
        {
            StrJsonPrase = "";
        }


        /// <summary>
        /// 展示Json树
        /// </summary>
        /// <param name="obj"></param>
        private void ShowJsonTreeCommandExecute(object obj)
        {
            if (!string.IsNullOrEmpty(StrJsonPrase))
            {
                //折叠Json原文
                CompresseJsonCommandExecute(obj);

                //装在tree node
                Nodes = new BindingList<JsonNode>();
                LoadJson(StrJsonPrase);
            }
        }


        public void LoadJson(string jsonString)
        {
            try
            {
                Nodes.Clear();
                var rootNode = CreateNode("Root", JToken.Parse(jsonString));
                Nodes.Add(rootNode);
            }
            catch (Exception ex)
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "展示Json树失败，errorMsg:" + ex.Message;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
            }

        }

        private JsonNode CreateNode(string name, JToken token)
        {
            var node = new JsonNode();
            node.Name = name;

            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (var property in token.Children<JProperty>())
                    {
                        var childNode = CreateNode(property.Name, property.Value);
                        node.Children.Add(childNode);
                    }
                    break;
                case JTokenType.Array:
                    int index = 0;
                    foreach (var arrayItem in token)
                    {
                        var childNode = CreateNode(index.ToString(), arrayItem);
                        node.Children.Add(childNode);
                        index++;
                    }
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                    node.Value = token.ToString();
                    break;
            }

            return node;
        }

    }

    public class JsonNode
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<JsonNode> Children { get; set; }

        public JsonNode()
        {
            Children = new List<JsonNode>();
        }
    }

}

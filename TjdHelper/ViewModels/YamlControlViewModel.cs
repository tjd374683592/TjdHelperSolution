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

        public ICommand FormatYamlCommand { get; set; }

        public YamlControlViewModel()
        {
            FormatYamlCommand = new RelayCommand(FormatYamlCommandExecute);
        }

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
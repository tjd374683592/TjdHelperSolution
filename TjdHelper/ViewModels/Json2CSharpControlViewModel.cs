using TjdHelper.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TjdHelper.ViewModels
{
    public class Json2CSharpControlViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public Json2CSharpControlViewModel()
        {
            JsonToCSharpCommand = new RelayCommand(JsonToCSharpCommandExecute);
            CompileCSharpCommand = new RelayCommand(CompileCSharpCommandExecute);
        }

        public ICommand JsonToCSharpCommand { get; private set; }
        public ICommand CompileCSharpCommand { get; set; }

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
        /// c#字符串
        /// </summary>
        private string _strCSharp;

        public string StrCSharp
        {
            get { return _strCSharp; }
            set
            {
                if (_strCSharp != value)
                {
                    _strCSharp = value;
                    OnPropertyChanged(nameof(StrCSharp));
                }
            }
        }

        /// <summary>
        /// Json转C#代码
        /// </summary>
        /// <param name="obj"></param>
        private void JsonToCSharpCommandExecute(object obj)
        {
            //json转c#类
            string cSharpStr = ConvertToCSharpClassHelper.GetCSharpStrByJson(StrJson);
            //代码格式化
            StrCSharp = ConvertToCSharpFormat.FormatCSharpCode(cSharpStr);
            //代码驼峰命名法
            StrCSharp = ConvertToCamelCase.ConvertToUpperCamelCase(StrCSharp);

            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "生成完毕";
        }

        /// <summary>
        /// 编译C#代码
        /// </summary>
        /// <param name="obj"></param>
        private void CompileCSharpCommandExecute(object obj)
        {
            CompileCSharpCodeHelper helper = new CompileCSharpCodeHelper();
            var compileResult = helper.ConpileCSharpCode(StrCSharp);
            var result = "编辑结果：" + (compileResult == true ? "编译成功" : "编译失败");

            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = result + helper.FailedReason;
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
        }
    }
}

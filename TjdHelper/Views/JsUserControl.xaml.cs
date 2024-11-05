using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using Jint;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Text.RegularExpressions;
using TjdHelper.Tools;
using MahApps.Metro.Controls;
using TjdHelper.ViewModels;
using System.Text.Json.Nodes;
using Jint.Native;

namespace TjdHelper.Views
{
    /// <summary>
    /// JsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class JsUserControl : UserControl
    {
        private string originJavsScriptSrt = "function main(arg1,arg2) {\r\n  //先点击“解析参数”创建参数输入textbox，main函数中有几个参数，解析参数汇编加对应个数的textbox作为参数输入\r\n  //在此处写入JavaScript代码，函数名main不可修改\r\n  \r\n  \r\n  \r\n  return arg1;\r\n}";
        public JsUserControl()
        {
            InitializeComponent();

            // 从 XML 文件加载语法定义
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("TjdHelper.Resources.json.xshd"))
            {
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            // 将语法定义分配给 TextEditor 控件
            txtExecuteResult.SyntaxHighlighting = customHighlighting;

            // 初始 JavaScript 函数字符串
            txtJavascriptInput.Text = originJavsScriptSrt;

        }

        /// <summary>
        /// 执行JavaScript代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtExecuteResult.Text = "";

                string inputCode = StringHelper.RemoveComments(txtJavascriptInput.Text);
                string oriCode = StringHelper.RemoveComments(originJavsScriptSrt);
                if (StringHelper.CompareStrings(inputCode, oriCode))
                {
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "JavaScript代码没写";
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                    return;
                }


                // 创建 JavaScript 引擎实例
                var engine = new Engine();

                // 执行前端传入的 JavaScript 函数字符串
                engine.Execute(txtJavascriptInput.Text);

                //调用前端传入的 JavaScript 函数
                List<string> paraList = new List<string>();

                foreach (var item in textBoxContainer.Children)
                {
                    if (item is TextBox textBox)
                    {
                        // 获取每个 TextBox 的 Text 值
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            //参数忘写
                            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "有参数没写";
                            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                            return;
                        }
                        paraList.Add(textBox.Text);
                    }
                }

                //执行JS函数
                var result = engine.Invoke("main", paraList.ToArray());

                //结果是对象类型
                if (result.IsObject() && !result.IsArray())
                {
                    //循环打印所有object类型数据
                    PrintObjectDetails(result);
                }

                if (result.IsArray())
                {
                    var array = result.AsArray();

                    if (array.First().IsObject())
                    {
                        txtExecuteResult.AppendText("[");
                        foreach (var item in array)
                        {
                            var obj = item.AsObject();
                            var properties = obj.GetOwnProperties();
                            // 创建一个包含多个键值对的 JObject
                            JObject jsonObject = new JObject();

                            // 遍历属性并输出值
                            foreach (var property in properties)
                            {
                                jsonObject[property.Key.ToString()] = property.Value.Value.ToString();
                            }

                            // 将 JObject 转换为 JSON 字符串
                            txtExecuteResult.AppendText(jsonObject.ToString() + ",");
                        }
                        var endIndex = txtExecuteResult.Text.LastIndexOf(',');
                        txtExecuteResult.Text = txtExecuteResult.Text.Substring(0, endIndex)+"]";
                    }
                    else
                    {
                        //循环打印所有object类型数据
                        PrintObjectDetails(result);
                    }
                }

                if (result.IsString() || result.IsNumber() || result.IsUndefined() || result.IsDate())
                {
                    txtExecuteResult.Text = result.ToString();
                }

                if (!string.IsNullOrEmpty(txtExecuteResult.Text))
                {
                    txtExecuteResult.Text = JsonFormatter.FormatJson(this.txtExecuteResult.Text);
                }
            }
            catch (Exception ex)
            {
                txtExecuteResult.Text = ex.Message;
            }
        }

        /// <summary>
        /// 循环打印所有object类型数据
        /// </summary>
        /// <param name="result"></param>
        private void PrintObjectDetails(JsValue result)
        {
            // 获取对象的所有自有属性
            var obj = result.AsObject();
            var properties = obj.GetOwnProperties();

            // 创建一个包含多个键值对的 JObject
            JObject jsonObject = new JObject();

            // 遍历属性并输出值
            foreach (var property in properties)
            {
                jsonObject[property.Key.ToString()] = property.Value.Value.ToString();
            }

            // 将 JObject 转换为 JSON 字符串
            txtExecuteResult.Text = jsonObject.ToString();
        }

        /// <summary>
        /// 计算JavaScript代码参数个数
        /// </summary>
        /// <param name="jsCode"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        private int CountFunctionParams(string jsCode, string functionName)
        {
            string pattern = @"function\s+" + Regex.Escape(functionName) + @"\s*\(([^)]*)\)";
            Match match = Regex.Match(jsCode, pattern);
            if (match.Success)
            {
                string paramsPart = match.Groups[1].Value;
                string[] paramsArray = paramsPart.Split(',');
                return paramsArray.Length;
            }
            return 0;
        }

        /// <summary>
        /// 格式化JavaScript代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            txtJavascriptInput.Text = JavaScriptFormatter.FormatJavaScriptCode(this.txtJavascriptInput.Text);
        }

        private void BtnCalcPara_Click(object sender, RoutedEventArgs e)
        {
            //计算参数个数
            string strScript = txtJavascriptInput.Text;
            int paraCount = CountFunctionParams(strScript, "main");
            if (paraCount > 0)
            {
                txtContentHeader.Visibility = Visibility.Visible;
            }
            else
            {
                txtContentHeader.Visibility = Visibility.Collapsed;
            }

            //创建textbox
            textBoxContainer.Children.Clear();
            for (int i = 0; i < paraCount; i++)
            {
                var newTextBox = new TextBox
                {
                    Margin = new Thickness(5),
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    ToolTip = "参数" + i,
                    TextWrapping = TextWrapping.Wrap,
                    AcceptsReturn = true,
                    MaxWidth = txtJavascriptInput.ActualWidth
                };

                // 使用附加属性设置水印对齐方式为左侧
                TextBoxHelper.SetWatermarkAlignment(newTextBox, TextAlignment.Left);
                // 设置水印
                TextBoxHelper.SetWatermark(newTextBox, "参数" + i + "：");
                // 设置使用浮动水印
                TextBoxHelper.SetUseFloatingWatermark(newTextBox, true);

                textBoxContainer.Children.Add(newTextBox);
            }
        }
    }
}

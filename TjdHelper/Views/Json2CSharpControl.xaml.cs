using ControlzEx.Standard;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using TjdHelper.Tools;
using TjdHelper.ViewModels;
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

namespace TjdHelper.Views
{
    /// <summary>
    /// Json2CSharpControl.xaml 的交互逻辑
    /// </summary>
    public partial class Json2CSharpControl : UserControl
    {
        public Json2CSharpControlViewModel JsonToCSharpCommandVM { get; set; }

        public Json2CSharpControl()
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
            txtJson.SyntaxHighlighting = customHighlighting;

            JsonToCSharpCommandVM = new Json2CSharpControlViewModel();
            DataContext = JsonToCSharpCommandVM;
        }
    }
}

using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
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
using TjdHelper.ViewModels;

namespace TjdHelper.Views
{
    /// <summary>
    /// YamlControl.xaml 的交互逻辑
    /// </summary>
    public partial class YamlControl : UserControl
    {
        public YamlControl()
        {
            InitializeComponent();

            // 从 XML 文件加载语法定义
            IHighlightingDefinition customHighlightingYaml;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("TjdHelper.Resources.yaml.xshd"))
            {
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlightingYaml = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            // 将语法定义分配给 TextEditor 控件
            txtYaml.SyntaxHighlighting = customHighlightingYaml;
            txtYAMLConverted.SyntaxHighlighting = customHighlightingYaml;

            IHighlightingDefinition customHighlightingJson;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("TjdHelper.Resources.json.xshd"))
            {
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlightingJson = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            // 将语法定义分配给 TextEditor 控件
            txtJson.SyntaxHighlighting = customHighlightingJson;
            

            YamlControlViewModel viewModel = new YamlControlViewModel();
            this.DataContext = viewModel;
        }
    }
}

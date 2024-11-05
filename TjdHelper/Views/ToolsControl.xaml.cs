using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
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
    /// TestControl.xaml 的交互逻辑
    /// </summary>
    public partial class ToolsControl : UserControl
    {
        public ToolsControlViewModel ToolsHelperControlVM { get; set; }

        public ToolsControl()
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


            ToolsHelperControlVM = new ToolsControlViewModel();
            ToolsHelperControlVM.TxtTimeInfoWaterMark = "格式：" + DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss");
            ToolsHelperControlVM.SecondsIsChecked = true;
            ToolsHelperControlVM.MillisecondsIsChecked = false;
            ToolsHelperControlVM.TimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ToolsHelperControlVM.QRImageSize = 500;

            DataContext = ToolsHelperControlVM;
        }
    }
}

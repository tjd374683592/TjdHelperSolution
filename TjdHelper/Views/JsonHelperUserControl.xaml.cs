using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using Newtonsoft.Json.Linq;
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
using System.Security.Policy;

namespace TjdHelper.Views
{
    /// <summary>
    /// JsonHelperUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class JsonHelperUserControl : UserControl
    {
        public JsonHelperUserControl()
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
            txtToConvertJson.SyntaxHighlighting = customHighlighting;

            JsonHelperUserControlViewModel jsonHelperUserControlViewModel = new JsonHelperUserControlViewModel();
            this.DataContext = jsonHelperUserControlViewModel;
        }

        void JsonHelperUserControl_ContentRendered(object sender, EventArgs e)
        {
            this.treeView.ExpandAll();
        }

        private void TreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取右键单击的 TreeViewItem
            TreeViewItem treeViewItem = GetTreeViewItemClicked(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                // 显示右键菜单
                ContextMenu contextMenu = new ContextMenu();
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "复制Key";
                menuItem.Click += CopyKey_Click;
                contextMenu.Items.Add(menuItem);
                menuItem = new MenuItem();
                menuItem.Header = "复制Value";
                menuItem.Click += CopyValue_Click;
                contextMenu.Items.Add(menuItem);
                treeViewItem.ContextMenu = contextMenu;
            }
        }

        private TreeViewItem GetTreeViewItemClicked(DependencyObject obj)
        {
            while (obj != null && obj != treeView)
            {
                if (obj is TreeViewItem)
                {
                    return (TreeViewItem)obj;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

        private void CopyKey_Click(object sender, RoutedEventArgs e)
        {
            // 将文本复制到剪贴板
            string strKey = ((JsonNode)treeView.SelectedItem).Name;
            if (!string.IsNullOrEmpty(strKey))
            {
                Clipboard.SetText(strKey);
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "复制成功Key：" + strKey;
            }
        }

        private void CopyValue_Click(object sender, RoutedEventArgs e)
        {
            // 将文本复制到剪贴板
            string strValue = ((JsonNode)treeView.SelectedItem).Value;
            if (!string.IsNullOrEmpty(strValue))
            {
                Clipboard.SetText(strValue);
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "复制成功Value：" + strValue;
            }
        }
    }

    /// <summary>
    /// treeview扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        public static void ExpandAll(this System.Windows.Controls.TreeView treeView)
        {
            ExpandAllItems(treeView);
        }

        private static void ExpandAllItems(ItemsControl control)
        {
            if (control == null)
            {
                return;
            }

            foreach (Object item in control.Items)
            {

                System.Windows.Controls.TreeViewItem treeItem = control.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (treeItem == null || !treeItem.HasItems)
                {
                    continue;
                }

                treeItem.IsExpanded = true;
                ExpandAllItems(treeItem as ItemsControl);
            }
        }
    }
}

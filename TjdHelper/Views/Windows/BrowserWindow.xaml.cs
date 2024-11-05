using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TjdHelper.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Diagnostics;
using System.Security.Policy;

namespace TjdHelper.Views.Windows
{
    /// <summary>
    /// BrowserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserWindow : MetroWindow
    {
        public string Url { get; set; }
        public BrowserWindowViewModel ViewModel { get; set; }
        public BrowserWindow(string url, string title)
        {
            InitializeComponent();
            InitializeWebView(url);
            this.Title = title;
            Url = url;
            ViewModel = new BrowserWindowViewModel();
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// 初始化edge webview2
        /// </summary>
        private async void InitializeWebView(string url)
        {
            await MyWebView.EnsureCoreWebView2Async();
            MyWebView.Source = new Uri(url);
        }

        /// <summary>
        /// 复制Url
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // 将文本复制到剪贴板
            Clipboard.SetText(Url);

            //成功提示
            var settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "ok",
                ColorScheme = this.MetroDialogOptions!.ColorScheme
            };

            MessageDialogResult result = this.ShowModalMessageExternal("提示:", "网页链接已复制到粘贴板\r\n\r\nUrl:" + Url, MessageDialogStyle.Affirmative, settings);
        }

        /// <summary>
        /// 默认浏览器打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloud_Click_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Url,
                UseShellExecute = true // 这行确保使用默认浏览器打开链接
            });
        }
    }
}

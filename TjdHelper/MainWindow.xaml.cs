using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using TjdHelper.Tools;
using System.Reflection;
using System.Windows;
using System.IO;
using System.Xml;
using Microsoft.CodeAnalysis;
using MahApps.Metro.Controls;
using TjdHelper.ViewModels;
using TjdHelper.Views.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TjdHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindowViewModel MainWindowVM { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            var t = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, this.Tick, this.Dispatcher);


            MainWindowVM = new MainWindowViewModel();
            MainWindowVM.LogInfo = "Status：系统启动";

            // 指定默认头像         
            if (MainWindowVM.UserImage == null)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Resources/images/userimage.gif"));
                MainWindowVM.UserImage = bitmapImage;
            }

            DataContext = MainWindowVM;
        }

        /// <summary>
        /// 时钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tick(object? sender, EventArgs e)
        {
            if (this.IsVisible == false)
            {
                return;
            }

            var dateTime = DateTime.Now;
            this.customTransitioning.Content = new TextBlock { Text = dateTime.ToString("yyyy-MM-dd HH:mm:ss"), SnapsToDevicePixels = true };
        }

        //Ansible社区文档
        private void btnAnsibleDocPage_Click(object sender, RoutedEventArgs e)
        {
            BrowserWindow browserWindow = new BrowserWindow("https://docs.ansible.org.cn/ansible/latest/getting_started/index.html", "Ansible社区文档");
            browserWindow.Show();
        }

        //Ansible笔记
        private void btnAnsibleNotePage_Click(object sender, RoutedEventArgs e)
        {
            BrowserWindow browserWindow = new BrowserWindow("https://note.youdao.com/s/NG3MoQwP", "Ansible笔记");
            browserWindow.Show();
        }

        //ChatGPT
        private void btnChatGPT_Click(object sender, RoutedEventArgs e)
        {
            BrowserWindow browserWindow = new BrowserWindow("https://chatgpt.com/", "ChatGPT");
            browserWindow.Show();
        }

        //豆包
        private void btnDouBao_Click(object sender, RoutedEventArgs e)
        {
            BrowserWindow browserWindow = new BrowserWindow("https://www.doubao.com/chat/", "豆包");
            browserWindow.Show();
        }
    }
}
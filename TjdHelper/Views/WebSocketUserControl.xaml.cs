using TjdHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace TjdHelper.Views
{
    /// <summary>
    /// WebSocketUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class WebSocketUserControl : UserControl
    {
        public WebSocketUserControl()
        {
            InitializeComponent();

            WebSocketUserControlViewModel viewModel = new WebSocketUserControlViewModel();
            viewModel.WebSocketSeverStateIsOn = false;
            this.DataContext = viewModel;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TjdHelper.ViewModels
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 日志信息
        /// </summary>
        private string _logInfo;

        public string LogInfo
        {
            get { return _logInfo; }
            set
            {
                if (_logInfo != value)
                {
                    _logInfo = value;
                    OnPropertyChanged(nameof(LogInfo));
                }
            }
        }

        /// <summary>
        /// 弹窗显示状态
        /// </summary>
        private bool _showFlyOut;

        public bool ShowFlyOut
        {
            get { return _showFlyOut; }
            set
            {
                if (_showFlyOut != value)
                {
                    _showFlyOut = value;
                    OnPropertyChanged(nameof(ShowFlyOut));
                }
            }
        }

        /// <summary>
        /// 用户头像
        /// </summary>
        private BitmapImage _userImage;

        public BitmapImage UserImage
        {
            get { return _userImage; }
            set
            {
                if (_userImage != value)
                {
                    _userImage = value;
                    OnPropertyChanged(nameof(UserImage));
                }
            }
        }
    }
}

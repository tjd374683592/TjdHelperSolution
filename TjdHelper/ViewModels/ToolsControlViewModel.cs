using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TjdHelper.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text.Json.Nodes;
using System.Drawing;
using ZXing.Common;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Microsoft.Win32;

namespace TjdHelper.ViewModels
{
    public class ToolsControlViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ToolsControlViewModel()
        {
            ConvertToTimeCommand = new RelayCommand(ConvertToTimeCommandExecute);
            ConvertToTimestampCommand = new RelayCommand(ConvertToTimestampCommandExecute);
            UrlChangedCommand = new RelayCommand(UrlChangedCommandExecute);
            UrlEncordChangedCommand = new RelayCommand(UrlEncordChangedCommandExecute);
            CreateQRCodeCommand = new RelayCommand(CreateQRCodeCommandExecute);
            ClearQRCodeCommand = new RelayCommand(ClearQRCodeCommandExecute);
            DownloadQRCodeCommand = new RelayCommand(DownloadQRCodeCommandExecute);
            DecodeQRCodeCommand = new RelayCommand(DecodeQRCodeCommandExecute);
            ChooseQRCodePathCommand = new RelayCommand(ChooseQRCodePathCommandExecute);
        }

        public ICommand ConvertToTimeCommand { get; private set; }
        public ICommand ConvertToTimestampCommand { get; private set; }
        public ICommand UrlChangedCommand { get; private set; }
        public ICommand UrlEncordChangedCommand { get; private set; }
        public ICommand EncryptDataDecodeCommand { get; private set; }
        public ICommand ConvertToJsonFormatCommand { get; private set; }
        public ICommand CreateQRCodeCommand { get; set; }
        public ICommand ClearQRCodeCommand { get; set; }
        public ICommand DownloadQRCodeCommand { get; set; }
        public ICommand DecodeQRCodeCommand { get; set; }
        public ICommand ChooseQRCodePathCommand { get; set; }

        /// <summary>
        /// 绑定时间转换TimeHelper User Control中的txtTime控件WaterMark属性值
        /// </summary>
        private string _txtTimeInfoWaterMark;

        public string TxtTimeInfoWaterMark
        {
            get { return _txtTimeInfoWaterMark; }
            set
            {
                if (_txtTimeInfoWaterMark != value)
                {
                    _txtTimeInfoWaterMark = value;
                    OnPropertyChanged(nameof(TxtTimeInfoWaterMark));
                }
            }
        }

        /// <summary>
        /// Radio Button秒选中状态
        /// </summary>
        private bool _secondsIsChecked;

        public bool SecondsIsChecked
        {
            get { return _secondsIsChecked; }
            set
            {
                if (_secondsIsChecked != value)
                {
                    _secondsIsChecked = value;
                    OnPropertyChanged(nameof(SecondsIsChecked));
                }
            }
        }

        /// <summary>
        /// Radio Button毫秒选中状态
        /// </summary>
        private bool _millisecondsIsChecked;

        public bool MillisecondsIsChecked
        {
            get { return _millisecondsIsChecked; }
            set
            {
                if (_millisecondsIsChecked != value)
                {
                    _millisecondsIsChecked = value;
                    OnPropertyChanged(nameof(MillisecondsIsChecked));
                }
            }
        }

        /// <summary>
        /// 时间戳的值
        /// </summary>
        private string _timestampStr;

        public string TimestampStr
        {
            get { return _timestampStr; }
            set
            {
                if (_timestampStr != value)
                {
                    _timestampStr = value;
                    OnPropertyChanged(nameof(TimestampStr));
                }
            }
        }

        /// <summary>
        /// 时间的值
        /// </summary>
        private string _timeStr;

        public string TimeStr
        {
            get { return _timeStr; }
            set
            {
                if (_timeStr != value)
                {
                    _timeStr = value;
                    OnPropertyChanged(nameof(TimeStr));
                }
            }
        }

        /// <summary>
        /// 转换结果
        /// </summary>
        private string _timeConvertResult;

        public string TimeConvertResult
        {
            get { return _timeConvertResult; }
            set
            {
                if (_timeConvertResult != value)
                {
                    _timeConvertResult = value;
                    OnPropertyChanged(nameof(TimeConvertResult));
                }
            }
        }

        /// <summary>
        /// Url
        /// </summary>
        private string _strUrl;

        public string StrUrl
        {
            get { return _strUrl; }
            set
            {
                if (_strUrl != value)
                {
                    _strUrl = value;
                    OnPropertyChanged(nameof(StrUrl));
                    UrlChangedCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Url Encord Str
        /// </summary>
        private string _strUrlEncode;

        public string StrUrlEncode
        {
            get { return _strUrlEncode; }
            set
            {
                if (_strUrlEncode != value)
                {
                    _strUrlEncode = value;
                    OnPropertyChanged(nameof(StrUrlEncode));
                    UrlEncordChangedCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Encrypt Key
        /// </summary>
        private string _encryptKey;

        public string EncryptKey
        {
            get { return _encryptKey; }
            set
            {
                if (_encryptKey != value)
                {
                    _encryptKey = value;
                    OnPropertyChanged(nameof(EncryptKey));
                    EncryptDataDecodeCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Encrypt Data
        /// </summary>
        private string _encryptData;

        public string EncryptData
        {
            get { return _encryptData; }
            set
            {
                if (_encryptData != value)
                {
                    _encryptData = value;
                    OnPropertyChanged(nameof(EncryptData));
                    EncryptDataDecodeCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Encrypt Data
        /// </summary>
        private string _encryptDataDecode;

        public string EncryptDataDecode
        {
            get { return _encryptDataDecode; }
            set
            {
                if (_encryptDataDecode != value)
                {
                    _encryptDataDecode = value;
                    OnPropertyChanged(nameof(EncryptDataDecode));
                    ConvertToJsonFormatCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// 二维码字符串
        /// </summary>
        private string _qRCodeStr;

        public string QRCodeStr
        {
            get { return _qRCodeStr; }
            set
            {
                if (_qRCodeStr != value)
                {
                    _qRCodeStr = value;
                    OnPropertyChanged(nameof(QRCodeStr));
                }
            }
        }

        /// <summary>
        /// 二维码图片
        /// </summary>
        private BitmapImage _qRImage;

        public BitmapImage QRImage
        {
            get { return _qRImage; }
            set
            {
                if (_qRImage != value)
                {
                    _qRImage = value;
                    OnPropertyChanged(nameof(QRImage));
                }
            }
        }

        /// <summary>
        /// 二维码图片边长
        /// </summary>
        private int _qRImageSize;

        public int QRImageSize
        {
            get { return _qRImageSize; }
            set
            {
                if (_qRImageSize != value)
                {
                    _qRImageSize = value;
                    OnPropertyChanged(nameof(QRImageSize));
                }
            }
        }

        /// <summary>
        /// 二维码解码内容
        /// </summary>
        private string _qRImageContent;

        public string QRImageContent
        {
            get { return _qRImageContent; }
            set
            {
                if (_qRImageContent != value)
                {
                    _qRImageContent = value;
                    OnPropertyChanged(nameof(QRImageContent));
                }
            }
        }

        /// <summary>
        /// 二维码解码路径
        /// </summary>
        private string _decodeQRImagePath;

        public string DecodeQRImagePath
        {
            get { return _decodeQRImagePath; }
            set
            {
                if (_decodeQRImagePath != value)
                {
                    _decodeQRImagePath = value;
                    OnPropertyChanged(nameof(DecodeQRImagePath));
                }
            }
        }

        /// <summary>
        /// 时间戳转换时间
        /// </summary>
        /// <param name="parameter"></param>
        private void ConvertToTimeCommandExecute(object parameter)
        {
            if (string.IsNullOrEmpty(TimestampStr))
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "时间戳为空";
                return;
            }

            if (TimestampStr.Length > 10)
            {
                //毫秒级
                MillisecondsIsChecked = true;
                SecondsIsChecked = false;
            }
            else
            {
                MillisecondsIsChecked = false;
                SecondsIsChecked = true;
            }
            string strTimeResult = TimeHelper.ConvertToTimeByTimestamp(long.Parse(TimestampStr), MillisecondsIsChecked);
            TimeSpan span = TimeSpan.FromMinutes(5);
            //5分钟前的时间戳
            TimeResultObj timeBefore = TimeHelper.GetTimeBefore(TimestampStr, span, MillisecondsIsChecked);

            //5分钟后的时间戳
            TimeResultObj timeAftere = TimeHelper.GetTimeAfter(TimestampStr, span, MillisecondsIsChecked);

            TimeConvertResult = GetFinalTimeResultStr(strTimeResult, timeBefore, timeAftere);

            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "时间戳 -> 时间";
        }

        /// <summary>
        /// 时间转换时间戳
        /// </summary>
        /// <param name="parameter"></param>
        private void ConvertToTimestampCommandExecute(object parameter)
        {
            if (string.IsNullOrEmpty(TimeStr))
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "时间为空";
                return;
            }

            long timestampNow = TimeHelper.ConvertToTimestampByTime(TimeStr, MillisecondsIsChecked);
            TimeSpan span = TimeSpan.FromMinutes(5);
            //5分钟前的时间戳
            TimeResultObj timeBefore = TimeHelper.GetTimeBefore(timestampNow.ToString(), span, MillisecondsIsChecked);

            //5分钟后的时间戳
            TimeResultObj timeAftere = TimeHelper.GetTimeAfter(timestampNow.ToString(), span, MillisecondsIsChecked);

            TimeConvertResult = GetFinalTimeResultStr(timestampNow.ToString(), timeBefore, timeAftere);

            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "时间 -> 时间戳";
        }

        /// <summary>
        /// 拼接时间转换结果字符串
        /// </summary>
        /// <param name="strTimeResult"></param>
        /// <param name="timeBefore"></param>
        /// <param name="timeAftere"></param>
        /// <returns></returns>
        private static string GetFinalTimeResultStr(string strTimeResult, TimeResultObj timeBefore, TimeResultObj timeAftere)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("转换结果：");
            sb.AppendLine(strTimeResult);
            sb.AppendLine();
            sb.AppendLine("5分钟前：");
            sb.AppendLine("Timestamp：" + timeBefore.TimestampBefore);
            sb.AppendLine("Time：" + timeBefore.TimeBefore);
            sb.AppendLine();
            sb.AppendLine("5分钟后：");
            sb.AppendLine("Timestamp：" + timeAftere.TimestampAfter);
            sb.AppendLine("Time：" + timeAftere.TimeAfter);
            return sb.ToString();
        }

        /// <summary>
        /// Url Encode
        /// </summary>
        /// <param name="parameter"></param>
        private void UrlChangedCommandExecute(object parameter)
        {
            //编码
            StrUrlEncode = HttpUtility.UrlEncode(StrUrl);
        }

        /// <summary>
        /// Url Decode
        /// </summary>
        /// <param name="parameter"></param>
        private void UrlEncordChangedCommandExecute(object parameter)
        {
            //解码
            StrUrl = HttpUtility.UrlDecode(StrUrlEncode);
        }

        public Bitmap QRCodeToSave { get; set; }
        /// <summary>
        /// 生成QR Code
        /// </summary>
        /// <param name="obj"></param>
        private void CreateQRCodeCommandExecute(object obj)
        {
            if (!string.IsNullOrEmpty(QRCodeStr))
            {
                BarcodeWriter<Bitmap> writer = new BarcodeWriter<Bitmap>
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions { Height = QRImageSize, Width = QRImageSize },
                    Renderer = new BitmapRenderer()
                };
                var bitmap = writer.Write(QRCodeStr);

                //保存需要下载的bitmap变量
                QRCodeToSave = bitmap;

                // 将 Bitmap 转换为 BitmapImage
                BitmapImage bitmapImage = ConvertBitmapToBitmapImage(bitmap);
                QRImage = bitmapImage;
            }
            else
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "二维码字符串为空";
            }
        }

        /// <summary>
        /// 下载QR Code
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DownloadQRCodeCommandExecute(object obj)
        {
            // 构建文件夹的完整路径
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"QRImage");
            // 判断文件夹是否存在
            if (!Directory.Exists(folderPath))
            {
                // 如果不存在，则创建文件夹
                Directory.CreateDirectory(folderPath);
            }
            //构建文件名
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid() + ".png";
            // 构建完整的文件路径
            string filePath = Path.Combine(folderPath, fileName);

            if (QRCodeToSave != null)
            {
                // 保存二维码图像
                QRCodeToSave.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            Process.Start("explorer.exe", folderPath);
        }

        /// <summary>
        /// Bitmap转换BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                // 将 Bitmap 保存到内存流中
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                // 创建 BitmapImage 并加载内存流中的图像
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        /// <summary>
        /// 清空二维码字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ClearQRCodeCommandExecute(object obj)
        {
            this.QRCodeStr = "";
            this.QRImage = null;
        }

        /// <summary>
        /// 二维码解码
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DecodeQRCodeCommandExecute(object obj)
        {
            try
            {
                // 加载二维码图片
                Bitmap bitmap = new Bitmap(DecodeQRImagePath);

                // 创建ZXing解码器
                BarcodeReader reader = new BarcodeReader();

                // 解码二维码
                var result = reader.Decode(bitmap);

                // 检查解码结果
                if (result != null)
                {
                    QRImageContent = result.Text;
                }
                else
                {
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "无法解码二维码，请确认图片是否正确";
                }
            }
            catch (Exception ex)
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = ex.Message;
            }
        }

        /// <summary>
        /// 选择二维码路径
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ChooseQRCodePathCommandExecute(object obj)
        {
            // 创建文件选择对话框
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "请选择文件",       // 对话框标题
                Filter = "所有文件|*.*",  // 文件过滤器
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) // 初始目录
            };

            // 显示对话框并判断用户是否选择了文件
            if (openFileDialog.ShowDialog() == true)
            {
                // 获取文件路径
                DecodeQRImagePath = openFileDialog.FileName;
            }
        }
    }
}

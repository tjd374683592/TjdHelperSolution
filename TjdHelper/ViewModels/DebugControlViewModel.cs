using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TjdHelper.Models;
using TjdHelper.Tools;

namespace TjdHelper.ViewModels
{
    public class DebugControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand CheckWinErrorDetailsCommand { get; set; }
        public ICommand WinErrorAndDetailsClearCommand { get; set; }
        public ICommand CalcVSizeSizeCommand { get; set; }
        public ICommand CalcResultClearCommand { get; set; }

        /// <summary>
        /// WinError Code字符串
        /// </summary>
        private string _strWinErrorCode;

        public string StrWinErrorCode
        {
            get { return _strWinErrorCode; }
            set
            {
                if (_strWinErrorCode != value)
                {
                    _strWinErrorCode = value;
                    OnPropertyChanged(nameof(StrWinErrorCode));
                }
            }
        }

        /// <summary>
        /// WinError Details字符串
        /// </summary>
        private string _strWinErrorCodeDetails;

        public string StrWinErrorCodeDetails
        {
            get { return _strWinErrorCodeDetails; }
            set
            {
                if (_strWinErrorCodeDetails != value)
                {
                    _strWinErrorCodeDetails = value;
                    OnPropertyChanged(nameof(StrWinErrorCodeDetails));
                }
            }
        }

        /// <summary>
        /// 起始地址
        /// </summary>
        private string _strStartAddress;

        public string StrStartAddress
        {
            get { return _strStartAddress; }
            set
            {
                if (_strStartAddress != value)
                {
                    _strStartAddress = value;
                    OnPropertyChanged(nameof(StrStartAddress));
                }
            }
        }

        /// <summary>
        /// 结束地址
        /// </summary>
        private string _strEndAddress;

        public string StrEndAddress
        {
            get { return _strEndAddress; }
            set
            {
                if (_strEndAddress != value)
                {
                    _strEndAddress = value;
                    OnPropertyChanged(nameof(StrEndAddress));
                }
            }
        }

        /// <summary>
        /// 虚拟空间大小
        /// </summary>
        private string _strVSize;

        public string StrVSize
        {
            get { return _strVSize; }
            set
            {
                if (_strVSize != value)
                {
                    _strVSize = value;
                    OnPropertyChanged(nameof(StrVSize));
                }
            }
        }

        public DebugControlViewModel()
        {
            CheckWinErrorDetailsCommand = new RelayCommand(CheckWinErrorDetailsCommandExecute);
            WinErrorAndDetailsClearCommand = new RelayCommand(WinErrorAndDetailsClearCommandExecute);
            CalcVSizeSizeCommand = new RelayCommand(CalcVSizeSizeCommandExecute);
            CalcResultClearCommand = new RelayCommand(CalcResultClearCommandExecute);
        }

        /// <summary>
        /// 查询Win Error Code详情
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CheckWinErrorDetailsCommandExecute(object obj)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string exePath = Path.Combine(currentPath, @"Resources\Tools\Err_6.4.5.exe");
            string errorCodesJsonFilePath = Path.Combine(currentPath, @"Resources\Tools\errorCodes.json");
            string arguments = StrWinErrorCode;  // 传递的参数

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true, // 重定向标准输出
                    RedirectStandardError = true,  // 重定向错误输出
                    UseShellExecute = false, // 不使用shell执行
                    CreateNoWindow = true   // 不创建窗口
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    // 读取标准输出
                    string output = process.StandardOutput.ReadToEnd();
                    // 读取错误输出
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();  // 等待进程结束
                    int exitCode = process.ExitCode;  // 获取返回的退出代码


                    StringBuilder sb = new StringBuilder();
                    // 读取 JSON 文件 -> 输出中文error info
                    string jsonContent = File.ReadAllText(errorCodesJsonFilePath);
                    // 解析 JSON 数据
                    List<WinErrorInfo> errorList = JsonConvert.DeserializeObject<List<WinErrorInfo>>(jsonContent);
                    // 查找对应的错误信息
                    WinErrorInfo errorJsonInfo = new WinErrorInfo();
                    //null的时候赋值为0
                    StrWinErrorCode = string.IsNullOrEmpty(StrWinErrorCode) ? "0" : StrWinErrorCode.Trim();

                    //判断用户输入的是10进制还是16进制
                    if (DecimalHelper.IsDecimal(StrWinErrorCode))
                    {
                        errorJsonInfo = errorList.FirstOrDefault(e => e.ErrorCode == Convert.ToInt32(StrWinErrorCode));
                    }
                    else if (DecimalHelper.IsHexadecimal(StrWinErrorCode))
                    {
                        errorJsonInfo = errorList.FirstOrDefault(e => Convert.ToInt32(e.HexCode, 16) == Convert.ToInt32(StrWinErrorCode, 16));
                    }


                    if (errorJsonInfo != null)
                    {
                        sb.AppendLine("Error Code:" + errorJsonInfo.ErrorCode);
                        sb.AppendLine("Error HexCode:" + errorJsonInfo.HexCode);
                        sb.AppendLine("Error ErrorName:" + errorJsonInfo.ErrorName);
                        sb.AppendLine("Error Description:" + errorJsonInfo.Description + "\r\n\r\n");
                    }

                    //拼装ErrTool输出的内容
                    sb.AppendLine("Output:");
                    sb.AppendLine(output);
                    sb.AppendLine("Error:");
                    sb.AppendLine(error);

                    StrWinErrorCodeDetails = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "Exception: " + ex.Message;
            }
        }

        /// <summary>
        /// 清空WinError Code和WinError Details
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void WinErrorAndDetailsClearCommandExecute(object obj)
        {
            StrWinErrorCode = "";
            StrWinErrorCodeDetails = "";
        }

        /// <summary>
        /// 根据地址计算文件大小
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CalcVSizeSizeCommandExecute(object obj)
        {
            try
            {
                // 清理输入：移除反引号、0x前缀和空白字符
                string startAddr = StrStartAddress.Replace("`", "").Replace("0x", "").Replace(" ", "").Trim().ToUpper();
                string endAddr = StrEndAddress.Replace("`", "").Replace("0x", "").Replace(" ", "").Trim().ToUpper();

                // 转换为数字
                IntPtr start = new IntPtr(Convert.ToInt64(startAddr, 16));
                IntPtr end = new IntPtr(Convert.ToInt64(endAddr, 16));

                //计算大小
                long vsize = end.ToInt64() - start.ToInt64();

                if (vsize <= 0)
                {
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = $"结束地址：{StrEndAddress} 必须大于起始地址：{StrStartAddress}";
                }

                StrVSize = "Size:  " + vsize + " byte    " + vsize / 1024 + "KB    " + vsize / 1024 / 1024 + "MB";
            }
            catch (Exception ex)
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "Exception: " + ex.Message;
            }
        }

        /// <summary>
        /// 清空计算结果
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CalcResultClearCommandExecute(object obj)
        {
            StrStartAddress = "";
            StrEndAddress = "";
            StrVSize = "";
        }
    }
}

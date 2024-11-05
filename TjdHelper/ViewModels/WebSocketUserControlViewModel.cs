using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TjdHelper.Tools;
using System.Windows;
using Fleck;
using ControlzEx.Standard;

namespace TjdHelper.ViewModels
{
    public class WebSocketUserControlViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        /// <summary>
        /// websocket url
        /// </summary>
        private string _webSocketUrl;

        public string WebSocketUrl
        {
            get { return _webSocketUrl; }
            set
            {
                if (_webSocketUrl != value)
                {
                    _webSocketUrl = value;
                    OnPropertyChanged(nameof(WebSocketUrl));
                }
            }
        }

        /// <summary>
        /// ip地址集合
        /// </summary>
        private BindingList<string> _ipAddrList;
        public BindingList<string> IpAddrList
        {
            get { return _ipAddrList; }
            set
            {
                if (_ipAddrList != value)
                {
                    _ipAddrList = value;
                    OnPropertyChanged(nameof(IpAddrList));
                }
            }
        }

        /// <summary>
        /// server开启/关闭状态
        /// </summary>
        private bool _webSocketSeverStateIsOn;
        public bool WebSocketSeverStateIsOn
        {
            get { return _webSocketSeverStateIsOn; }
            set
            {
                if (_webSocketSeverStateIsOn != value)
                {
                    _webSocketSeverStateIsOn = value;
                    OnPropertyChanged(nameof(WebSocketSeverStateIsOn));
                }
            }
        }

        /// <summary>
        /// websocket server log
        /// </summary>
        private string _serverLog;

        public string ServerLog
        {
            get { return _serverLog; }
            set
            {
                if (_serverLog != value)
                {
                    _serverLog = value;
                    OnPropertyChanged(nameof(ServerLog));
                }
            }
        }

        /// <summary>
        /// websocket server log
        /// </summary>
        private string _msgToSend;

        public string MsgToSend
        {
            get { return _msgToSend; }
            set
            {
                if (_msgToSend != value)
                {
                    _msgToSend = value;
                    OnPropertyChanged(nameof(MsgToSend));
                }
            }
        }

        public WebSocketUserControlViewModel()
        {
            //填充本机IP地址
            IpAddrList = new BindingList<string>();

            // 获取本地计算机上的所有网络接口
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // 过滤掉非活动的接口和回环接口
                if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    // 获取网络接口上的IP属性
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    // 获取该接口上的所有IP地址
                    foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                    {
                        // 过滤掉IPv6地址和非本地链接地址
                        if (ipAddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(ipAddressInfo.Address))
                        {
                            IpAddrList.Add("ws://" + ipAddressInfo.Address.ToString() + ":7080");
                        }
                    }
                }
            }

            if (IpAddrList.Count > 0)
            {
                WebSocketUrl = IpAddrList.First();
            }
            ServerStartStopCommand = new RelayCommand(ServerStartStopCommandExecute);
            SendMessageCommand = new RelayCommand(SendMessageCommandExecute);
            ClearLogCommand = new RelayCommand(ClearLogCommandExecute);
        }

        public ICommand ServerStartStopCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand ClearLogCommand { get; set; }

        /// <summary>
        /// 启动关闭web socket server
        /// </summary>
        WebSocketServer server = null;
        List<IWebSocketConnection> allSockets = null;
        private void ServerStartStopCommandExecute(object obj)
        {
            if (WebSocketSeverStateIsOn == true)
            {
                ServerLog += DateTime.Now + "   WebSocket Server 启动\r\n";

                FleckLog.Level = LogLevel.Debug;
                allSockets = new List<IWebSocketConnection>();
                server = new WebSocketServer(WebSocketUrl);
                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        ServerLog += DateTime.Now + "   客户端连接成功!\r\n";
                        allSockets.Add(socket);
                        ServerLog += DateTime.Now + "   当前客户端数量：" + allSockets.ToList().Count + "\r\n";
                    };
                    socket.OnClose = () =>
                    {
                        ServerLog += DateTime.Now + "   客户端已经关闭!\r\n";
                        allSockets.Remove(socket);
                        ServerLog += DateTime.Now + "   当前客户端数量：" + allSockets.ToList().Count + "\r\n";
                    };
                    //收到消息时
                    socket.OnMessage = message =>
                    {
                        ServerLog += DateTime.Now + "   " + message + "\r\n";
                        allSockets.ToList().ForEach(s => s.Send(message));
                    };
                });
            }
            else
            {
                // 关闭所有连接
                foreach (var socket in allSockets)
                {
                    socket.Close();
                }
                // 停止服务器
                server.Dispose();

                ServerLog += DateTime.Now + "   WebSocket Server 关闭\r\n";
            }
        }

        /// <summary>
        /// 发送web socket消息
        /// </summary>
        /// <param name="obj"></param>
        private void SendMessageCommandExecute(object obj)
        {

            if (WebSocketSeverStateIsOn == false)
            {
                //error提示
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "请检查web socket server系统开启状态";
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                return;
            }
            if (string.IsNullOrEmpty(MsgToSend))
            {
                //error提示
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).LogInfo = "请填写需要发送的消息内容";
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).ShowFlyOut = true;
                return;
            }

            if (allSockets.ToList().Count <= 0)
            {
                ServerLog += DateTime.Now + "   发送失败：客户端链接数量为0";
            }
            //遍历所有的socket客户端，给每个客户端发送消息
            foreach (var socket in allSockets.ToList())
            {
                socket.Send(MsgToSend);
                ServerLog += DateTime.Now + "   向客户端：" + socket.ConnectionInfo.ClientIpAddress + "   发送了消息：" + MsgToSend + "\r\n";
            }
        }

        /// <summary>
        /// 清空server log
        /// </summary>
        /// <param name="obj"></param>
        private void ClearLogCommandExecute(object obj)
        {
            ServerLog = "";
        }
    }
}

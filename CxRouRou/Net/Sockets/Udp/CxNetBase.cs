using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Util;

namespace CxSolution.CxRouRou.Net.Sockets.Udp
{
    /// <summary>
    /// 网络基类(Udp)
    /// </summary>
    public abstract class CxNetBase
    {
        /// <summary>
        /// 主Socket对象(IPv4)
        /// </summary>
        private Socket _socketIPv4;
        /// <summary>
        /// 主Socket对象(IPv6)
        /// </summary>
        private Socket _socketIPv6;
        /// <summary>
        /// 会话(IPv4)
        /// </summary>
        private CxSession _sessionIPv4;
        /// <summary>
        /// 会话(IPv6)
        /// </summary>
        private CxSession _sessionIPv6;
        /// <summary>
        /// Socket异步操作参数(IPv4)
        /// </summary>
        private SocketAsyncEventArgs _socketAsyncEventArgsIPv4;
        /// <summary>
        /// Socket异步操作参数(IPv4)
        /// </summary>
        private SocketAsyncEventArgs _socketAsyncEventArgsIPv6;
        /// <summary>
        /// 网络配置
        /// </summary>
        protected readonly CxNetConfig _netConfig;
        /// <summary>
        ///  是否初始化标记
        /// </summary>
        private int _isIntFlag;
        /// <summary>
        /// 初始化
        /// </summary>
        public CxNetBase()
        {
            _netConfig = new CxNetConfig() { ReceiveBufferSize = 8196, ReceiveTimeout = 5000, SendBufferSize = 1400, SendTimeout = 5000, ListenIPv6 = false };
        }
        /// <summary>
        /// 设置网络配置
        /// </summary>
        /// <param name="receiveBufferSize"></param>
        /// <param name="receiveTimeout"></param>
        /// <param name="sendBufferSize"></param>
        /// <param name="sendTimeout"></param>
        /// <param name="listenIPv6"></param>
        public virtual void SetNetConfig(int receiveBufferSize = 8196, int receiveTimeout = 5000, int sendBufferSize = 1400, int sendTimeout = 5000, bool listenIPv6 = false)
        {
            if (_isIntFlag != 0)
            {
                throw new MethodAccessException("设置网络配置无效,应在启动服务或连接服务器之前调用");
            }
            _netConfig.ReceiveBufferSize = receiveBufferSize;
            _netConfig.ReceiveTimeout = receiveTimeout;
            _netConfig.SendBufferSize = sendBufferSize;
            _netConfig.SendTimeout = sendTimeout;
            _netConfig.ListenIPv6 = listenIPv6;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected void Init()
        {
            if (Interlocked.Exchange(ref _isIntFlag, 1) == 1) return;

            _sessionIPv4 = new CxSession(_netConfig.ReceiveBufferSize);
            _socketAsyncEventArgsIPv4 = new SocketAsyncEventArgs();
            _socketAsyncEventArgsIPv4.Completed += ReceiveCallBackIPv4;
            _socketAsyncEventArgsIPv4.SetBuffer(_sessionIPv4.Buffer, 0, _sessionIPv4.Buffer.Length);

            if (_netConfig.ListenIPv6)
            {
                _sessionIPv6 = new CxSession(_netConfig.ReceiveBufferSize);
                _socketAsyncEventArgsIPv6 = new SocketAsyncEventArgs();
                _socketAsyncEventArgsIPv6.Completed += ReceiveCallBackIPv6;
                _socketAsyncEventArgsIPv6.SetBuffer(_sessionIPv6.Buffer, 0, _sessionIPv6.Buffer.Length);
            }
        }
        #region Listen
        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="port"></param>
        public void StartListen(ushort port)
        {
            Init();
            StartListenIPv4(new IPEndPoint(IPAddress.Any, 0), port);
            if (_netConfig.ListenIPv6)
            {
                StartListenIPv6(new IPEndPoint(IPAddress.Any, 0), port);
            }
        }
        /// <summary>
        /// 开始监听(指定host的消息)
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="comparer"></param>
        public void StartListenFrom(string host, ushort port, IComparer<IPAddress> comparer = null)
        {
            Init();
            var ipAddressArray = CxNetTool.GetIPAddress(host, port, comparer);
            if (ipAddressArray != null && ipAddressArray.Length > 0)
            {
                var listenIPv4 = false;
                var listenIPv6 = false;
                //遍历监听解析出的地址数组
                foreach (var ipAddress in ipAddressArray)
                {
                    var ipEndPoint = new IPEndPoint(ipAddress, port);
                    if (!listenIPv4 && ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        StartListenIPv4(ipEndPoint, port);
                        listenIPv4 = true;
                    }
                    else if (!listenIPv6 && ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        StartListenIPv6(ipEndPoint, port);
                        listenIPv6 = true;
                    }
                }
                if (!listenIPv4 && !listenIPv6)
                {
                    OnStartListenFail(port, CxString.Format("无法进行监听的地址 host:{0}", host));
                }
            }
            else
            {
                OnStartListenFail(port, CxString.Format("无法解析的地址 host:{0}", host));
            }
        }
        /// <summary>
        /// 开始监听(IPv4)
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="port"></param>
        private void StartListenIPv4(EndPoint remoteAddress, ushort port)
        {
            if (_socketIPv4 != null)
            {
                OnStartListenFail(port, "已经开始监听(IPv4)");
                return;
            }
            _socketIPv4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socketIPv4.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//重用地址
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            try
            {
                _socketIPv4.Bind(localEndPoint);
                _socketIPv4.SendBufferSize = _netConfig.SendBufferSize;

                _sessionIPv4.RemoteAddress = remoteAddress;

                StartSessionIPv4();

                OnStartListenSuccess(port, "监听成功(IPv4)");
            }
            catch (Exception e)
            {
                OnStartListenFail(port, e.Message + "(IPv4)");
                StopListenIPv4();
            }
        }
        /// <summary>
        /// 开始监听(IPv6)
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="port"></param>
        private void StartListenIPv6(EndPoint remoteAddress, ushort port)
        {
            if (_socketIPv6 != null)
            {
                OnStartListenFail(port, "已经开始监听(IPv6)");
                return;
            }
            _socketIPv6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            _socketIPv6.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//重用地址
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            try
            {
                _socketIPv6.Bind(localEndPoint);
                _socketIPv6.SendBufferSize = _netConfig.SendBufferSize;

                _sessionIPv6.RemoteAddress = remoteAddress;

                StartSessionIPv6();

                OnStartListenSuccess(port, "监听成功(IPv6)");
            }
            catch (Exception e)
            {
                OnStartListenFail(port, e.Message + "(IPv6)");
                StopListenIPv6();
            }
        }
        /// <summary>
        /// 关闭监听
        /// </summary>
        public void StopListen()
        {
            if (_isIntFlag == 0) return;
            StopListenIPv4();
            StopListenIPv6();
        }
        /// <summary>
        /// 关闭监听(IPv4)
        /// </summary>
        private void StopListenIPv4()
        {
            //关闭监听的Socket
            if (_socketIPv4 != null)
            {
                _socketIPv4.Close();
                _socketIPv4 = null;
                //关闭会话(IPv4)
                _sessionIPv4 = null;
            }
        }
        /// <summary>
        /// 关闭监听IPv6
        /// </summary>
        private void StopListenIPv6()
        {
            //关闭监听的Socket
            if (_socketIPv6 != null)
            {
                _socketIPv6.Close();
                _socketIPv6 = null;
                //关闭会话(IPv6)
                _sessionIPv6 = null;
            }
        }
        #endregion
        #region Receive
        /// <summary>
        /// 收到数据回调(IPv4)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void ReceiveCallBackIPv4(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            string message = "";
            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                if (socketAsyncEventArgs.BytesTransferred > 0)
                {
                    OnReceiveData(socketAsyncEventArgs.RemoteEndPoint, _sessionIPv4.Buffer, socketAsyncEventArgs.BytesTransferred);
                }
                //继续接收
                StartSessionIPv4();
                return;
            }
            else if (socketAsyncEventArgs.SocketError == SocketError.OperationAborted)
            {
                message = "Socket 已关闭(IPv4)";
            }
            else
            {
                message = socketAsyncEventArgs.SocketError.ToString() + "(IPv4)";
            }
            OnReceiveError(socketAsyncEventArgs.RemoteEndPoint, message);
            StopListenIPv4();
        }
        /// <summary>
        /// 收到数据回调(IPv6)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void ReceiveCallBackIPv6(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            string message = "";
            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                if (socketAsyncEventArgs.BytesTransferred > 0)
                {
                    OnReceiveData(socketAsyncEventArgs.RemoteEndPoint, _sessionIPv6.Buffer, socketAsyncEventArgs.BytesTransferred);
                }
                //继续接收
                StartSessionIPv6();
                return;
            }
            else if (socketAsyncEventArgs.SocketError == SocketError.OperationAborted)
            {
                message = "Socket 已关闭(IPv6)";
            }
            else
            {
                message = socketAsyncEventArgs.SocketError.ToString() + "(IPv6)";
            }
            OnReceiveError(socketAsyncEventArgs.RemoteEndPoint, message);
            StopListenIPv4();
        }
        #endregion
        #region Send
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public void Send(EndPoint endPoint, byte[] buffer, int index, int length)
        {
            if (_isIntFlag == 0)
            {
                throw new MethodAccessException("无法发送消息,应在启动监听后调用.");
            }
            try
            {
                if (_socketIPv4 != null)
                {
                    _socketIPv4.SendTo(buffer, index, length, SocketFlags.None, endPoint);
                }
                else if (_socketIPv6 != null)
                {
                    _socketIPv6.SendTo(buffer, index, length, SocketFlags.None, endPoint);
                }
            }
            catch
            {
                //已经处理
            }
        }
        #endregion
        #region Method
        /// <summary>
        /// 开始会话(IPv4)
        /// </summary>
        private void StartSessionIPv4()
        {
            _socketAsyncEventArgsIPv4.RemoteEndPoint = _sessionIPv4.RemoteAddress;
            if (!_socketIPv4.ReceiveFromAsync(_socketAsyncEventArgsIPv4))
            {
                ReceiveCallBackIPv4(_socketIPv4, _socketAsyncEventArgsIPv4);
            }
        }
        /// <summary>
        /// 开始会话(IPv6)
        /// </summary>
        private void StartSessionIPv6()
        {
            _socketAsyncEventArgsIPv6.RemoteEndPoint = _sessionIPv6.RemoteAddress;
            if (!_socketIPv6.ReceiveFromAsync(_socketAsyncEventArgsIPv6))
            {
                ReceiveCallBackIPv4(_socketIPv6, _socketAsyncEventArgsIPv6);
            }
        }
        #endregion
        #region Virtual Method
        /// <summary>
        /// 当启动失败时
        /// </summary>
        /// <param name="port"></param>
        /// <param name="error"></param>
        protected virtual void OnStartListenFail(ushort port, string error)
        {
            CxDebug.WriteLine("Socket OnStartListenFail port:{0} error:{1}", port, error);
        }
        /// <summary>
        /// 当启动成功时
        /// </summary>
        /// <param name="port"></param>
        /// <param name="message"></param>
        protected virtual void OnStartListenSuccess(ushort port, string message)
        {
            CxDebug.WriteLine("Socket OnStartListenSuccess port:{0} message:{1}", port, message);
        }
        /// <summary>
        /// 当接收发生错误
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="message"></param>
        protected virtual void OnReceiveError(EndPoint endPoint, string message)
        {
            CxDebug.WriteLine("Socket OnLossConnection endPoint:{0} message:{2}", endPoint.ToString(), message);
        }
        /// <summary>
        /// 当收到数据时
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        protected virtual void OnReceiveData(EndPoint endPoint, byte[] data, int length)
        {

        }
        /// <summary>
        /// 当关闭时
        /// </summary>
        /// <param name="error"></param>
        protected virtual void OnClose(string error)
        {
            CxDebug.WriteLine("Socket OnClose error:{0}", error);
        }
        #endregion
    }
}

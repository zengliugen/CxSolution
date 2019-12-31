using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using CxSolution.CxRouRou.Collections;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Util;

namespace CxSolution.CxRouRou.Net.Sockets.Tcp
{
    /// <summary>
    /// 关闭类型
    /// </summary>
    public enum CloseType
    {
        /// <summary>
        /// 主动关闭
        /// </summary>
        Close,
        /// <summary>
        /// 被关闭
        /// </summary>
        BeClose,
    }
    /// <summary>
    /// 网络基类
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
        /// 会话管理器
        /// </summary>
        private CxSessionManager _sessionManager;
        /// <summary>
        /// Socket异步操作参数对象池(由外部指定)
        /// </summary>
        private CxPool<SocketAsyncEventArgs> _saeaPool;
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
        protected CxNetBase()
        {
            _netConfig = new CxNetConfig { PoolSize = 1000, ReceiveBufferSize = 8196, ReceiveTimeout = 20000, SendBufferSize = 1400, SendTimeout = 20000, ListenIPv6 = false };
        }
        /// <summary>
        /// 设置网络配置
        /// </summary>
        /// <param name="poolSize"></param>
        /// <param name="receiveBufferSize"></param>
        /// <param name="receiveTimeout"></param>
        /// <param name="sendBufferSize"></param>
        /// <param name="sendTimeout"></param>
        /// <param name="listenIPv6"></param>
        public virtual void SetNetConfig(int poolSize = 1000, int receiveBufferSize = 8196, int receiveTimeout = 20000, int sendBufferSize = 1400, int sendTimeout = 20000, bool listenIPv6 = false)
        {
            if (_isIntFlag == 0)
            {
                throw new MethodAccessException("设置网络配置无效，应在启动服务或连接服务器之前调用");
            }
            _netConfig.PoolSize = poolSize;
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
            _saeaPool = CxPool<SocketAsyncEventArgs>.ThreadSafePool(_netConfig.PoolSize, () =>
            {
                return new SocketAsyncEventArgs();
            });
            _sessionManager = new CxSessionManager(_netConfig.PoolSize, _netConfig.ReceiveBufferSize, _saeaPool);
        }
        #region Accept
        /// <summary>
        /// 开始监听接收连接
        /// </summary>
        /// <param name="port"></param>
        public void StartListenAccept(ushort port)
        {
            Init();
            StartListenAcceptIPv4(port);
            if (_netConfig.ListenIPv6)
            {
                StartListenAcceptIPv6(port);
            }
        }
        /// <summary>
        /// 开始监听接受连接(IPv4)
        /// </summary>
        /// <param name="port"></param>
        private void StartListenAcceptIPv4(ushort port)
        {
            if (_socketIPv4 != null)
            {
                OnStartListenAcceptFail(port, "已经开始监听(IPv4)");
                return;
            }
            _socketIPv4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketIPv4.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); //重用地址

            SocketAsyncEventArgs socketAsyncEventArgs = PopAcceptSaea();
            socketAsyncEventArgs.Completed += AcceptCallbackIPv4;
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, port);
            try
            {
                _socketIPv4.Bind(iPEndPoint);
                _socketIPv4.Listen(int.MaxValue);
                if (!_socketIPv4.AcceptAsync(socketAsyncEventArgs))
                {
                    AcceptCallbackIPv4(_socketIPv4, socketAsyncEventArgs);
                }
                OnStartListenAcceptSuccess(port, "监听成功(IPv4)");
            }
            catch (Exception e)
            {
                socketAsyncEventArgs.Completed -= AcceptCallbackIPv4;
                PushAcceptSaea(socketAsyncEventArgs);
                OnStartListenAcceptFail(port, e.Message);
                StopAcceptIPv4();
            }
        }
        /// <summary>
        /// 开始监听接受连接(IPv6)
        /// </summary>
        /// <param name="port"></param>
        private void StartListenAcceptIPv6(ushort port)
        {
            if (_socketIPv6 != null)
            {
                OnStartListenAcceptFail(port, "已经开始监听(IPv6)");
                return;
            }
            _socketIPv6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            _socketIPv6.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); //重用地址

            SocketAsyncEventArgs socketAsyncEventArgs = PopAcceptSaea();
            socketAsyncEventArgs.Completed += AcceptCallbackIPv6;
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.IPv6Any, port);
            try
            {
                _socketIPv6.Bind(iPEndPoint);
                _socketIPv6.Listen(int.MaxValue);
                if (!_socketIPv6.AcceptAsync(socketAsyncEventArgs))
                {
                    AcceptCallbackIPv6(_socketIPv6, socketAsyncEventArgs);
                }
                OnStartListenAcceptSuccess(port, "监听成功(IPv6)");
            }
            catch (Exception e)
            {
                socketAsyncEventArgs.Completed -= AcceptCallbackIPv6;
                PushAcceptSaea(socketAsyncEventArgs);
                OnStartListenAcceptFail(port, e.Message);
                StopAcceptIPv6();
            }
        }
        /// <summary>
        /// 接受连接回调(IPv4)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void AcceptCallbackIPv4(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                CxSession session = _sessionManager.Pop();
                session.SetSocket(socketAsyncEventArgs.AcceptSocket, socketAsyncEventArgs.AcceptSocket.AddressFamily, _netConfig.SendBufferSize, _netConfig.SendTimeout, _netConfig.ReceiveTimeout);
                OnAcceptSuccess(session.ID, socketAsyncEventArgs.AcceptSocket.RemoteEndPoint.ToString());
                StartSession(session, null);
                socketAsyncEventArgs.AcceptSocket = null;
                try
                {
                    if (!_socketIPv4.AcceptAsync(socketAsyncEventArgs))
                    {
                        AcceptCallbackIPv4(_socketIPv4, socketAsyncEventArgs);
                    }
                }
                catch (ObjectDisposedException)
                {
                    //主动关闭的
                    socketAsyncEventArgs.Completed -= AcceptCallbackIPv4;
                    PushAcceptSaea(socketAsyncEventArgs);
                }
                catch (NullReferenceException)
                {
                    //主动关闭的
                    socketAsyncEventArgs.Completed -= AcceptCallbackIPv4;
                    PushAcceptSaea(socketAsyncEventArgs);
                }
                catch (Exception e)
                {
                    socketAsyncEventArgs.Completed -= AcceptCallbackIPv4;
                    PushAcceptSaea(socketAsyncEventArgs);
                    OnAcceptFail(e.Message);
                    StopAcceptIPv4();
                }
                return;
            }
            if (socketAsyncEventArgs.SocketError == SocketError.OperationAborted)
            {
                //主动关闭的   
                socketAsyncEventArgs.Completed -= AcceptCallbackIPv4;
                PushAcceptSaea(socketAsyncEventArgs);
                return;
            }
            socketAsyncEventArgs.Completed -= AcceptCallbackIPv4;
            PushAcceptSaea(socketAsyncEventArgs);
            OnAcceptFail(socketAsyncEventArgs.SocketError.ToString());
            StopAcceptIPv4();
        }
        /// <summary>
        /// 接受连接回调(IPv6)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void AcceptCallbackIPv6(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                CxSession session = _sessionManager.Pop();
                session.SetSocket(socketAsyncEventArgs.AcceptSocket, socketAsyncEventArgs.AcceptSocket.AddressFamily, _netConfig.SendBufferSize, _netConfig.SendTimeout, _netConfig.ReceiveTimeout);
                OnAcceptSuccess(session.ID, socketAsyncEventArgs.AcceptSocket.RemoteEndPoint.ToString());
                StartSession(session, null);
                socketAsyncEventArgs.AcceptSocket = null;
                try
                {
                    if (!_socketIPv6.AcceptAsync(socketAsyncEventArgs))
                    {
                        AcceptCallbackIPv6(_socketIPv6, socketAsyncEventArgs);
                    }
                }
                catch (ObjectDisposedException)
                {
                    //主动关闭的
                    socketAsyncEventArgs.Completed -= AcceptCallbackIPv6;
                    PushAcceptSaea(socketAsyncEventArgs);
                }
                catch (NullReferenceException)
                {
                    //主动关闭的
                    socketAsyncEventArgs.Completed -= AcceptCallbackIPv6;
                    PushAcceptSaea(socketAsyncEventArgs);
                }
                catch (Exception e)
                {
                    socketAsyncEventArgs.Completed -= AcceptCallbackIPv6;
                    PushAcceptSaea(socketAsyncEventArgs);
                    OnAcceptFail(e.Message);
                    StopAcceptIPv6();
                }
                return;
            }
            if (socketAsyncEventArgs.SocketError == SocketError.OperationAborted)
            {
                //主动关闭的   
                socketAsyncEventArgs.Completed -= AcceptCallbackIPv6;
                PushAcceptSaea(socketAsyncEventArgs);
                return;
            }
            socketAsyncEventArgs.Completed -= AcceptCallbackIPv6;
            PushAcceptSaea(socketAsyncEventArgs);
            OnAcceptFail(socketAsyncEventArgs.SocketError.ToString());
            StopAcceptIPv6();
        }
        /// <summary>
        /// 关闭接受连接
        /// </summary>
        public void StopAccept()
        {
            if (_isIntFlag == 0) return;
            OnClose(CxString.Format("!SocketClose OnlineNum:{0}", _sessionManager.OnlineNum));
            StopAcceptIPv4();
            StopAcceptIPv6();
        }
        /// <summary>
        /// 关闭接受连接(IPv4)
        /// </summary>
        private void StopAcceptIPv4()
        {
            //关闭接受连接的Socket
            if (_socketIPv4 != null)
            {
                _socketIPv4.Close();
                _socketIPv4 = null;
                //关闭所有在线会话(IPv4)
                ICollection<CxSession> onlines = _sessionManager.GetAllOnline();
                List<CxSession> removeSessions = new List<CxSession>(onlines.Count);
                foreach (var session in onlines)
                {
                    if (session.NetType == AddressFamily.InterNetwork)
                    {
                        removeSessions.Add(session);
                    }
                }
                foreach (var session in removeSessions)
                {
                    //关闭会话
                    session.Close();
                }
            }
        }
        /// <summary>
        /// 关闭接受连接(IPv6)
        /// </summary>
        private void StopAcceptIPv6()
        {
            //关闭接受连接的Socket
            if (_socketIPv6 != null)
            {
                _socketIPv6.Close();
                _socketIPv6 = null;
                //关闭所有在线会话(IPv6)
                ICollection<CxSession> onlines = _sessionManager.GetAllOnline();
                List<CxSession> removeSessions = new List<CxSession>(onlines.Count);
                foreach (var session in onlines)
                {
                    if (session.NetType == AddressFamily.InterNetworkV6)
                    {
                        removeSessions.Add(session);
                    }
                }
                foreach (var session in removeSessions)
                {
                    //关闭会话
                    session.Close();
                }
            }
        }
        #endregion
        #region Connect
        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="comparer">排序方法</param>
        public void StartConnect(string host, ushort port, IComparer<IPAddress> comparer = null)
        {
            Init();
            var ipAddressArray = GetIPAddress(host, port, comparer);
            if (ipAddressArray != null && ipAddressArray.Length > 0)
            {
                var ipAddress = ipAddressArray[0];
                var iPEndPoint = new IPEndPoint(ipAddress, port);
                StartConnect(iPEndPoint, ipAddress.AddressFamily);
            }
            else
            {
                OnConnetFail(host, "无法解析的地址");
            }
        }
        /// <summary>
        /// 获取指定域名或者ip字符串的IP地址信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="comparer">排序方法</param>
        /// <returns></returns>
        private IPAddress[] GetIPAddress(string host, ushort port, IComparer<IPAddress> comparer = null)
        {
            var iPAddressList = new List<IPAddress>();
            if (CxNetTool.IsIP(host))
            {
                IPAddress.TryParse(host, out IPAddress iPAddress);
                iPAddressList.Add(iPAddress);
            }
            else
            {
                var hostEntry = Dns.GetHostEntry(host);
                if (hostEntry != null)
                {
                    iPAddressList.AddRange(hostEntry.AddressList);
                }
            }
            if (comparer != null)
            {
                iPAddressList.Sort(comparer);
            }
            return iPAddressList.ToArray();
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="iPEndPoint"></param>
        /// <param name="addressFamily"></param>
        private void StartConnect(IPEndPoint iPEndPoint, AddressFamily addressFamily)
        {
            Socket socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//重用地址
            SocketAsyncEventArgs socketAsyncEventArgs = PopConnectSaea();
            socketAsyncEventArgs.Completed += ConnectCallback;
            socketAsyncEventArgs.RemoteEndPoint = iPEndPoint;
            try
            {
                if (!socket.ConnectAsync(socketAsyncEventArgs))
                    ConnectCallback(socket, socketAsyncEventArgs);
            }
            catch (Exception e)
            {
                socket.Close();
                socketAsyncEventArgs.Completed -= ConnectCallback;
                PushConnectSaea(socketAsyncEventArgs);
                OnConnetFail(iPEndPoint.ToString(), e.Message);
            }
        }
        /// <summary>
        /// 连接回调(IPv4)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void ConnectCallback(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            Socket socket = (Socket)sender;
            EndPoint endPoint = socketAsyncEventArgs.RemoteEndPoint;
            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                var session = _sessionManager.Pop();
                session.SetSocket(socket, socket.AddressFamily, _netConfig.SendBufferSize, _netConfig.SendTimeout, _netConfig.ReceiveTimeout);
                OnConnetSuccess(session.ID, endPoint.ToString());
                StartSession(session, null);
                return;
            }
            socket.Close();
            socketAsyncEventArgs.Completed -= ConnectCallback;
            PushConnectSaea(socketAsyncEventArgs);
            OnConnetFail(endPoint.ToString(), socketAsyncEventArgs.SocketError.ToString());
        }
        /// <summary>
        /// 停止连接
        /// </summary>
        public void StopConnect()
        {
            if (_isIntFlag == 0) return;
            OnClose(CxString.Format("!SocketClose OnlineNum:{0}", _sessionManager.OnlineNum));
            //关闭所有在线会话
            ICollection<CxSession> onlines = _sessionManager.GetAllOnline();
            List<CxSession> removeSessions = new List<CxSession>(onlines.Count);
            foreach (var session in onlines)
            {
                removeSessions.Add(session);
            }
            foreach (var session in removeSessions)
            {
                //关闭会话
                session.Close();
            }
        }
        #endregion
        #region Send
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="useAsync"></param>
        public void Send(uint id, byte[] buffer, int index, int length, bool useAsync)
        {
            if (_isIntFlag == 0) return;
            CxSession session = _sessionManager.GetOnline(id);
            if (session != null)
            {
                if (useAsync)
                {
                    session.SendAsync(buffer, index, length);
                    return;
                }
                try
                {
                    session.Socket.Send(buffer, index, length, SocketFlags.None);
                }
                catch
                {
                    //已经处理
                }
            }
        }
        #endregion
        #region Receive
        /// <summary>
        /// 接收数据回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void ReceiveCallBack(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            CxSession session = (CxSession)socketAsyncEventArgs.UserToken;
            CloseType closeType = CloseType.BeClose;
            string message = "远程端已关闭连接";
            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                if (socketAsyncEventArgs.BytesTransferred > 0)
                {
                    OnReceiveData(session, socketAsyncEventArgs.BytesTransferred + session.Offset);
                    StartSession(session, socketAsyncEventArgs);
                    return;
                }
                else
                {
                    // 如果从读取操作返回零，则远程端已关闭连接。
                }

            }
            else if (socketAsyncEventArgs.SocketError == SocketError.OperationAborted)
            {
                //主动关闭
                closeType = CloseType.Close;
                message = "Socket 已关闭";
            }
            else
            {
                session.Close();
            }
            PushReceiveSaea(socketAsyncEventArgs);
            StopSession(session, closeType, message);
        }
        #endregion
        #region SaeaPool
        /// <summary>
        /// 取得一个接受连接所用的Socket异步操作参数
        /// </summary>
        /// <returns></returns>
        private SocketAsyncEventArgs PopAcceptSaea()
        {
            SocketAsyncEventArgs socketAsyncEventArgs = _saeaPool.Pop();
            return socketAsyncEventArgs;
        }
        /// <summary>
        /// 回收一个接受连接所用的Socket异步操作参数
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void PushAcceptSaea(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs == null)
            {
                return;
            }
            _saeaPool.Push(socketAsyncEventArgs);
        }
        /// <summary>
        /// 取得一个接收数据所用的Socket异步操作参数
        /// </summary>
        /// <returns></returns>
        private SocketAsyncEventArgs PopReceiveSaea()
        {
            var socketAsyncEventArgs = _saeaPool.Pop();
            socketAsyncEventArgs.Completed += ReceiveCallBack;
            return socketAsyncEventArgs;
        }
        /// <summary>
        /// 回收一个接收数据所用的Socket异步操作参数
        /// </summary>
        /// <returns></returns>
        private void PushReceiveSaea(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs == null)
            {
                return;
            }
            socketAsyncEventArgs.Completed -= ReceiveCallBack;
            _saeaPool.Push(socketAsyncEventArgs);
        }
        /// <summary>
        /// 取得一个连接所用的Socket异步操作参数
        /// </summary>
        /// <returns></returns>
        private SocketAsyncEventArgs PopConnectSaea()
        {
            var socketAsyncEventArgs = _saeaPool.Pop();
            return socketAsyncEventArgs;
        }
        /// <summary>
        /// 回收一个连接所用的Socket异步操作参数
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void PushConnectSaea(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            _saeaPool.Push(socketAsyncEventArgs);
        }
        #endregion
        #region Method
        /// <summary>
        /// 开始一个会话
        /// </summary>
        /// <param name="session"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void StartSession(CxSession session, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs == null)
            {
                socketAsyncEventArgs = PopReceiveSaea();
                socketAsyncEventArgs.UserToken = session;
            }
            socketAsyncEventArgs.SetBuffer(session.Buffer, session.Offset, session.Buffer.Length - session.Offset);
            try
            {
                if (!session.Socket.ReceiveAsync(socketAsyncEventArgs))
                {
                    ReceiveCallBack(session.Socket, socketAsyncEventArgs);
                }
            }
            catch
            {
                //主动关闭的
                StopSession(session, CloseType.Close, "Socket 已关闭");
            }
        }
        /// <summary>
        /// 停止并回收会话
        /// </summary>
        /// <param name="session"></param>
        /// <param name="closeType"></param>
        /// <param name="message"></param>
        private void StopSession(CxSession session, CloseType closeType, string message)
        {
            //回收会话需要ID支持
            uint id = session.ID;
            session.Clear();
            _sessionManager.Push(id, session);
            OnLossConnection(id, closeType, message);
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="id"></param>
        public void Close(uint id)
        {
            if (_isIntFlag == 0) return;
            CxSession session = _sessionManager.GetOnline(id);
            if (session != null)
            {
                session.Close();
            }
        }
        /// <summary>
        /// 关闭所有连接
        /// </summary>
        public void CloseAll()
        {
            if (_isIntFlag == 0) return;
            ICollection<CxSession> onlines = _sessionManager.GetAllOnline();
            List<CxSession> removeSessions = new List<CxSession>(onlines.Count);
            foreach (var session in onlines)
            {
                removeSessions.Add(session);
            }
            foreach (var session in removeSessions)
            {
                session.Close();
            }
        }
        /// <summary>
        /// 获取在线数量
        /// </summary>
        public int OnlineNum
        {
            get
            {
                return _sessionManager.OnlineNum;
            }
        }
        #endregion
        #region Virtual Method
        /// <summary>
        /// 当启动失败时
        /// </summary>
        /// <param name="port"></param>
        /// <param name="error"></param>
        protected virtual void OnStartListenAcceptFail(ushort port, string error)
        {
            CxDebug.WriteLine("Socket OnStartListenAcceptFail port:{0} error:{1}", port, error);
        }
        /// <summary>
        /// 当启动成功时
        /// </summary>
        /// <param name="port"></param>
        /// <param name="message"></param>
        protected virtual void OnStartListenAcceptSuccess(ushort port, string message)
        {
            CxDebug.WriteLine("Socket OnStartListenAcceptSuccess port:{0} message:{1}", port, message);
        }
        /// <summary>
        /// 当接受连接失败
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnAcceptFail(string message)
        {
            CxDebug.WriteLine("Socket OnAcceptFail message:{0}", message);
        }
        /// <summary>
        /// 当接受到连接时
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        protected virtual void OnAcceptSuccess(uint id, string address)
        {
            CxDebug.WriteLine("Socket OnAcceptSuccess id:{0} address:{1}", id, address);
        }
        /// <summary>
        /// 当连接成功时
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        protected virtual void OnConnetSuccess(uint id, string address)
        {
            CxDebug.WriteLine("Socket OnConnetSuccess id:{0} address:{1}", id, address);
        }
        /// <summary>
        /// 当连接失败时
        /// </summary>
        /// <param name="address"></param>
        /// <param name="error"></param>
        protected virtual void OnConnetFail(string address, string error)
        {
            CxDebug.WriteLine("Socket OnConnetFail address:{0} CloseType:{1}", address, error);
        }
        /// <summary>
        /// 当连接断开时
        /// </summary>
        /// <param name="id"></param>
        /// <param name="closeType"></param>
        /// <param name="message"></param>
        protected virtual void OnLossConnection(uint id, CloseType closeType, string message)
        {
            CxDebug.WriteLine("Socket OnLossConnection id:{0} closeType:{1} message:{2}", id, closeType, message);
        }
        /// <summary>
        /// 当收到数据时
        /// </summary>
        /// <param name="receiveData"></param>
        /// <param name="length"></param>
        protected virtual void OnReceiveData(IReceiveData receiveData, int length)
        {
            receiveData.Offset = 0;
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

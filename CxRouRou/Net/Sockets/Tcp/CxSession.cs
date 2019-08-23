using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using CxSolution.CxRouRou.Collections;

namespace CxSolution.CxRouRou.Net.Sockets.Tcp
{
    /// <summary>
    /// 会话网络类型枚举
    /// </summary>
    public enum ECxSessionNetType
    {
        /// <summary>
        /// 默认
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        IPv4,
        /// <summary>
        /// 
        /// </summary>
        IPv6,
    }
    /// <summary>
    /// 会话
    /// </summary>
    public class CxSession : IReceiveData
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public uint ID { get; internal set; }
        /// <summary>
        /// 会话网络类型
        /// </summary>
        public AddressFamily NetType { get; private set; }
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        public byte[] Buffer { get; private set; }
        /// <summary>
        /// 数据缓冲区接收偏移
        /// </summary>
        public int Offset { internal get; set; }
        /// <summary>
        /// 会话Socket
        /// </summary>
        internal Socket Socket;
        /// <summary>
        /// Socket异步操作参数
        /// </summary>
        private SocketAsyncEventArgs _sendSaea;
        /// <summary>
        /// Socket异步操作参数对象池(由外部指定)
        /// </summary>
        private readonly CxPool<SocketAsyncEventArgs> _saeaPool;
        /// <summary>
        /// 发送数据循环列表
        /// </summary>
        private readonly CxRoundQueue<IList<ArraySegment<byte>>> _sendList;
        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly object _syncLock = new object();
        /// <summary>
        /// 是否发送标记
        /// </summary>
        private bool _isSend = false;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="receiveBufferSize"></param>
        /// <param name="saeaPool"></param>
        public CxSession(int receiveBufferSize, CxPool<SocketAsyncEventArgs> saeaPool)
        {
            _saeaPool = saeaPool;
            Buffer = new byte[receiveBufferSize];
            _sendList = new CxRoundQueue<IList<ArraySegment<byte>>>(2, () =>
            {
                return new List<ArraySegment<byte>>(4);
            });
        }
        /// <summary>
        /// 设置Socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="netType"></param>
        /// <param name="sendBufferSize"></param>
        /// <param name="sendTimeout"></param>
        /// <param name="receiveTimeout"></param>
        internal void SetSocket(Socket socket, AddressFamily netType, int sendBufferSize, int sendTimeout, int receiveTimeout)
        {
            Socket = socket;
            Socket.SendBufferSize = sendBufferSize;
            Socket.SendTimeout = sendTimeout;
            Socket.ReceiveTimeout = receiveTimeout;
            Socket.NoDelay = !false;// 不使用nagle算法延迟(将收到发送请求就立即发送数据)  对udp无效
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            NetType = netType;

            _sendSaea = _saeaPool.Pop();
            _sendSaea.Completed += SendCallBack;
        }
        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCallBack(object sender, SocketAsyncEventArgs e)
        {
            lock (_syncLock)
            {
                //此处使用的是循环列表中的对象，清空即可
                e.BufferList.Clear();
                e.BufferList = null;
            }
            if (e.SocketError == SocketError.Success)
            {
                SendList();
                return;
            }
            PushSendSaea();
        }
        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        internal void SendAsync(byte[] data, int index, int length)
        {
            lock (_syncLock)
            {
                if (Socket == null)
                {
                    //此处表示连接被关闭
                    return;
                }
                _sendList.Peek().Add(new ArraySegment<byte>(data, index, length));
            }
            SendList();
        }
        /// <summary>
        /// 发送列表中的数据
        /// </summary>
        private void SendList()
        {
            _isSend = false;
            try
            {
                lock (_syncLock)
                {
                    if (_sendList.Peek().Count > 0 && _sendSaea.BufferList == null)
                    {
                        _isSend = true;
                        _sendSaea.BufferList = _sendList.Dequeue();
                    }
                }
                if (_isSend)
                {
                    if (!Socket.SendAsync(_sendSaea))
                        SendCallBack(Socket, _sendSaea);
                }
            }
            catch //Socket 可能已被关闭
            {
                PushSendSaea();
            }
        }
        /// <summary>
        /// 回收发送Socket异步操作参数
        /// </summary>
        private void PushSendSaea()
        {
            var tempSendSaea = _sendSaea;
            lock (_syncLock)
            {
                if (_sendSaea == null) return;
                _sendSaea = null;
            }
            tempSendSaea.Completed -= SendCallBack;
            //此时可能还未发送过消息，BufferList为空
            if (tempSendSaea.BufferList != null)
            {
                tempSendSaea.BufferList.Clear();
                tempSendSaea.BufferList = null;
            }
            _saeaPool.Push(tempSendSaea);
        }
        /// <summary>
        /// 清理Sesion
        /// </summary>
        internal void Clear()
        {
            Offset = 0;
            ID = 0;
            lock (_syncLock)
            {
                foreach (var item in _sendList)
                {
                    item.Clear();
                }
            }
        }
        /// <summary>
        /// 关闭Session
        /// </summary>
        internal void Close()
        {
            lock (_syncLock)
            {
                if (Socket != null)
                {
                    //关闭Socket
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                    Socket = null;
                    NetType = AddressFamily.Unknown;
                }
            }
            PushSendSaea(); //从来没有调用过发送的时候  可以在这里回收
        }
    }
}

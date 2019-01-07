//是否使用uint作为长度
#define CX_MSGPACKET_USE_UINT_LENGTH
#undef CX_MSGPACKET_USE_UINT_LENGTH
using CxRouRou.Collections;
using CxRouRou.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CxRouRou.Net.Sockets.Tcp
{
    /// <summary>
    /// 网络
    /// </summary>
    public class CxNet : CxNetBase, IDisposable
    {
        /// <summary>
        /// 错误类型
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// 默认
            /// </summary>
            None = 0,
            /// <summary>
            /// 错误的消息标志
            /// </summary>
            MsgFlagError = 1001,
            /// <summary>
            /// 未注册的指令
            /// </summary>
            CmdNotRegion = 1002,
            /// <summary>
            /// 接收数据长度错误
            /// </summary>
            ReceiveDataLengthError = 4001,
            /// <summary>
            /// 指令处理错误
            /// </summary>
            CmdHandlerError = 7001,
        }

        /// <summary>
        /// 消息标志
        /// </summary>
        public const ushort MsgFlag = 0x0328;
        /// <summary>
        /// 发送消息队列数据
        /// </summary>
        private struct SendMsgQueueData
        {
            public uint ID;
            public byte[] Data;
        }
        /// <summary>
        /// 消息派发方法
        /// </summary>
        private readonly Dictionary<ushort, Action<uint, CxMsgPacket>> _msgHandler;
        /// <summary>
        /// 派发消息队列
        /// </summary>
        private readonly CxRoundQueue<Queue<SendMsgQueueData>> _dispacthMsgQueue;
        /// <summary>
        /// 是否使用派发消息队列
        /// </summary>
        private readonly bool _useDispacthMsgQueue;
        /// <summary>
        /// 派发消息线程
        /// </summary>
        private readonly Thread _dispacthMsgThread;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="msgHandlerSize"></param>
        /// <param name="useDispacthMsgQueue"></param>
        public CxNet(int msgHandlerSize = 512, bool useDispacthMsgQueue = true)
        {
            _msgHandler = new Dictionary<ushort, Action<uint, CxMsgPacket>>(msgHandlerSize);
            _useDispacthMsgQueue = useDispacthMsgQueue;
            if (useDispacthMsgQueue)
            {
                _dispacthMsgQueue = new CxRoundQueue<Queue<SendMsgQueueData>>(2, () => new Queue<SendMsgQueueData>(256));
                _dispacthMsgThread = CxThread.StartThread(() =>
                  {
                      while (true)
                      {
                          Queue<SendMsgQueueData> msgs;
                          lock (_dispacthMsgQueue)
                          {
                              msgs = _dispacthMsgQueue.Dequeue();
                          }
                          while (msgs.Count > 0)
                          {
                              SendMsgQueueData msgData = msgs.Dequeue();
                              OnReceiveMsgPacket(msgData.ID, msgData.Data);
                          }
                          Thread.Sleep(1);
                      }
                  }, false);
            }
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
        public override void SetNetConfig(int poolSize = 1000, int receiveBufferSize = 8196, int receiveTimeout = 20000, int sendBufferSize = 1400, int sendTimeout = 20000, bool listenIPv6 = false)
        {
#if DEBUG
            //检测参数是否错误
#if CX_MSGPACKET_USE_UINT_LENGTH
            if (receiveBufferSize > int.MaxValue || sendBufferSize > int.MaxValue)
            {
                if (receiveBufferSize > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("receiveBufferSize", CxString.Format("发送缓冲区太大 CurSize:{0} MaxSize:{1}", receiveBufferSize, int.MaxValue));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("sendBufferSize", CxString.Format("接收缓冲区太大 CurSize:{0} MaxSize:{1}", sendBufferSize, int.MaxValue));
                }
            }
#else
            if (receiveBufferSize > ushort.MaxValue || sendBufferSize > ushort.MaxValue)
            {
                if (receiveBufferSize > ushort.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("receiveBufferSize", CxString.Format("发送缓冲区太大 CurSize:{0} MaxSize:{1}", receiveBufferSize, ushort.MaxValue));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("sendBufferSize", CxString.Format("接收缓冲区太大 CurSize:{0} MaxSize:{1}", sendBufferSize, ushort.MaxValue));
                }
            }
#endif
#endif
            base.SetNetConfig(poolSize, receiveBufferSize, receiveTimeout, sendBufferSize, sendTimeout, listenIPv6);
        }
        /// <summary>
        /// 添加指令处理函数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        public void AddMsgHandler(ushort cmd, Action<uint, CxMsgPacket> handler)
        {
            _msgHandler[cmd] = handler ?? throw new ArgumentNullException("handler");
        }
        /// <summary>
        /// 派发消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cmd"></param>
        /// <param name="msgPacket"></param>
        private void Dispatch(uint id, ushort cmd, CxMsgPacket msgPacket)
        {
            if (_msgHandler.TryGetValue(cmd, out Action<uint, CxMsgPacket> handler))
            {
                try
                {
                    handler.Invoke(id, msgPacket);
                }
                catch (Exception e)
                {
                    OnError(id, ErrorType.CmdHandlerError, CxString.Format("cmd:{0} - Error:{1}", cmd, e.Message));
                }
            }
            else
            {
                OnError(id, ErrorType.CmdNotRegion, CxString.Format("cmd:{0} - 指令未注册", cmd));
            }
        }
        /// <summary>
        /// 当收到消息包
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        private void OnReceiveMsgPacket(uint id, byte[] data)
        {
            BeforeUnPacket(id, data, CxMsgPacket.LengthSize, data.Length - CxMsgPacket.LengthSize);
            CxMsgPacket msgPacket = new CxMsgPacket(data) { ReadPos = CxMsgPacket.LengthSize };//从CxMsgPacket.LengthSize开始读取
            ushort msgFlag = msgPacket.Pop_ushort();
            ushort cmd = msgPacket.Pop_ushort();
            if (msgFlag == MsgFlag)
            {
                if (BeforeDispacth(id, cmd))
                {
                    Dispatch(id, cmd, msgPacket);
                }
            }
            else
            {
                OnError(id, ErrorType.MsgFlagError, "错误的消息标志");
            }
        }
        /// <summary>
        /// 当收到数据 如非必要 不要重写该函数
        /// </summary>
        /// <param name="receiveData"></param>
        /// <param name="length"></param>
        protected override void OnReceiveData(IReceiveData receiveData, int length)
        {
            //预留字节CxMsgPacket.FreeNum包含(CxMsgPacket.LengthSize字节长度 CxMsgPacket.FlagSize字节标志)CxMsgPacket.CmdSize字节指令
#if CX_MSGPACKET_USE_UINT_LENGTH
            int packageLength = 0;
#else
            ushort packageLength = 0;
#endif
            CxMsgPacket msgPacket = new CxMsgPacket(receiveData.Buffer);
            if (length >= CxMsgPacket.LengthSize)//需要CxMsgPacket.LengthSize字节才可解出包长
            {
                //获取包长
#if CX_MSGPACKET_USE_UINT_LENGTH
                packageLength = (int)msgPacket.Pop_uint();
#else
                packageLength = msgPacket.Pop_ushort();
#endif
                while (packageLength >= CxMsgPacket.FreeNum - CxMsgPacket.LengthSize &&//最小包长度 至少需(CxMsgPacket.FreeNum - CxMsgPacket.LengthSize)字节
                        packageLength <= _netConfig.ReceiveBufferSize &&//小于接收缓冲区大小
                        packageLength <= (length - msgPacket.ReadPos))//小于剩余数据长度，否则可能断包，需要粘包处理
                {
                    //得到完整的包
                    if (_useDispacthMsgQueue)
                    {
                        lock (_dispacthMsgQueue)
                        {
                            _dispacthMsgQueue.Peek().Enqueue(new SendMsgQueueData()
                            {
                                ID = receiveData.ID,
                                Data = msgPacket.GetBytesAt(msgPacket.ReadPos - CxMsgPacket.LengthSize, packageLength + CxMsgPacket.LengthSize),
                            });
                        }
                    }
                    else
                    {
                        OnReceiveMsgPacket(receiveData.ID, msgPacket.GetBytesAt(msgPacket.ReadPos - CxMsgPacket.LengthSize, packageLength + CxMsgPacket.LengthSize));
                    }
                    msgPacket.ReadPos += packageLength;
                    if (msgPacket.ReadPos + CxMsgPacket.LengthSize <= length)//还能解出一个包头
                    {
#if CX_MSGPACKET_USE_UINT_LENGTH
                        packageLength = (int)msgPacket.Pop_uint();
#else
                        packageLength = msgPacket.Pop_ushort();
#endif
                    }
                    else//不能解出一个包头
                    {
                        //读取偏移增加,下一次while进行判断
                        msgPacket.ReadPos += CxMsgPacket.LengthSize;
                    }
                }
                msgPacket.ReadPos -= CxMsgPacket.LengthSize; //修正当前位置到包头位置
            }
            if ((length >= msgPacket.ReadPos + CxMsgPacket.LengthSize) &&//能解出包长
                (packageLength > _netConfig.ReceiveBufferSize - CxMsgPacket.LengthSize ||//包长大于了接收缓冲区大小
                packageLength < CxMsgPacket.FreeNum - CxMsgPacket.LengthSize))//小于最小包长度 至少需(CxMsgPacket.FreeNum - CxMsgPacket.LengthSize)字节
            {
                //长度错误
                receiveData.Offset = 0;
                OnError(receiveData.ID, ErrorType.ReceiveDataLengthError, CxString.Format("Length:{0} - 数据长度错误", packageLength));
            }
            if (length == msgPacket.ReadPos)//刚好解完数据
            {
                receiveData.Offset = 0;
            }
            else if (msgPacket.ReadPos == 0)//根本没有解
            {
                receiveData.Offset = length;
            }
            else//断包 继续接收
            {
                //粘包处理
                Array.Copy(receiveData.Buffer, msgPacket.ReadPos, receiveData.Buffer, 0, length - msgPacket.ReadPos);
                receiveData.Offset = length - msgPacket.ReadPos;
            }
        }
        /// <summary>
        /// 发送消息包
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cmd"></param>
        /// <param name="msgPacket"></param>
        /// <param name="useAsync"></param>
        public void SendMsgPacket(uint id, ushort cmd, CxMsgPacket msgPacket, bool useAsync = true)
        {
            if (msgPacket.Length > _netConfig.SendBufferSize)
            {
                throw new ArgumentOutOfRangeException("msgPacket", CxString.Format("发送数据过长 超过了SendBufferSize设置的大小 msgPacket.Length:{0} SendBufferSize:{1}", msgPacket.Length, _netConfig.SendBufferSize));
            }
            msgPacket.Package(MsgFlag, cmd);
            BeforeSend(id, msgPacket.Data, CxMsgPacket.LengthSize, msgPacket.Length - CxMsgPacket.LengthSize);
            Send(id, msgPacket.Data, 0, msgPacket.Length, useAsync);
        }
        /// <summary>
        /// 释放非托管对象
        /// </summary>
        public void Dispose()
        {
            if (_dispacthMsgThread != null)
            {
                _dispacthMsgThread.Abort();
            }
        }
        #region Virtual Method
        /// <summary>
        /// 当发生错误时
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorType"></param>
        /// <param name="error"></param>
        protected virtual void OnError(uint id, ErrorType errorType, string error)
        {

        }
        /// <summary>
        /// 在发送之前 可以进行加密操作 (只可从index开始)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        protected virtual void BeforeSend(uint id, byte[] data, int index, int length)
        {

        }
        /// <summary>
        /// 在解包之前 可以进行解密之类的操作 (只可从index开始)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        protected virtual void BeforeUnPacket(uint id, byte[] data, int index, int length)
        {

        }
        /// <summary>
        /// 在派发之前 可以进行指令的有效性检测
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected virtual bool BeforeDispacth(uint id, ushort cmd)
        {
            return true;
        }
        #endregion
    }
}

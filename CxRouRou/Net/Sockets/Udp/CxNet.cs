using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Util;

namespace CxSolution.CxRouRou.Net.Sockets.Udp
{
    /// <summary>
    /// 网络类(Udp)
    /// </summary>
    public class CxNet : CxNetBase
    {
        /// <summary>
        /// 地址信息
        /// </summary>
        private readonly Dictionary<Tuple<string, ushort>, EndPoint> _hostInfoMap;
        /// <summary>
        /// 初始化
        /// </summary>
        public CxNet()
        {
            _hostInfoMap = new Dictionary<Tuple<string, ushort>, EndPoint>();
            CxDebug.WriteLine("当前最大支持消息包长度为:{0}", CxMsgPacket.MaxLength);
        }
        /// <summary>
        /// 设置网络配置
        /// </summary>
        /// <param name="receiveBufferSize"></param>
        /// <param name="receiveTimeout"></param>
        /// <param name="sendBufferSize"></param>
        /// <param name="sendTimeout"></param>
        /// <param name="listenIPv6"></param>
        public override void SetNetConfig(int receiveBufferSize = 8196, int receiveTimeout = 5000, int sendBufferSize = 1400, int sendTimeout = 5000, bool listenIPv6 = false)
        {
#if DEBUG
            //检测参数是否错误
            if (receiveBufferSize > CxMsgPacket.MaxLength || sendBufferSize > CxMsgPacket.MaxLength)
            {
                if (receiveBufferSize > CxMsgPacket.MaxLength)
                {
                    CxDebug.WriteLine(CxString.Format("接收缓冲区大于了消息包最大长度,造成了内存浪费 CurSize:{0} CxMsgPacket.MaxLength:{1}", receiveBufferSize, CxMsgPacket.MaxLength));
                }
                if (sendBufferSize > CxMsgPacket.MaxLength)
                {
                    CxDebug.WriteLine(CxString.Format("发送缓冲区大于了消息包最大长度,造成了内存浪费 CurSize:{0} CxMsgPacket.MaxLength:{1}", receiveBufferSize, CxMsgPacket.MaxLength));
                }
            }
#endif
            base.SetNetConfig(receiveBufferSize, receiveTimeout, sendBufferSize, sendTimeout, listenIPv6);
        }
        /// <summary>
        /// 当收到消息包
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="data"></param>
        private void OnReceiveMsgPacket(EndPoint endPoint, byte[] data)
        {
            BeforeUnPacket(endPoint, data, CxMsgPacket.LengthSize, data.Length - CxMsgPacket.LengthSize);
            CxMsgPacket msgPacket = new CxMsgPacket(data) { ReadPos = CxMsgPacket.LengthSize };//从CxMsgPacket.LengthSize开始读取
            OnReceiveMsgPacket(endPoint, msgPacket);
        }
        /// <summary>
        /// 当收到数据 如非必要 不要重写该函数
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        protected override void OnReceiveData(EndPoint endPoint, byte[] data, int length)
        {
            //预留字节CxMsgPacket.FreeNum包含(CxMsgPacket.LengthSize字节长度 CxMsgPacket.FlagSize字节标志)CxMsgPacket.CmdSize字节指令
            int packageLength = 0;
            CxMsgPacket msgPacket = new CxMsgPacket(data);
            if (length >= CxMsgPacket.LengthSize)//需要CxMsgPacket.LengthSize字节才可解出包长
            {
                //获取包长
                packageLength = msgPacket.PopMsgPackageLength();
                while (packageLength >= CxMsgPacket.FreeNum - CxMsgPacket.LengthSize &&//最小包长度 至少需(CxMsgPacket.FreeNum - CxMsgPacket.LengthSize)字节
                        packageLength <= _netConfig.ReceiveBufferSize &&//小于接收缓冲区大小
                        packageLength <= (length - msgPacket.ReadPos))//小于剩余数据长度,否则可能断包,需要粘包处理
                {
                    OnReceiveMsgPacket(endPoint, msgPacket.GetBytesAt(msgPacket.ReadPos - CxMsgPacket.LengthSize, packageLength + CxMsgPacket.LengthSize));
                    msgPacket.ReadPos += packageLength;
                    if (msgPacket.ReadPos + CxMsgPacket.LengthSize <= length)//还能解出一个包头
                    {
                        packageLength = msgPacket.PopMsgPackageLength();
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
                OnError(endPoint, CxString.Format("Length:{0} - 数据长度错误", packageLength));
            }
        }
        /// <summary>
        /// 发送消息包
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="msgPacket"></param>
        public EndPoint SendMsgPacket(string host, ushort port, CxMsgPacket msgPacket)
        {
            var hostPortTuple = new Tuple<string, ushort>(host, port);
            if (!_hostInfoMap.TryGetValue(hostPortTuple, out EndPoint endPoint))
            {
                endPoint = CxNetTool.GetEndPoint(host, port);
                _hostInfoMap.Add(hostPortTuple, endPoint);
            }
            SendMsgPacket(endPoint, msgPacket);
            return endPoint;
        }
        /// <summary>
        /// 发送消息包
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="msgPacket"></param>
        public void SendMsgPacket(EndPoint endPoint, CxMsgPacket msgPacket)
        {
            if (msgPacket.Length > CxMsgPacket.MaxLength)
            {
                throw new ArgumentOutOfRangeException("msgPacket", CxString.Format("发送数据过长 超过了CxMsgPacket.MaxLength设置的大小 msgPacket.Length:{0} CxMsgPacket.MaxLength:{1}", msgPacket.Length, _netConfig.SendBufferSize));
            }
            if (msgPacket.Length > _netConfig.SendBufferSize)
            {
                throw new ArgumentOutOfRangeException("msgPacket", CxString.Format("发送数据过长 超过了SendBufferSize设置的大小 msgPacket.Length:{0} SendBufferSize:{1}", msgPacket.Length, _netConfig.SendBufferSize));
            }
            msgPacket.Package();
            BeforeSend(endPoint, msgPacket.Data, CxMsgPacket.LengthSize, msgPacket.Length - CxMsgPacket.LengthSize);
            Send(endPoint, msgPacket.Data, 0, msgPacket.Length);
        }
        #region Virtual Method
        /// <summary>
        /// 当发生错误时
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="error"></param>
        protected virtual void OnError(EndPoint endPoint, string error)
        {
        }
        /// <summary>
        /// 在发送之前 可以进行加密操作 (只可从index开始)
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        protected virtual void BeforeSend(EndPoint endPoint, byte[] data, int index, int length)
        {

        }
        /// <summary>
        /// 在解包之前 可以进行解密之类的操作 (只可从index开始)
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        protected virtual void BeforeUnPacket(EndPoint endPoint, byte[] data, int index, int length)
        {

        }
        /// <summary>
        /// 当收到消息包
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="msgPacket"></param>
        protected virtual void OnReceiveMsgPacket(EndPoint endPoint, CxMsgPacket msgPacket)
        {

        }
        #endregion
    }
}

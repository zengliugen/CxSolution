using System;

using CxSolution.CxRouRou.Expand;
using CxSolution.CxRouRou.Util;

namespace CxSolution.CxRouRou.Net.Sockets.Tcp
{
    /// <summary>
    /// 网络
    /// </summary>
    public class CxNet : CxNetBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public CxNet()
        {
            CxDebug.WriteLine("当前最大支持消息包长度为:{0}", CxMsgPacket.MaxLength);
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
            if (receiveBufferSize > CxMsgPacket.MaxLength || sendBufferSize > CxMsgPacket.MaxLength)
            {
                if (receiveBufferSize > CxMsgPacket.MaxLength)
                {
                    CxDebug.WriteLine(CxString.Format("接收缓冲区大于了消息包最大长度，造成了内存浪费 CurSize:{0} CxMsgPacket.MaxLength:{1}", receiveBufferSize, CxMsgPacket.MaxLength));
                }
                if (sendBufferSize > CxMsgPacket.MaxLength)
                {
                    CxDebug.WriteLine(CxString.Format("发送缓冲区大于了消息包最大长度，造成了内存浪费 CurSize:{0} CxMsgPacket.MaxLength:{1}", receiveBufferSize, CxMsgPacket.MaxLength));
                }
            }
#endif
            base.SetNetConfig(poolSize, receiveBufferSize, receiveTimeout, sendBufferSize, sendTimeout, listenIPv6);
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
            OnReceiveMsgPacket(id, msgPacket);
        }
        /// <summary>
        /// 当收到数据 如非必要 不要重写该函数
        /// </summary>
        /// <param name="receiveData"></param>
        /// <param name="length"></param>
        protected override void OnReceiveData(IReceiveData receiveData, int length)
        {
            //预留字节CxMsgPacket.FreeNum包含(CxMsgPacket.LengthSize字节长度 CxMsgPacket.FlagSize字节标志)CxMsgPacket.CmdSize字节指令
            int packageLength = 0;
            CxMsgPacket msgPacket = new CxMsgPacket(receiveData.Buffer);
            if (length >= CxMsgPacket.LengthSize)//需要CxMsgPacket.LengthSize字节才可解出包长
            {
                //获取包长
                packageLength = msgPacket.PopMsgPackageLength();
                while (packageLength >= CxMsgPacket.FreeNum - CxMsgPacket.LengthSize &&//最小包长度 至少需(CxMsgPacket.FreeNum - CxMsgPacket.LengthSize)字节
                        packageLength <= _netConfig.ReceiveBufferSize &&//小于接收缓冲区大小
                        packageLength <= (length - msgPacket.ReadPos))//小于剩余数据长度，否则可能断包，需要粘包处理
                {
                    OnReceiveMsgPacket(receiveData.ID, msgPacket.GetBytesAt(msgPacket.ReadPos - CxMsgPacket.LengthSize, packageLength + CxMsgPacket.LengthSize));
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
                receiveData.Offset = 0;
                OnError(receiveData.ID, CxString.Format("Length:{0} - 数据长度错误", packageLength));
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
        /// <param name="msgPacket"></param>
        /// <param name="useAsync"></param>
        public void SendMsgPacket(uint id, CxMsgPacket msgPacket, bool useAsync = true)
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
            BeforeSend(id, msgPacket.Data, CxMsgPacket.LengthSize, msgPacket.Length - CxMsgPacket.LengthSize);
            Send(id, msgPacket.Data, 0, msgPacket.Length, useAsync);
        }
        #region Virtual Method
        /// <summary>
        /// 当发生错误时
        /// </summary>
        /// <param name="id"></param>
        /// <param name="error"></param>
        protected virtual void OnError(uint id, string error)
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
        /// 当收到消息包
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msgPacket"></param>
        protected virtual void OnReceiveMsgPacket(uint id, CxMsgPacket msgPacket)
        {

        }
        #endregion
    }
}

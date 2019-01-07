//是否使用uint作为长度
#define CX_MSGPACKET_USE_UINT_LENGTH
#undef CX_MSGPACKET_USE_UINT_LENGTH
using CxRouRou.Collections;
using System;
using System.Collections.Generic;
using System.Text;
namespace CxRouRou.Net.Sockets
{
    /// <summary>
    /// 消息包
    /// </summary>
    public sealed class CxMsgPacket : CxByteBuffer
    {
        /// <summary>
        /// 长度大小
        /// </summary>
#if CX_MSGPACKET_USE_UINT_LENGTH
        public const int LengthSize = sizeof(uint);
#else
        public const int LengthSize = sizeof(ushort);
#endif
        /// <summary>
        /// 标志大小
        /// </summary>
        public const int FlagSize = sizeof(ushort);
        /// <summary>
        /// 指令大小
        /// </summary>
        public const int CmdSize = sizeof(ushort);
        /// <summary>
        /// 预留字节 LengthSize字节长度 FlagSize字节标志 CmdSize字节指令
        /// </summary>
        public const int FreeNum = LengthSize + FlagSize + CmdSize;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="capacity"></param>
        public CxMsgPacket(int capacity) : base(capacity)
        {
            WritePos = FreeNum;
        }
        /// <summary>
        /// 以指定数据创建，不复制源数据
        /// </summary>
        /// <param name="data"></param>
        public CxMsgPacket(byte[] data) : base(data)
        {
        }
        /// <summary>
        /// 清空 重置读取写入偏移 保留预留字节
        /// </summary>
        /// <param name="resetData"></param>
        public override void Clear(bool resetData = false)
        {
            base.Clear(resetData);
            WritePos = FreeNum;
        }
        /// <summary>
        /// 读取偏移
        /// </summary>
        internal new int ReadPos
        {
            get
            {
                return base.ReadPos;
            }
            set
            {
                base.ReadPos = value;
            }
        }
        /// <summary>
        /// 数据集合 不可使用Data.Length获取长度 应使用Length
        /// </summary>
        internal new byte[] Data
        {
            get
            {
                return base.Data;
            }
        }
        /// <summary>
        /// 包装
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="cmd"></param>
        internal void Package(ushort flag, ushort cmd)
        {
            int length = Length;
            WritePos = 0;
            //写入长度(不包长度自身所占字节)
#if CX_MSGPACKET_USE_UINT_LENGTH
            Push_uint(length - LengthSize);
#else
            Push_ushort(length - LengthSize);
#endif
            //写入标记
            Push_ushort(flag);
            //写入指令
            Push_ushort(cmd);
            WritePos = length;
        }
    }
}

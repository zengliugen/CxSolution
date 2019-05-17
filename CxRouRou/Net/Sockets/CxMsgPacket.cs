//MSG_PACKAGE_SUPPORT_INT_MAX_LENGTH
//支持int做为最大长度

using CxRouRou.Collections;
namespace CxRouRou.Net.Sockets
{
    /// <summary>
    /// 消息包
    /// 默认最大包长度 ushort.MaxValue 65535
    /// 默认最大数据长度(最大包长减去sizeof(ushort)) 65533
    /// 支持使用int 需调用 SupportIntMaxLength
    /// 支持最大包长度 int.MaxValue 2147483647
    /// 支持最大数据长度(最大包长减去sizeof(int)) 2147483643
    /// </summary>
    public sealed class CxMsgPacket : CxByteBuffer
    {
        /// <summary>
        /// 是否使用int作为最大长度
        /// </summary>
        internal static bool UseIntMaxLength = false;
        /// <summary>
        /// 最大长度
        /// </summary>
        internal static int MaxLength = ushort.MaxValue;
        /// <summary>
        /// 长度大小
        /// </summary>
        internal static int LengthSize = sizeof(ushort);
        /// <summary>
        /// 标志大小
        /// </summary>
        internal const int FlagSize = sizeof(ushort);
        /// <summary>
        /// 指令大小
        /// </summary>
        internal const int CmdSize = sizeof(ushort);
        /// <summary>
        /// 预留字节 LengthSize字节长度 FlagSize字节标志 CmdSize字节指令
        /// </summary>
        internal static int FreeNum = LengthSize + FlagSize + CmdSize;
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
            PushMsgPackageLength(length - LengthSize);
            //写入标记
            Push_ushort(flag);
            //写入指令
            Push_ushort(cmd);
            WritePos = length;
        }
        /// <summary>
        /// 写入消息包长度
        /// </summary>
        /// <param name="length"></param>
        internal void PushMsgPackageLength(int length)
        {
#if MSG_PACKAGE_SUPPORT_INT_MAX_LENGTH
            if (UseIntMaxLength)
            {
                Push_int(length);
            }
            else
            {
                Push_ushort(length);
            }
#else
            Push_ushort(length);
#endif
        }
        /// <summary>
        /// 读取消息包长度
        /// </summary>
        /// <returns></returns>
        internal int PopMsgPackageLength()
        {
#if MSG_PACKAGE_SUPPORT_INT_MAX_LENGTH
            if (UseIntMaxLength)
            {
                return Pop_int();
            }
            else
            {
                return Pop_ushort();
            }
#else
            return Pop_ushort();
#endif
        }
#if MSG_PACKAGE_SUPPORT_INT_MAX_LENGTH
        /// <summary>
        /// 支持int作为最大长度
        /// </summary>
        public static void SupportIntMaxLength()
        {
            MaxLength = int.MaxValue;
            LengthSize = sizeof(int);
            FreeNum = LengthSize + FlagSize + CmdSize;
            UseIntMaxLength = true;
        }
#endif
        /// <summary>
        /// 获取预留字节 LengthSize字节长度 FlagSize字节标志 CmdSize字节指令
        /// </summary>
        /// <returns></returns>
        public static int GetFreeNum()
        {
            return FreeNum;
        }
    }
}

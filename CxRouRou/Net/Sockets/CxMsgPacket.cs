using CxSolution.CxRouRou.Collections;

namespace CxSolution.CxRouRou.Net.Sockets
{
    /// <summary>
    /// 消息包
    /// 默认最大包长度 ushort.MaxValue 65535
    /// 默认最大数据长度(最大包长减去sizeof(ushort)) 65533
    /// 支持使用int 需调用 SupportIntMaxLength
    /// 支持最大包长度 int.MaxValue 2147483647
    /// 支持最大数据长度(最大包长减去sizeof(int)) 2147483643
    /// </summary>
    public class CxMsgPacket : CxByteBuffer
    {
        /// <summary>
        /// 是否使用int作为最大长度
        /// </summary>
        private static bool _useIntMaxLength = false;
        /// <summary>
        /// 最大长度
        /// </summary>
        internal static int MaxLength = ushort.MaxValue;
        /// <summary>
        /// 长度大小
        /// </summary>
        internal static int LengthSize = sizeof(ushort);
        /// <summary>
        /// 预留字节 LengthSize字节长度
        /// </summary>
        internal static int FreeNum = LengthSize;
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
        protected internal virtual void Package()
        {
            var length = Length;
            WritePos = 0;
            //写入长度(不包长度自身所占字节)
            PushMsgPackageLength(length - LengthSize);
            WritePos = length;
        }
        /// <summary>
        /// 写入消息包长度
        /// </summary>
        /// <param name="length"></param>
        internal void PushMsgPackageLength(int length)
        {
            if (_useIntMaxLength)
            {
                Push_int(length);
            }
            else
            {
                Push_ushort(length);
            }
        }
        /// <summary>
        /// 读取消息包长度
        /// </summary>
        /// <returns></returns>
        internal int PopMsgPackageLength()
        {
            if (_useIntMaxLength)
            {
                return Pop_int();
            }
            else
            {
                return Pop_ushort();
            }
        }
        /// <summary>
        /// 支持int作为最大长度
        /// </summary>
        public static void SupportIntMaxLength()
        {
            _useIntMaxLength = true;
            MaxLength = int.MaxValue;
            LengthSize = sizeof(int);
            FreeNum = LengthSize;
        }
    }
}

using System;

namespace CxSolution.CxRouRou.Attributes
{
    /// <summary>
    /// ByteBuffer自动压入弹出属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AutoByteBufferAttribute : Attribute
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        public bool Handle { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handle"></param>
        public AutoByteBufferAttribute(bool handle)
        {
            Handle = handle;
        }
    }
}

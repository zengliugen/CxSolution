using System;
using System.Collections.Generic;
using System.Text;

namespace CxRouRou.Attributes
{
    /// <summary>
    /// ByteBuffer自动压入弹出属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ByteBufferAttribute : Attribute
    {
        /// <summary>
        /// 是否处理
        /// </summary>
        public bool Handle { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handle"></param>
        public ByteBufferAttribute(bool handle)
        {
            Handle = handle;
        }
    }
}

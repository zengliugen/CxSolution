using System;
using System.Collections.Generic;
using System.Text;

namespace CxRouRou.Net.Sockets.Tcp
{
    /// <summary>
    /// 网络配置
    /// </summary>
    public sealed class CxNetConfig
    {
        /// <summary>
        /// 会话对象池大小 默认1000
        /// </summary>
        public int PoolSize { get; set; }
        /// <summary>
        /// 接收数据缓冲区大小 默认8196
        /// </summary>
        public ushort ReceiveBufferSize { get; set; }
        /// <summary>
        /// 接收数据超时间 单位毫秒 默认20秒
        /// </summary>
        public int ReceiveTimeout { get; set; }
        /// <summary>
        /// 发送数据缓冲区大小 默认1400 (以太网MTU一般为 1500) 防止数据太大导致MTU分片
        /// </summary>
        public ushort SendBufferSize { get; set; }
        /// <summary>
        /// 接收数据超时间 单位毫秒 默认20秒
        /// </summary>
        public int SendTimeout { get; set; }
        /// <summary>
        /// 是否侦听IPv6网络地址 默认false
        /// </summary>
        public bool ListenIPv6 { get; set; }
    }
}

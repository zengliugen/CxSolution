﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Net.Sockets.Udp
{
    /// <summary>
    /// 网络配置
    /// </summary>
    public sealed class CxNetConfig
    {
        /// <summary>
        /// 接收数据缓冲区大小 默认 8196
        /// </summary>
        public int ReceiveBufferSize { get; set; }
        /// <summary>
        /// 接收数据超时时间 单位毫秒 默认5秒
        /// </summary>
        public int ReceiveTimeout { get; set; }
        /// <summary>
        /// 发送数据缓冲区大小 默认1400 (以太网MTU一般为 1500) 防止数据太大导致MTU分片(如需发送大数据,建议自主拆包发送)
        /// </summary>
        public int SendBufferSize { get; set; }
        /// <summary>
        /// 发送数据超时时间 单位毫秒 默认5秒
        /// </summary>
        public int SendTimeout { get; set; }
        /// <summary>
        /// 是否侦听IPv6网络地址 默认false
        /// </summary>
        public bool ListenIPv6 { get; set; }
    }
}

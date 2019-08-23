using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Net.Sockets.Tcp
{
    /// <summary>
    /// 收到数据结构
    /// </summary>
    public interface IReceiveData
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        uint ID { get; }
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        byte[] Buffer { get; }
        /// <summary>
        /// 数据缓冲区接收偏移
        /// </summary>
        int Offset { set; }
    }
}

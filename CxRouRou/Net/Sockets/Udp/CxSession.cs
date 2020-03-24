using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CxSolution.CxRouRou.Net.Sockets.Udp
{
    /// <summary>
    /// 会话
    /// </summary>
    public class CxSession
    {
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        internal byte[] Buffer;
        /// <summary>
        /// 远程地址
        /// </summary>
        internal EndPoint RemoteAddress;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bufferSize"></param>
        public CxSession(int bufferSize)
        {
            Buffer = new byte[bufferSize];
        }
    }
}

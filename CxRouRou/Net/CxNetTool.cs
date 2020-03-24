using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace CxSolution.CxRouRou.Net
{
    /// <summary>
    /// 网络工具
    /// </summary>
    public static class CxNetTool
    {
        /// <summary>
        /// IP检测正则表达式
        /// </summary>
        private static Regex IPRegex = new Regex(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        /// <summary>
        /// 检测字符串是否为IPV4
        /// </summary>
        /// <param name="ipv4"></param>
        /// <returns></returns>
        public static bool IsIPV4(string ipv4)
        {
            return IPRegex.IsMatch(ipv4);
        }
        /// <summary>
        /// 获取指定域名或者ip字符串的IP地址信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="comparer">排序方法</param>
        /// <returns></returns>
        public static IPAddress[] GetIPAddress(string host, ushort port, IComparer<IPAddress> comparer = null)
        {
            var iPAddressList = new List<IPAddress>();
            if (IPAddress.TryParse(host, out IPAddress iPAddress))
            {
                iPAddressList.Add(iPAddress);
            }
            else
            {
                var hostEntry = Dns.GetHostEntry(host);
                if (hostEntry != null)
                {
                    iPAddressList.AddRange(hostEntry.AddressList);
                }
            }
            if (comparer != null)
            {
                iPAddressList.Sort(comparer);
            }
            return iPAddressList.ToArray();
        }
        /// <summary>
        /// 终端地址(获取指定IP的第一个地址)
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IPEndPoint GetEndPoint(string host, ushort port, IComparer<IPAddress> comparer = null)
        {
            IPEndPoint ipEndPoint = null;
            var ipAddressArray = GetIPAddress(host, port, comparer);
            if (ipAddressArray != null && ipAddressArray.Length > 0)
            {
                var ipAddress = ipAddressArray[0];
                ipEndPoint = new IPEndPoint(ipAddress, port);
            }
            return ipEndPoint;
        }
    }
}

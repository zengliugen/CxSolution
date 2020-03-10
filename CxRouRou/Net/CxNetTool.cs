using System;
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
    }
}

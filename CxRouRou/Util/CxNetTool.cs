using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CxRouRou.Util
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
        /// 检测字符串是否为IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return IPRegex.IsMatch(ip);
        }
    }
}

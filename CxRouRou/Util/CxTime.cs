using System;

namespace CxRouRou.Util
{
    /// <summary>
    /// 时间操作
    /// </summary>
    public static class CxTime
    {
        /// <summary>
        /// 格式化日期时间
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetDateTime(string format = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.Now.ToString(format);
        }
    }
}

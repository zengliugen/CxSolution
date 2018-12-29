using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace CxRouRou.Util
{
    /// <summary>
    /// 调试日志
    /// </summary>
    public static class CxDebug
    {
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="value"></param>
        [Conditional("DEBUG")]
        public static void Write(object value)
        {
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        [Conditional("DEBUG")]
        public static void Write(string format, params object[] arg)
        {
            string value = CxString.Format(format, arg);
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="value"></param>
        [Conditional("DEBUG")]
        public static void WriteLine(object value)
        {
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        [Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] arg)
        {
            string value = CxString.Format(format, arg);
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace CxRouRou.Util
{
    /// <summary>
    /// 跟踪日志
    /// </summary>
    public class CxTrace
    {
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="value"></param>
        [Conditional("TRACE")]
        public static void Write(object value)
        {
            Trace.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        [Conditional("TRACE")]
        public static void Write(string format, params object[] arg)
        {
            string value = CxString.Format(format, arg);
            Trace.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="value"></param>
        [Conditional("TRACE")]
        public static void WriteLine(object value)
        {
            Trace.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        [Conditional("TRACE")]
        public static void WriteLine(string format, params object[] arg)
        {
            string value = CxString.Format(format, arg);
            Trace.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
    }
}

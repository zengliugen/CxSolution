using System.Diagnostics;

using CxSolution.CxRouRou.Expands;

namespace CxSolution.CxRouRou.Util
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
        /// <param name="args"></param>
        [Conditional("TRACE")]
        public static void Write(string format, params object[] args)
        {
            string value = CxString.Format(format, args);
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
        /// <param name="args"></param>
        [Conditional("TRACE")]
        public static void WriteLine(string format, params object[] args)
        {
            string value = CxString.Format(format, args);
            Trace.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
    }
}

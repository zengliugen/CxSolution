using System.Diagnostics;

using CxSolution.CxRouRou.Expand;

namespace CxSolution.CxRouRou.Util
{
    /// <summary>
    /// 调试操作
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
        /// <param name="value"></param>
        /// <param name="category"></param>
        [Conditional("DEBUG")]
        public static void Write(object value, string category)
        {
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value), category);
        }
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void Write(string format, params object[] args)
        {
            string value = CxString.Format(format, args);
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="format"></param>
        /// <param name="category"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void Write(string format, string category, params object[] args)
        {
            string value = CxString.Format(format, args);
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value), category);
        }
        /// <summary>
        /// 如果输出
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        [Conditional("DEBUG")]
        public static void WriteIf(bool condition, object value)
        {
            if (!condition) return;
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 如果输出
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        /// <param name="category"></param>
        [Conditional("DEBUG")]
        public static void WriteIf(bool condition, object value, string category)
        {
            if (!condition) return;
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value), category);
        }
        /// <summary>
        /// 如果输出
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteIf(bool condition, string format, params object[] args)
        {
            if (!condition) return;
            string value = CxString.Format(format, args);
            Debug.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 如果输出
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="format"></param>
        /// <param name="category"></param>
        /// <param name="args"></param>
        public static void WriteIf(bool condition, string format, string category, params object[] args)
        {
            if (!condition) return;
            string value = CxString.Format(format, args);
            Debug.Write(category, CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
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
        /// <param name="value"></param>
        /// <param name="category"></param>
        [Conditional("DEBUG")]
        public static void WriteLine(object value, string category)
        {
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value), category);
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] args)
        {
            string value = CxString.Format(format, args);
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="format"></param>
        /// <param name="category"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLine(string format, string category, params object[] args)
        {
            string value = CxString.Format(format, args);
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value), category);
        }
        /// <summary>
        /// 如果输出行
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        [Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, object value)
        {
            if (!condition) return;
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        /// <param name="category"></param>
        [Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, object value, string category)
        {
            if (!condition) return;
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value), category);
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string format, params object[] args)
        {
            if (!condition) return;
            string value = CxString.Format(format, args);
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="format"></param>
        /// <param name="category"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string format, string category, params object[] args)
        {
            if (!condition) return;
            string value = CxString.Format(format, args);
            Debug.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value), category);
        }
    }
}

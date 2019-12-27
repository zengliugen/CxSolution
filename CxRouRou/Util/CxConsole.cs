using System;
using System.Collections.Generic;
using System.Text;
using CxSolution.CxRouRou.Collections;

namespace CxSolution.CxRouRou.Util
{
    /// <summary>
    /// 控制台操作
    /// </summary>
    public static class CxConsole
    {
        /// <summary>
        /// 是否输出时间
        /// </summary>
        public static bool WriteTime = true;
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="value"></param>
        public static void Write(object value)
        {
            if (WriteTime)
            {
                Console.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
            }
            else
            {
                Console.Write(value);
            }
        }
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string format, params object[] args)
        {
            string value = CxString.Format(format, args);
            if (WriteTime)
            {
                Console.Write(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
            }
            else
            {
                Console.Write(value);

            }
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLine(object value)
        {
            if (WriteTime)
            {
                Console.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
            }
            else
            {
                Console.WriteLine(value);
            }
        }
        /// <summary>
        /// 输出行
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WriteLine(string format, params object[] args)
        {
            string value = CxString.Format(format, args); if (WriteTime)
            {
                Console.WriteLine(CxString.Format("[{0}] {1}", CxTime.GetDateTime(), value));
            }
            else
            {
                Console.WriteLine(value);
            }
        }
    }
}

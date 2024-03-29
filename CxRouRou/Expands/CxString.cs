﻿namespace CxSolution.CxRouRou.Expands
{
    /// <summary>
    /// 字符串操作
    /// </summary>
    public static class CxString
    {
        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                return format;
            }
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                //忽略
            }
            return format;
        }
        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatSelf(this string source, params object[] args)
        {
            return Format(source, args);
        }
    }
}

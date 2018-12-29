using System;
using System.Collections.Generic;
using System.Text;

namespace CxRouRou.Util
{
    /// <summary>
    /// 字符串操作
    /// </summary>
    public static class CxString
    {
        public static string Format(string format, params object[] arg)
        {
            if (arg.Length == 0)
            {
                return format;
            }
            try
            {
                return string.Format(format, arg);
            }
            catch
            {
                //忽略
            }
            return format;
        }
    }
}

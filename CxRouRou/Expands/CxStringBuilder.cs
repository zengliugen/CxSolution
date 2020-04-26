using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Expands
{
    /// <summary>
    /// 字符串生成
    /// </summary>
    public static class CxStringBuilder
    {
        /// <summary>
        /// 添加格式化行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        public static void AppendFormatLine(this StringBuilder source, string format, object arg0)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            source.AppendFormat(format, arg0);
            source.AppendLine();
        }
        /// <summary>
        /// 添加格式化行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        public static void AppendFormatLine(this StringBuilder source, string format, object arg0, object arg1)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            source.AppendFormat(format, arg0, arg1);
            source.AppendLine();
        }
        /// <summary>
        /// 添加格式化行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public static void AppendFormatLine(this StringBuilder source, string format, object arg0, object arg1, object arg2)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            source.AppendFormat(format, arg0, arg2);
            source.AppendLine();
        }
        /// <summary>
        /// 添加格式化行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void AppendFormatLine(this StringBuilder source, string format, params object[] args)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            source.AppendFormat(format, args);
            source.AppendLine();
        }
    }
}

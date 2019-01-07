using System;
using System.Collections.Generic;
using System.Text;

namespace CxRouRou.Collections
{
    /// <summary>
    /// IList操作
    /// </summary>
    public static class CxIList
    {
        /// <summary>
        /// 比较IList是否相同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualsEx<T>(this IList<T> source, IList<T> target)
        {
            if (source == null && target == null)
            {
                return true;
            }
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (target == null)
            {
                return false;
            }
            if (source.Count != target.Count)
            {
                return false;
            }
            for (int i = 0; i < source.Count; i++)
            {
                if (!source[i].Equals(target[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// IList转字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToStringEx<T>(this IList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            int count = source.Count;
            for (int i = 0; i < count; i++)
            {
                sb.Append(source[i]);
                if (i < count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}

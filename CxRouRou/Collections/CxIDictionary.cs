using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Collections
{
    /// <summary>
    /// IDictionary操作
    /// </summary>
    public static class CxIDictionary
    {
        /// <summary>
        /// 比较IDictionary是否相同
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualsEx<T1, T2>(this IDictionary<T1, T2> source, IDictionary<T1, T2> target)
        {
            if (source==null && target == null)
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
            foreach (var item in source)
            {
                if (!item.Value.Equals(target[item.Key]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// IDictionary转字符串
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToStringEx<T1, T2>(this IDictionary<T1, T2> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            int index = 0;
            foreach (var item in source)
            {
                sb.Append("{");
                sb.Append(item.Key.ToString());
                sb.Append(",");
                sb.Append(item.Value.ToString());
                sb.Append("}");
                if (index != source.Count)
                {
                    sb.Append(",");
                }
                index++;
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}

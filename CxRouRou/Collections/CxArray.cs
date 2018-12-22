using System;
using System.Collections.Generic;
using System.Text;

namespace CxRouRou.Collections
{
    public static class CxArray
    {
        /// <summary>
        /// 获取数组的一部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] GetArray<T>(this T[] source, int index, int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (index < 0 || index + length > source.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            T[] ts = new T[length];
            Array.Copy(source, index, ts, 0, length);
            return ts;
        }
        /// <summary>
        /// 数组转字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToStringEx<T>(this T[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            int length = source.Length;
            for (int i = 0; i < length; i++)
            {
                sb.Append(source[i]);
                if (i < length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
        /// <summary>
        /// 比较数组是否相同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualsEx<T>(this T[] source, T[] target)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (target == null)
            {
                return false;
            }
            if (source.Length != target.Length)
            {
                return false;
            }
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(target[i]) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

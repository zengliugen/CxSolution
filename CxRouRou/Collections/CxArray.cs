using System;
using System.Collections.Generic;
using System.Text;

namespace CxRouRou.Collections
{
    /// <summary>
    /// 数组操作
    /// </summary>
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
        /// 获取数组的一部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] GetArray<T>(this T[] source, long index, long length)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            if (index < 0 || index + length > source.LongLength)
            {
                throw new ArgumentOutOfRangeException();
            }
            T[] ts = new T[length];
            Array.Copy(source, index, ts, 0, length);
            return ts;
        }
        /// <summary>
        /// 创建随机byte数组
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] CreateRandomByte(int length)
        {
            byte[] bytes = new byte[length];
            new Random().NextBytes(bytes);
            return bytes;
        }
    }
}

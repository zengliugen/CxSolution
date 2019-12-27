using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CxSolution.CxRouRou.Collections
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
            sb.Append("[");
            int count = source.Length;
            for (int i = 0; i < count; i++)
            {
                sb.Append(source[i]);
                if (i < count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
        /// <summary>
        /// byte数组转换为16进制字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="connector">连接符号</param>
        /// <returns></returns>
        public static string ToHexString(this byte[] source, char connector = ' ')
        {
            return source.ToHexString(0, source.Length, connector);
        }
        /// <summary>
        /// byte数组转换为16进制字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="connector">连接符号</param>
        /// <returns></returns>
        public static string ToHexString(this byte[] source, int startIndex, char connector = ' ')
        {
            return source.ToHexString(startIndex, source.Length - startIndex, connector);
        }
        /// <summary>
        /// byte数组转换为16进制字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="connector">连接符号</param>
        /// <returns></returns>
        public static string ToHexString(this byte[] source, int startIndex, int length, char connector = ' ')
        {
            return BitConverter.ToString(source, startIndex, length).Replace('-', connector);
        }
        /// <summary>
        /// 16进制字符串转byte数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] GetBytesByHexString(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentException("hexString");
            }
            hexString = Regex.Replace(hexString, @"[^\dA-F]", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (hexString.Length == 0)
            {
                return null;
            }
            List<byte> listBytes = new List<byte>(hexString.Length / 2);
            for (int i = 0; i < hexString.Length; i += 2)
            {
                var s = hexString.Substring(i, 2);
                listBytes.Add(Convert.ToByte(hexString.Substring(i, 2), 16));
            }
            return listBytes.ToArray();
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

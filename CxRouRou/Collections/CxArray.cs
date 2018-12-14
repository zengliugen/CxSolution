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
        /// <param name="colletion"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] GetArray<T>(this T[] colletion, int index, int length)
        {
            if (colletion == null)
            {
                throw new ArgumentNullException();
            }
            if (index < 0 || index + length > colletion.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            T[] ts = new T[length];
            Array.Copy(colletion, index, ts, 0, length);
            return ts;
        }
    }
}

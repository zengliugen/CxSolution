using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Exceptions;

using Number = System.UInt64;
using BigNumber = System.Numerics.BigInteger;

namespace CxSolution.CxRouRou.Maths
{
    /// <summary>
    /// 进制转换工具类
    /// </summary>
    public static class CxBaseConversion
    {
        /// <summary>
        /// Number数据支持的最大二进制位长度
        /// </summary>
        private static readonly int NumberBitLength = (int)Math.Ceiling(Math.Log(Number.MaxValue, 2));
        /// <summary>
        /// 二进制数组转换为Number
        /// </summary>
        /// <param name="binarys">二进制数组</param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Number BinarysToNumber(Number[] binarys, int index = 0, int length = 0)
        {
            if (binarys == null)
                throw new ArgumentNullException(nameof(binarys));
            if (binarys.Length > NumberBitLength)
                throw new Exception("二进制数组:{0} 对应的值大于了Type:{1}类型所能表示的最大值:{2}".FormatSelf(binarys.Concat(), typeof(Number).GetType().Name, Number.MaxValue));
            if (index < 0 || length < 0 || index + length > binarys.Length)
                throw new ArgumentOutOfRangeException();
            if (index == 0)
                length = binarys.Length;
            Number value = 0;
            Number pow = 1;
            for (int i = index + length - 1; i >= index; i--)
            {
                value += pow * binarys[i];
                pow <<= 1;
            }
            return value;
        }
        /// <summary>
        /// Number转换为二进制数组
        /// </summary>
        /// <param name="other"></param>
        /// <param name="binaryBit">需要提取的二进制位数(从低位开始提取)</param>
        /// <returns></returns>
        public static Number[] NumberToBinarys(Number other, int binaryBit = 0)
        {
            var values = new List<Number>();
            Number pow = 1;
            pow <<= NumberBitLength - 1;
            while (pow > 0)
            {
                values.Add(other / pow);
                other = other % pow;
                pow >>= 1; ;
            }
            if (binaryBit == 0)
                binaryBit = values.Count;
            return values.GetRange(values.Count - binaryBit, binaryBit).ToArray();
        }
        /// <summary>
        /// 二进制转换其他进制(2的冥次方进制),返回其他就进制数组,按照高位到低位排列
        /// </summary>
        /// <param name="binarys"></param>
        /// <param name="otherBase"></param>
        /// <returns></returns>
        public static Number[] BinaryToOther2Power(Number[] binarys, int otherBase)
        {
            if (binarys == null)
                throw new ArgumentNullException(nameof(binarys));
            if (otherBase <= 1)
                throw new ArgumentException("参数必须大于等于2", nameof(otherBase));
            if ((otherBase & (otherBase - 1)) != 0)
                throw new Exception("param:{0}不是2的冥次方,请使用BinaryToOther接口进行转换".FormatSelf(nameof(otherBase)));
            //获取其他进制对应的二进制位长度
            var otherBaseBitLength = (int)Math.Ceiling(Math.Log(otherBase, 2));
            //获取二进制数组进行对齐长度
            var remainder = binarys.Length % otherBaseBitLength;
            var binaryLength = remainder == 0 ? binarys.Length : binarys.Length + otherBaseBitLength - remainder;
            //对二进制数组进行对齐操作
            var array = new Number[binaryLength];
            Array.Copy(binarys, 0, array, binaryLength - binarys.Length, binarys.Length);
            binarys = array;
            //保存结果数组
            var otherNumbers = new Number[binaryLength / otherBaseBitLength];
            for (int i = 0; i < binarys.Length; i += otherBaseBitLength)
            {
                otherNumbers[i / otherBaseBitLength] = BinarysToNumber(binarys, i, otherBaseBitLength);
            }
            return otherNumbers;
        }
        /// <summary>
        /// 其他进制(2的冥次方进制)转换二进制
        /// </summary>
        /// <param name="others"></param>
        /// <param name="otherBase"></param>
        /// <returns></returns>
        public static Number[] Other2PowerToBinary(Number[] others, int otherBase)
        {
            if (others == null)
                throw new ArgumentNullException(nameof(others));
            if (otherBase <= 1)
                throw new ArgumentException("参数必须大于等于2", nameof(otherBase));
            if ((otherBase & (otherBase - 1)) != 0)
                throw new Exception("param:{0}不是2的冥次方,请使用BinaryToOther接口进行转换".FormatSelf(nameof(otherBase)));
            //获取其他进制对应的二进制位长度
            var otherBaseBitLength = (int)Math.Ceiling(Math.Log(otherBase, 2));
            //保存结果数组
            var otherNumbers = new Number[others.Length * otherBaseBitLength];
            for (int i = 0; i < others.Length; i++)
            {
                var bs = NumberToBinarys(others[i], otherBase);
                Array.Copy(bs, bs.Length - otherBaseBitLength, otherNumbers, i * otherBaseBitLength, otherBaseBitLength);
            }
            return otherNumbers;
        }
        /// <summary>
        /// 二进制转换其他进制
        /// </summary>
        /// <param name="binaryValue"></param>
        /// <param name="otherBase"></param>
        /// <returns></returns>
        public static Number[] BinaryToOther(string binaryValue, int otherBase)
        {
            if (binaryValue == null)
                throw new ArgumentNullException(nameof(binaryValue));

            return new Number[] { };
        }
        /// <summary>
        /// 其他进制转换二进制
        /// </summary>
        /// <param name="otherValue"></param>
        /// <param name="otherBase"></param>
        /// <returns></returns>
        public static Number[] OtherToBinary(string otherValue, int otherBase)
        {
            if (otherValue == null)
                throw new ArgumentNullException(nameof(otherValue));
            return new Number[] { };
        }
        /// <summary>
        /// 进制转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sourceBase"></param>
        /// <param name="targetBase"></param>
        /// <returns></returns>
        public static string Conversion(string value, int sourceBase, int targetBase)
        {
            return "";
        }
    }
}

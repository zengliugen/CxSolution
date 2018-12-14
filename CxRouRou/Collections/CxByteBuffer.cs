using System;
using System.Collections.Generic;
using System.Text;

namespace CxRouRou.Collections
{
    /// <summary>
    /// 字节缓冲区
    /// </summary>
    public class CxByteBuffer
    {
        /// <summary>
        /// 空数据
        /// </summary>
        private static readonly byte[] EmptyData = new byte[0];
        /// <summary>
        /// 默认数据长度
        /// </summary>
        private const int DefultDataLength = 4;
        /// <summary>
        /// 数据集合 不可使用Data.Length获取长度 应使用Length
        /// </summary>
        protected byte[] Data;
        /// <summary>
        /// 读取偏移
        /// </summary>
        protected int ReadPos;
        /// <summary>
        /// 写入偏移 修改后将会覆盖之后的数据
        /// </summary>
        protected int WritePos;
        /// <summary>
        /// 字符串编码
        /// </summary>
        protected Encoding StringEncoding = Encoding.UTF8;
        /// <summary>
        /// 容量
        /// </summary>
        public virtual int Capacity
        {
            get
            {
                return Data.Length;
            }
            set
            {
                if (value < WritePos)
                {
                    return;
                }
                if (value != Data.Length)
                {
                    if (value > 0)
                    {
                        byte[] array = new byte[value];
                        if (WritePos > 0)
                        {
                            Array.Copy(Data, 0, array, 0, WritePos);
                        }
                        Data = array;
                    }
                }
            }
        }
        /// <summary>
        /// 数据长度
        /// </summary>
        public virtual int Length
        {
            get
            {
                return WritePos;
            }
        }
        /// <summary>
        /// 判断是否已读取到末尾
        /// </summary>
        public virtual bool Eof
        {
            get
            {
                return ReadPos >= Data.Length;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="capacity">容量</param>
        public CxByteBuffer(int capacity)
        {
            Data = EmptyData;
            Capacity = capacity;
        }
        /// <summary>
        /// 以指定数据创建，不复制源数据
        /// </summary>
        /// <param name="data"></param>
        public CxByteBuffer(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException();
            WritePos = data.Length;
        }
        /// <summary>
        /// 获取一个数据
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public virtual byte this[int index]
        {
            get
            {
                return Data[index];
            }
        }
        /// <summary>
        /// 获取一段数据(拷贝)
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public virtual byte[] this[int index, int length]
        {
            get
            {
                return GetBytesAt(index, length);
            }
        }
        /// <summary>
        /// 清空 重置读取写入偏移 
        /// </summary>
        /// <param name="resetData"></param>
        public virtual void Clear(bool resetData = false)
        {
            ReadPos = 0;
            WritePos = 0;
            if (resetData)
            {
                Array.Clear(Data, 0, WritePos);
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToArray()
        {
            return GetBytesAt(0, Length);
        }
        /// <summary>
        /// 动态分配大小
        /// </summary>
        /// <param name="size"></param>
        protected virtual void EnsureCapacity(int size)
        {
            int min = WritePos + size;
            if (Data.Length < min)
            {
                int num = (Data.Length == 0) ? DefultDataLength : (Data.Length * 2);
                if (num < min)
                {
                    num = min;
                }
                Capacity = num;
            }
        }
        /// <summary>
        /// 追加数据 不写入数据长度
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Append(byte[] data)
        {
            return Append(data, 0, data.Length);
        }
        /// <summary>
        /// 追加数据 不写入数据长度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Append(byte[] data, int index, int length)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }
            if (index < 0 || length < 0 || index + length < data.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            EnsureCapacity(length);
            Array.Copy(data, index, Data, WritePos, length);
            WritePos += length;
            return this;
        }
        /// <summary>
        /// 获取数据 更新读取偏移
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual byte[] GetBytes(int length)
        {
            byte[] data = GetBytesAt(ReadPos, length);
            ReadPos += length;
            return data;
        }
        /// <summary>
        /// 获取数据 不更新读取偏移
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual byte[] GetBytesAt(int index, int length)
        {
            return Data.GetArray(index, length);
        }
        #region 压入数据
        /// <summary>
        /// 压入byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(byte value)
        {
            EnsureCapacity(1);
            Data[WritePos++] = value;
            return this;
        }
        /// <summary>
        /// 压入sbyte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(sbyte value)
        {
            return Push((byte)value);
        }
        /// <summary>
        /// 压入bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(bool value)
        {
            EnsureCapacity(1);
            byte b = 0;
            if (value)
            {
                b = 1;
            }
            Data[WritePos++] = b;
            return this;
        }
        /// <summary>
        /// 压入short
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(short value)
        {
            EnsureCapacity(2);
            Data[WritePos++] = (byte)(value >> 8);
            Data[WritePos++] = (byte)(value << 8 >> 8);
            return this;
        }
        /// <summary>
        /// 压入ushort
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(ushort value)
        {
            return Push((short)value);
        }
        /// <summary>
        /// 压入char
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(char value)
        {
            return Push((short)value);
        }
        /// <summary>
        /// 压入int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(int value)
        {
            EnsureCapacity(4);
            Data[WritePos++] = (byte)(value >> 24);
            Data[WritePos++] = (byte)(value << 8 >> 24);
            Data[WritePos++] = (byte)(value << 16 >> 24);
            Data[WritePos++] = (byte)(value << 24 >> 24);
            return this;
        }
        /// <summary>
        /// 压入uint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(uint value)
        {
            return Push((int)value);
        }
        /// <summary>
        /// 压入float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual unsafe CxByteBuffer Push(float value)
        {
            return Push(*(int*)(&value));
        }
        /// <summary>
        /// 压入long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(long value)
        {
            EnsureCapacity(8);
            Data[WritePos++] = (byte)(value >> 56);
            Data[WritePos++] = (byte)(value << 8 >> 56);
            Data[WritePos++] = (byte)(value << 16 >> 56);
            Data[WritePos++] = (byte)(value << 24 >> 56);
            Data[WritePos++] = (byte)(value << 32 >> 56);
            Data[WritePos++] = (byte)(value << 40 >> 56);
            Data[WritePos++] = (byte)(value << 48 >> 56);
            Data[WritePos++] = (byte)(value << 56 >> 56);
            return this;
        }
        /// <summary>
        /// 压入ulong
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(ulong value)
        {
            return Push((long)value);
        }
        /// <summary>
        /// 压入double
        /// </summary>
        /// <param name="vlaue"></param>
        /// <returns></returns>
        public virtual unsafe CxByteBuffer Push(double vlaue)
        {
            return Push(*(long*)(&vlaue));
        }
        /// <summary>
        /// 压入byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(byte[] value)
        {
            if (value.Length > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("Push byte array fail, array lenght greater than ushort.MaxValue");
            }
            Push((ushort)value.Length);
            Append(value);
            return this;
        }
        /// <summary>
        /// 压入string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push(string value)
        {
            return Push(StringEncoding.GetBytes(value));
        }
        #endregion

        #region 弹出数据

        #endregion
    }
}

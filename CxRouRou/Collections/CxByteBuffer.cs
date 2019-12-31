//CHECK_LOOP_REFERENCE_OFF 自动压入弹出 检查循环引用是否关闭
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using CxSolution.CxRouRou.Attributes;
using CxSolution.CxRouRou.Expands;

namespace CxSolution.CxRouRou.Collections
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
        public CxByteBuffer(int capacity = 4)
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
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is CxByteBuffer))
            {
                return false;
            }
            if (!(obj is CxByteBuffer other))
            {
                return false;
            }
            return GetBytesAt(0, Length).EqualsEx(other.GetBytesAt(0, other.Length));
        }
        /// <summary>
        /// 获取HashCode(使用当前数据进行计算)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 0;
            byte[] data = GetBytesAt(0, Length);
            foreach (var d in data)
            {
                hashCode += d.GetHashCode();
            }
            return hashCode;
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
            //Data[WritePos++] = (byte)(value >> 8);
            //Data[WritePos++] = (byte)(value << 8 >> 8);

            Data[WritePos++] = (byte)(value >> 8);
            Data[WritePos++] = (byte)value;
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
            //Data[WritePos++] = (byte)(value >> 24);
            //Data[WritePos++] = (byte)(value << 8 >> 24);
            //Data[WritePos++] = (byte)(value << 16 >> 24);
            //Data[WritePos++] = (byte)(value << 24 >> 24);

            Data[WritePos++] = (byte)(value >> 24);
            Data[WritePos++] = (byte)(value >> 16);
            Data[WritePos++] = (byte)(value >> 8);
            Data[WritePos++] = (byte)value;
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
            //Data[WritePos++] = (byte)(value >> 56);
            //Data[WritePos++] = (byte)(value << 8 >> 56);
            //Data[WritePos++] = (byte)(value << 16 >> 56);
            //Data[WritePos++] = (byte)(value << 24 >> 56);
            //Data[WritePos++] = (byte)(value << 32 >> 56);
            //Data[WritePos++] = (byte)(value << 40 >> 56);
            //Data[WritePos++] = (byte)(value << 48 >> 56);
            //Data[WritePos++] = (byte)(value << 56 >> 56);

            Data[WritePos++] = (byte)(value >> 56);
            Data[WritePos++] = (byte)(value >> 48);
            Data[WritePos++] = (byte)(value >> 40);
            Data[WritePos++] = (byte)(value >> 32);
            Data[WritePos++] = (byte)(value >> 24);
            Data[WritePos++] = (byte)(value >> 16);
            Data[WritePos++] = (byte)(value >> 8);
            Data[WritePos++] = (byte)value;
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
                throw new ArgumentOutOfRangeException("value", CxString.Format("value数组长度不可大于{0}", ushort.MaxValue));
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
        /// <summary>
        /// 压入字面常量byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_byte(int value)
        {
            return Push((byte)value);
        }
        /// <summary>
        /// 压入字面常量sbyte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_sbyte(int value)
        {
            return Push((sbyte)value);
        }
        /// <summary>
        /// 压入字面常量short
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_short(int value)
        {
            return Push((short)value);
        }
        /// <summary>
        /// 压入字面常量ushort
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_ushort(int value)
        {
            return Push((ushort)value);
        }
        /// <summary>
        /// 压入字面常量char
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_char(int value)
        {
            return Push((char)value);
        }
        /// <summary>
        /// 压入字面常量int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_int(int value)
        {
            return Push(value);
        }
        /// <summary>
        /// 压入字面常量uint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_uint(int value)
        {
            return Push((uint)value);
        }
        /// <summary>
        /// 压入字面常量float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_float(int value)
        {
            return Push((float)value);
        }
        /// <summary>
        /// 压入字面常量float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_float(double value)
        {
            return Push((float)value);
        }
        /// <summary>
        /// 压入字面常量long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_long(int value)
        {
            return Push((long)value);
        }
        /// <summary>
        /// 压入字面常量ulong
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_ulong(int value)
        {
            return Push((ulong)value);
        }
        /// <summary>
        /// 压入字面常量double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_double(int value)
        {
            return Push((double)value);
        }
        /// <summary>
        /// 压入字面常量double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual CxByteBuffer Push_double(double value)
        {
            return Push((double)value);
        }
        #endregion
        #region 弹出数据
        /// <summary>
        /// 弹出byte
        /// </summary>
        /// <returns></returns>
        public virtual byte Pop_byte()
        {
            if (Eof || ReadPos + 1 > Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            return Data[ReadPos++];
        }
        /// <summary>
        /// 弹出sbyte
        /// </summary>
        /// <returns></returns>
        public virtual sbyte Pop_sbyte()
        {
            return (sbyte)Pop_byte();
        }
        /// <summary>
        /// 弹出bool
        /// </summary>
        /// <returns></returns>
        public virtual bool Pop_bool()
        {
            bool b = false;
            if (Pop_byte() == 1)
            {
                b = true;
            }
            return b;

        }
        /// <summary>
        /// 弹出short
        /// </summary>
        /// <returns></returns>
        public virtual short Pop_short()
        {
            if (Eof || ReadPos + 2 > Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            return (short)(Data[ReadPos++] << 8 | Data[ReadPos++]);
        }
        /// <summary>
        /// 弹出ushort
        /// </summary>
        /// <returns></returns>
        public virtual ushort Pop_ushort()
        {
            return (ushort)Pop_short();
        }
        /// <summary>
        /// 弹出char
        /// </summary>
        /// <returns></returns>
        public virtual char Pop_char()
        {
            return (char)Pop_short();
        }
        /// <summary>
        /// 弹出int
        /// </summary>
        /// <returns></returns>
        public virtual int Pop_int()
        {
            if (Eof || ReadPos + 4 > Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            return (int)(Data[ReadPos++] << 24 | Data[ReadPos++] << 16 | Data[ReadPos++] << 8 | Data[ReadPos++]);
        }
        /// <summary>
        /// 弹出uint
        /// </summary>
        /// <returns></returns>
        public virtual uint Pop_uint()
        {
            return (uint)Pop_int();
        }
        /// <summary>
        /// 弹出float
        /// </summary>
        /// <returns></returns>
        public virtual unsafe float Pop_float()
        {
            int value = Pop_int();
            return *(float*)&value;
        }
        /// <summary>
        /// 弹出long
        /// </summary>
        /// <returns></returns>
        public virtual long Pop_long()
        {
            if (Eof || ReadPos + 8 > Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            //return (long)Pop_int() << 32 | Pop_uint();
            return ((long)Data[ReadPos++]) << 56 | ((long)Data[ReadPos++]) << 48 | ((long)Data[ReadPos++]) << 40 | ((long)Data[ReadPos++]) << 32 | ((long)Data[ReadPos++]) << 24 | ((long)Data[ReadPos++]) << 16 | ((long)Data[ReadPos++]) << 8 | Data[ReadPos++];
        }
        /// <summary>
        /// 弹出ulong
        /// </summary>
        /// <returns></returns>
        public virtual ulong Pop_ulong()
        {
            return (ulong)Pop_long();
        }
        /// <summary>
        /// 弹出double
        /// </summary>
        /// <returns></returns>
        public virtual unsafe double Pop_double()
        {
            long value = Pop_long();
            return *(double*)&value;
        }
        /// <summary>
        /// 弹出byte数组
        /// </summary>
        /// <returns></returns>
        public virtual byte[] Pop_bytes()
        {
            ushort length = Pop_ushort();
            return GetBytes(length);
        }
        /// <summary>
        /// 弹出string
        /// </summary>
        /// <returns></returns>
        public virtual string Pop_string()
        {
            return StringEncoding.GetString(Pop_bytes());
        }
        #endregion
        #region 自动压入弹出
        /// <summary>
        /// 是否处理私有字段
        /// </summary>
        public static bool IsHandlePrivateField;
        /// <summary>
        /// 是否处理私有属性
        /// </summary>
        public static bool IsHandlePrivateProperty;
        /// <summary>
        /// 类型比较
        /// </summary>
        private static readonly Type byteType = typeof(byte);
        private static readonly Type stringType = typeof(string);
        private static readonly Type IListType = typeof(IList);
        private static readonly Type IDictionaryType = typeof(IDictionary);
        /// <summary>
        /// 特性判断类型
        /// </summary>
        private static readonly Type ByteBufferAttributeType = typeof(AutoByteBufferAttribute);
#if !CHECK_LOOP_REFERENCE_OFF
        /// <summary>
        /// 自动压入
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public virtual CxByteBuffer AutoPush(object obj, Dictionary<object, int> flags = null)
#else
        /// <summary>
        /// 自动压入
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual CxByteBuffer AutoPush(object obj)
#endif
        {
            Type type = obj.GetType();
            TypeInfo typeInfo = type as TypeInfo;
            if (obj == null)
            {
                throw new NullReferenceException("压入对象不可为空");
            }
#if !CHECK_LOOP_REFERENCE_OFF
            if (flags == null)
            {
                flags = new Dictionary<object, int>();
            }
            if (flags.ContainsKey(obj))
            {
                throw new Exception("压入对象包含了循环引用");
            }
#endif
            //值类型
            if (type.IsValueType)
            {
                //基础类型
                if (type.IsPrimitive)
                {
                    switch (type.Name)
                    {
                        case "Byte":
                            Push((byte)obj);
                            break;
                        case "SByte":
                            Push((sbyte)obj);
                            break;
                        case "Boolean":
                            Push((bool)obj);
                            break;
                        case "Int16":
                            Push((short)obj);
                            break;
                        case "UInt16":
                            Push((ushort)obj);
                            break;
                        case "Char":
                            Push((char)obj);
                            break;
                        case "Int32":
                            Push((int)obj);
                            break;
                        case "UInt32":
                            Push((uint)obj);
                            break;
                        case "Single":
                            Push((float)obj);
                            break;
                        case "Int64":
                            Push((long)obj);
                            break;
                        case "UInt64":
                            Push((ulong)obj);
                            break;
                        case "Double":
                            Push((double)obj);
                            break;
                        //无法解析的基础类型
                        default:
                            throw new Exception("自动压入失败，无法解析的基础类型");
                    }
                }
                //枚举类型
                else if (type.IsEnum)
                {
                    Push((int)obj);
                }
                //结构体
                else
                {
#if !CHECK_LOOP_REFERENCE_OFF
                    AutoPushStructOrClass(ref typeInfo, ref obj, flags);
#else
                    AutoPushStructOrClass(ref typeInfo, ref obj);
#endif
                }
            }
            //类类型
            else if (type.IsClass)
            {
                //Array
                if (type.IsArray)
                {
#if !CHECK_LOOP_REFERENCE_OFF
                    //保存标记，防止循环引用
                    flags.Add(obj, 1);
#endif
                    if (obj is Array array)
                    {
                        Type elementType = type.GetElementType();
                        //如果是byte数组，直接进行写入
                        if (byteType.IsAssignableFrom(elementType))
                        {
                            Push((byte[])obj);
                        }
                        else
                        {
                            int length = array.Length;
                            Push((ushort)length);
                            for (int i = 0; i < length; i++)
                            {
#if !CHECK_LOOP_REFERENCE_OFF
                                AutoPush(array.GetValue(i), flags);
#else
                                AutoPush(array.GetValue(i));
#endif
                            }
                        }
                    }
                }
                //string
                else if (stringType.IsAssignableFrom(type))
                {
                    Push((string)obj);
                }
                //IList
                else if (IListType.IsAssignableFrom(type))
                {
#if !CHECK_LOOP_REFERENCE_OFF
                    //保存标记，防止循环引用
                    flags.Add(obj, 1);
#endif
                    if (obj is IList list)
                    {
                        int count = list.Count;
                        Push((ushort)count);
                        for (int i = 0; i < count; i++)
                        {
#if !CHECK_LOOP_REFERENCE_OFF
                            AutoPush(list[i], flags);
#else
                            AutoPush(list[i]);
#endif
                        }
                    }
                }
                //IDictionary
                else if (IDictionaryType.IsAssignableFrom(type))
                {
#if !CHECK_LOOP_REFERENCE_OFF
                    //保存标记，防止循环引用
                    flags.Add(obj, 1);
#endif
                    if (obj is IDictionary dictionary)
                    {
                        int count = dictionary.Count;
                        Push((ushort)count);
                        foreach (DictionaryEntry kv in dictionary)
                        {
#if !CHECK_LOOP_REFERENCE_OFF
                            AutoPush(kv.Key, flags);
                            AutoPush(kv.Value, flags);
#else
                            AutoPush(kv.Key);
                            AutoPush(kv.Value);
#endif
                        }
                    }
                }
                //其他类类型
                else
                {
#if !CHECK_LOOP_REFERENCE_OFF
                    AutoPushStructOrClass(ref typeInfo, ref obj, flags);
#else
                    AutoPushStructOrClass(ref typeInfo, ref obj);
#endif
                }

            }
            //其他类型(不处理的类型)
            else
            {

            }
            return this;
        }
#if !CHECK_LOOP_REFERENCE_OFF

        /// <summary>
        /// 自动压入结构体或类
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        public virtual void AutoPushStructOrClass(ref TypeInfo typeInfo, ref Object obj, Dictionary<object, int> flags = null)
#else
        /// <summary>
        /// 自动压入结构体或类
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        public virtual void AutoPushStructOrClass(ref TypeInfo typeInfo, ref Object obj, Dictionary<object, int> flags = null)
#endif
        {
            //处理字段 仅处理具有ByteBufferAttribute特性的字段
            List<FieldInfo> fieldInfoList = new List<FieldInfo>();
            //此处获取的字段包含了public标记与private标记
            fieldInfoList.AddRange(typeInfo.DeclaredFields);
            fieldInfoList.Sort((l, r) =>
            {
                return l.Name.CompareTo(r.Name);
            });
            FieldInfo[] fieldInfos = fieldInfoList.ToArray();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                if (fieldInfo.GetCustomAttribute(ByteBufferAttributeType) is AutoByteBufferAttribute byteBufferAttribute && byteBufferAttribute.Handle)
                {
#if !CHECK_LOOP_REFERENCE_OFF
                    AutoPush(fieldInfo.GetValue(obj), flags);
#else
                    AutoPush(fieldInfo.GetValue(obj));
#endif
                }
            }
            //处理属性 仅处理具有ByteBufferAttribute特性的属性
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
            //此处获取的属性包含了public标记与private标记
            propertyInfoList.AddRange(typeInfo.DeclaredProperties);
            propertyInfoList.Sort((l, r) =>
            {
                return l.Name.CompareTo(r.Name);
            });
            PropertyInfo[] propertyInfos = propertyInfoList.ToArray();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                if (propertyInfo.GetCustomAttribute(ByteBufferAttributeType) is AutoByteBufferAttribute byteBufferAttribute && byteBufferAttribute.Handle)
                {
#if !CHECK_LOOP_REFERENCE_OFF
                    AutoPush(propertyInfo.GetValue(obj), flags);
#else
                    AutoPush(propertyInfo.GetValue(obj));
#endif
                }
            }
        }
        /// <summary>
        /// 自动弹出
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual object AutoPop(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "参数不可为空");
            }
            object obj = null;
            TypeInfo typeInfo = type as TypeInfo;
            //值类型
            if (type.IsValueType)
            {
                //基础类型
                if (type.IsPrimitive)
                {
                    switch (type.Name)
                    {
                        case "Byte":
                            obj = Pop_byte();
                            break;
                        case "SByte":
                            obj = Pop_sbyte();
                            break;
                        case "Boolean":
                            obj = Pop_bool();
                            break;
                        case "Int16":
                            obj = Pop_short();
                            break;
                        case "UInt16":
                            obj = Pop_ushort();
                            break;
                        case "Char":
                            obj = Pop_char();
                            break;
                        case "Int32":
                            obj = Pop_int();
                            break;
                        case "UInt32":
                            obj = Pop_uint();
                            break;
                        case "Single":
                            obj = Pop_float();
                            break;
                        case "Int64":
                            obj = Pop_long();
                            break;
                        case "UInt64":
                            obj = Pop_ulong();
                            break;
                        case "Double":
                            obj = Pop_double();
                            break;
                        //无法解析的基础类型
                        default:
                            throw new Exception("自动压入失败，无法解析的基础类型");
                    }
                }
                //枚举类型
                else if (type.IsEnum)
                {
                    obj = Pop_int();
                }
                //结构体
                else
                {
                    obj = Activator.CreateInstance(type);
                    AutoPopStructOrClass(ref typeInfo, ref obj);
                }
            }
            //类类型
            else if (type.IsClass)
            {
                //Array
                if (type.IsArray)
                {
                    Type elementType = type.GetElementType();
                    if (byteType.IsAssignableFrom(elementType))
                    {
                        obj = Pop_bytes();
                    }
                    else
                    {
                        int length = Pop_ushort();
                        Array array = Array.CreateInstance(elementType, length);
                        for (int i = 0; i < length; i++)
                        {
                            array.SetValue(AutoPop(elementType), i);
                        }
                        obj = array;
                    }
                }
                //string
                else if (stringType.IsAssignableFrom(type))
                {
                    obj = Pop_string();
                }
                //IList
                else if (IListType.IsAssignableFrom(type))
                {
                    Type[] genericArguments = type.GetGenericArguments();
                    int count = Pop_ushort();
                    IList list = Activator.CreateInstance(type) as IList;
                    for (int i = 0; i < count; i++)
                    {
                        list.Add(AutoPop(genericArguments[0]));
                    }
                    obj = list;
                }
                //IDictionary
                else if (IDictionaryType.IsAssignableFrom(type))
                {
                    Type[] genericArguments = type.GetGenericArguments();
                    int count = Pop_ushort();
                    IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
                    for (int i = 0; i < count; i++)
                    {
                        dictionary.Add(AutoPop(genericArguments[0]), AutoPop(genericArguments[1]));
                    }
                    obj = dictionary;
                }
                //其他类类型
                else
                {
                    obj = Activator.CreateInstance(type);
                    AutoPopStructOrClass(ref typeInfo, ref obj);
                }
            }
            //其他类型(不处理的类型)
            else
            {

            }
            return obj;
        }
        /// <summary>
        /// 自动弹出结构体或类
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="obj"></param>
        public virtual void AutoPopStructOrClass(ref TypeInfo typeInfo, ref object obj)
        {
            //处理字段 仅处理具有ByteBufferAttribute特性的字段
            List<FieldInfo> fieldInfoList = new List<FieldInfo>();
            //此处获取的字段包含了public标记与private标记
            fieldInfoList.AddRange(typeInfo.DeclaredFields);
            fieldInfoList.Sort((l, r) =>
            {
                return l.Name.CompareTo(r.Name);
            });
            FieldInfo[] fieldInfos = fieldInfoList.ToArray();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                if (fieldInfo.GetCustomAttribute(ByteBufferAttributeType) is AutoByteBufferAttribute byteBufferAttribute && byteBufferAttribute.Handle)
                {
                    fieldInfo.SetValue(obj, AutoPop(fieldInfo.FieldType));
                }
            }
            //处理属性 仅处理具有ByteBufferAttribute特性的属性
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
            //此处获取的属性包含了public标记与private标记
            propertyInfoList.AddRange(typeInfo.DeclaredProperties);
            propertyInfoList.Sort((l, r) =>
            {
                return l.Name.CompareTo(r.Name);
            });
            PropertyInfo[] propertyInfos = propertyInfoList.ToArray();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                if (propertyInfo.GetCustomAttribute(ByteBufferAttributeType) is AutoByteBufferAttribute byteBufferAttribute && byteBufferAttribute.Handle)
                {
                    propertyInfo.SetValue(obj, AutoPop(propertyInfo.PropertyType));
                }
            }
        }
        #endregion
    }
}

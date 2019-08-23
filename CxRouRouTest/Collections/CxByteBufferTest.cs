using CxSolution.CxRouRou.Attributes;
using CxSolution.CxRouRou.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxSolution.CxRouRouTest.Collections
{
    /// <summary>
    /// CxByteBuffer测试
    /// </summary>
    public class CxByteBufferTest : CxTestBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="testName"></param>
        public CxByteBufferTest(string testName) : base(testName)
        {
        }
        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public override void Dispose()
        {
        }
        /// <summary>
        /// 测试内容
        /// </summary>
        /// <returns></returns>
        public override bool Test()
        {
            PrintL("---------------------------------------------");

            PrintL("Normal Test Start:");
            bool normalTestResult = NormalTest();
            PrintL("Normal Test End Result:{0}", normalTestResult);

            PrintL("---------------------------------------------");

            PrintL("Auto Test Start:");
            bool autoTestResult = AutoTest();
            PrintL("Auto Test End Result:{0}", autoTestResult);

            PrintL("---------------------------------------------");

            SpeedTest();

            PrintL("---------------------------------------------");

            return normalTestResult && autoTestResult;
            //return true;
        }
        /// <summary>
        /// 普通测试
        /// </summary>
        /// <returns></returns>
        private bool NormalTest()
        {
            byte value_byte = 123 * 2;
            sbyte value_sbyte = 123;
            bool value_bool = false;
            short value_short = 12345;
            ushort value_ushort = 12345 * 2;
            char value_char = 'c';
            int value_int = 1234567890;
            uint value_uint = (uint)1234567890 * 2;
            float value_float = 123456.789f;
            long value_long = 1234567890123;
            ulong value_ulong = 1234567890123 * 2;
            double value_double = 1234567890.123456789;
            byte[] value_bytes = new byte[] { 1, 2, 3, 4, 5 };
            string value_string = "12345678901234567890";

            CxByteBuffer byteBuffer = new CxByteBuffer(0);
            byteBuffer.Push(value_byte);
            byteBuffer.Push(value_sbyte);
            byteBuffer.Push(value_bool);
            byteBuffer.Push(value_short);
            byteBuffer.Push(value_ushort);
            byteBuffer.Push(value_char);
            byteBuffer.Push(value_int);
            byteBuffer.Push(value_uint);
            byteBuffer.Push(value_float);
            byteBuffer.Push(value_long);
            byteBuffer.Push(value_ulong);
            byteBuffer.Push(value_double);
            byteBuffer.Push(value_bytes);
            byteBuffer.Push(value_string);
            bool result = false;
            List<bool> resultList = new List<bool>();
            result = byteBuffer.Pop_byte() == value_byte;
            PrintL("byte value:{0} result:{1}", value_byte, result);
            resultList.Add(result);
            result = byteBuffer.Pop_sbyte() == value_sbyte;
            PrintL("sbyte value:{0} result:{1}", value_sbyte, result);
            resultList.Add(result);
            result = byteBuffer.Pop_bool() == value_bool;
            PrintL("bool value:{0} result:{1}", value_bool, result);
            resultList.Add(result);
            result = byteBuffer.Pop_short() == value_short;
            PrintL("short value:{0} result:{1}", value_short, result);
            resultList.Add(result);
            result = byteBuffer.Pop_ushort() == value_ushort;
            PrintL("ushort value:{0} result:{1}", value_ushort, result);
            resultList.Add(result);
            result = byteBuffer.Pop_char() == value_char;
            PrintL("char value:{0} result:{1}", value_char, result);
            resultList.Add(result);
            result = byteBuffer.Pop_int() == value_int;
            PrintL("int value:{0} result:{1}", value_int, result);
            resultList.Add(result);
            result = byteBuffer.Pop_uint() == value_uint;
            PrintL("uint value:{0} result:{1}", value_uint, result);
            resultList.Add(result);
            result = byteBuffer.Pop_float() == value_float;
            PrintL("float value:{0} result:{1}", value_float, result);
            resultList.Add(result);
            result = byteBuffer.Pop_long() == value_long;
            PrintL("long value:{0} result:{1}", value_long, result);
            resultList.Add(result);
            result = byteBuffer.Pop_ulong() == value_ulong;
            PrintL("ulong value:{0} result:{1}", value_ulong, result);
            resultList.Add(result);
            result = byteBuffer.Pop_double() == value_double;
            PrintL("double value:{0} result:{1}", value_double, result);
            resultList.Add(result);
            result = byteBuffer.Pop_bytes().EqualsEx(value_bytes);
            PrintL("bytes value:{0} result:{1}", value_bytes.ToStringEx(), result);
            resultList.Add(result);
            result = byteBuffer.Pop_string() == value_string;
            PrintL("string value:{0} result:{1}", value_string, result);
            resultList.Add(result);

            foreach (var item in resultList)
            {
                if (!item)
                {
                    return false;
                }
            }
            PrintL("长度:{0}", byteBuffer.Length);
            return true;
        }

        private bool AutoTest()
        {

            byte value_byte = 123 * 2;
            sbyte value_sbyte = 123;
            bool value_bool = false;
            short value_short = 12345;
            ushort value_ushort = 12345 * 2;
            char value_char = 'c';
            int value_int = 1234567890;
            uint value_uint = (uint)1234567890 * 2;
            float value_float = 123456.789f;
            long value_long = 1234567890123;
            ulong value_ulong = 1234567890123 * 2;
            double value_double = 1234567890.123456789;
            byte[] value_bytes = new byte[] { 1, 2, 3, 4, 5 };
            string value_string = "12345678901234567890";

            List<int> value_list = new List<int>() { 1, 2, 3, 4, 5 };
            Dictionary<int, int> value_dictionary = new Dictionary<int, int>() { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            TestStruct value_struct = new TestStruct()
            {
                p_int = 1234,
                p_ints = new int[] { 1, 2, 3, 4, 5 },
                p_intList = new List<int>() { 1, 2, 3, 4, 5 },
                p_intDictionary = new Dictionary<int, int>() { { 1, 2 }, { 3, 4 }, { 5, 6 } }
            };
            TestClass value_class = new TestClass()
            {
                p_int = 1234,
                p_ints = new int[] { 1, 2, 3, 4, 5 },
                p_intList = new List<int>() { 1, 2, 3, 4, 5 },
                p_intDictionary = new Dictionary<int, int>() { { 1, 2 }, { 3, 4 }, { 5, 6 } },
                p_testStruct = value_struct,
            };

            CxByteBuffer byteBuffer = new CxByteBuffer(0);
            byteBuffer.AutoPush(value_byte);
            byteBuffer.AutoPush(value_sbyte);
            byteBuffer.AutoPush(value_bool);
            byteBuffer.AutoPush(value_short);
            byteBuffer.AutoPush(value_ushort);
            byteBuffer.AutoPush(value_char);
            byteBuffer.AutoPush(value_int);
            byteBuffer.AutoPush(value_uint);
            byteBuffer.AutoPush(value_float);
            byteBuffer.AutoPush(value_long);
            byteBuffer.AutoPush(value_ulong);
            byteBuffer.AutoPush(value_double);
            byteBuffer.AutoPush(value_bytes);
            byteBuffer.AutoPush(value_string);

            //byteBuffer.AutoPush(value_list);
            //byteBuffer.AutoPush(value_dictionary);
            //byteBuffer.AutoPush(value_struct);
            //byteBuffer.AutoPush(value_class);


            bool result = false;
            List<bool> resultList = new List<bool>();
            result = (byte)byteBuffer.AutoPop(typeof(byte)) == value_byte;
            PrintL("byte value:{0} result:{1}", value_byte, result);
            resultList.Add(result);
            result = (sbyte)byteBuffer.AutoPop(typeof(sbyte)) == value_sbyte;
            PrintL("sbyte value:{0} result:{1}", value_sbyte, result);
            resultList.Add(result);
            result = (bool)byteBuffer.AutoPop(typeof(bool)) == value_bool;
            PrintL("bool value:{0} result:{1}", value_bool, result);
            resultList.Add(result);
            result = (short)byteBuffer.AutoPop(typeof(short)) == value_short;
            PrintL("short value:{0} result:{1}", value_short, result);
            resultList.Add(result);
            result = (ushort)byteBuffer.AutoPop(typeof(ushort)) == value_ushort;
            PrintL("ushort value:{0} result:{1}", value_ushort, result);
            resultList.Add(result);
            result = (char)byteBuffer.AutoPop(typeof(char)) == value_char;
            PrintL("char value:{0} result:{1}", value_char, result);
            resultList.Add(result);
            result = (int)byteBuffer.AutoPop(typeof(int)) == value_int;
            PrintL("int value:{0} result:{1}", value_int, result);
            resultList.Add(result);
            result = (uint)byteBuffer.AutoPop(typeof(uint)) == value_uint;
            PrintL("uint value:{0} result:{1}", value_uint, result);
            resultList.Add(result);
            result = (float)byteBuffer.AutoPop(typeof(float)) == value_float;
            PrintL("float value:{0} result:{1}", value_float, result);
            resultList.Add(result);
            result = (long)byteBuffer.AutoPop(typeof(long)) == value_long;
            PrintL("long value:{0} result:{1}", value_long, result);
            resultList.Add(result);
            result = (ulong)byteBuffer.AutoPop(typeof(ulong)) == value_ulong;
            PrintL("ulong value:{0} result:{1}", value_ulong, result);
            resultList.Add(result);
            result = (double)byteBuffer.AutoPop(typeof(double)) == value_double;
            PrintL("double value:{0} result:{1}", value_double, result);
            resultList.Add(result);
            result = ((byte[])byteBuffer.AutoPop(typeof(byte[]))).EqualsEx(value_bytes);
            PrintL("bytes value:{0} result:{1}", value_bytes.ToStringEx(), result);
            resultList.Add(result);
            result = (string)byteBuffer.AutoPop(typeof(string)) == value_string;
            PrintL("string value:{0} result:{1}", value_string, result);
            resultList.Add(result);

            //result = ((List<int>)byteBuffer.AutoPop(typeof(List<int>))).EqualsEx(value_list);
            //PrintL("List<int> value:{0} result:{1}", value_list.ToStringEx(), result);
            //resultList.Add(result);
            //result = ((Dictionary<int, int>)byteBuffer.AutoPop(typeof(Dictionary<int, int>))).EqualsEx(value_dictionary);
            //PrintL("Dictionary<int,int> value:{0} result:{1}", value_dictionary.ToStringEx(), result);
            //resultList.Add(result);
            //result = ((TestStruct)byteBuffer.AutoPop(typeof(TestStruct))).EqualsEx(value_struct);
            //PrintL("TestStruct value:{0} result:{1}", value_struct.ToStringEx(), result);
            //resultList.Add(result);
            //result = ((TestClass)byteBuffer.AutoPop(typeof(TestClass))).EqualsEx(value_class);
            //PrintL("TestClass value:{0} result:{1}", value_class.ToStringEx(), result);
            //resultList.Add(result);

            foreach (var item in resultList)
            {
                if (!item)
                {
                    return false;
                }
            }

            PrintL("长度:{0}", byteBuffer.Length);
            return true;
        }
        struct TestStruct
        {
            [AutoByteBuffer(true)]
            public int p_int;
            [AutoByteBuffer(true)]
            public int[] p_ints;
            [AutoByteBuffer(true)]
            public List<int> p_intList;
            [AutoByteBuffer(true)]
            public Dictionary<int, int> p_intDictionary;
            public bool EqualsEx(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                TestStruct target = (TestStruct)obj;
                if (!p_int.Equals(target.p_int))
                {
                    return false;
                }
                if (!p_ints.EqualsEx(target.p_ints))
                {
                    return false;
                }
                if (!p_intList.EqualsEx(target.p_intList))
                {
                    return false;
                }
                if (!p_intDictionary.EqualsEx(target.p_intDictionary))
                {
                    return false;
                }
                return true;
            }
            public string ToStringEx()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append(p_int);
                sb.Append(",");
                sb.Append(p_ints.ToStringEx());
                sb.Append(",");
                sb.Append(p_intList.ToStringEx());
                sb.Append(",");
                sb.Append(p_intDictionary.ToStringEx());
                sb.Append("}");
                return sb.ToString();
            }
        }
        class TestClass
        {
            [AutoByteBuffer(true)]
            public int p_int;
            [AutoByteBuffer(true)]
            public int[] p_ints;
            [AutoByteBuffer(true)]
            public List<int> p_intList;
            [AutoByteBuffer(true)]
            public Dictionary<int, int> p_intDictionary;
            [AutoByteBuffer(true)]
            public TestStruct p_testStruct;
            public bool EqualsEx(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                TestClass target = (TestClass)obj;
                if (!p_int.Equals(target.p_int))
                {
                    return false;
                }
                if (!p_ints.EqualsEx(target.p_ints))
                {
                    return false;
                }
                if (!p_intList.EqualsEx(target.p_intList))
                {
                    return false;
                }
                if (!p_intDictionary.EqualsEx(target.p_intDictionary))
                {
                    return false;
                }
                if (!p_testStruct.EqualsEx(target.p_testStruct))
                {
                    return false;
                }
                return true;
            }
            public string ToStringEx()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append(p_int);
                sb.Append(",");
                sb.Append(p_ints.ToStringEx());
                sb.Append(",");
                sb.Append(p_intList.ToStringEx());
                sb.Append(",");
                sb.Append(p_intDictionary.ToStringEx());
                sb.Append(",");
                sb.Append(p_testStruct.ToStringEx());
                sb.Append("}");
                return sb.ToString();
            }
        }

        private void SpeedTest()
        {
            byte value_byte = 123 * 2;
            sbyte value_sbyte = 123;
            bool value_bool = false;
            short value_short = 12345;
            ushort value_ushort = 12345 * 2;
            char value_char = 'c';
            int value_int = 1234567890;
            uint value_uint = (uint)1234567890 * 2;
            float value_float = 123456.789f;
            long value_long = 1234567890123;
            ulong value_ulong = 1234567890123 * 2;
            double value_double = 1234567890.123456789;
            byte[] value_bytes = new byte[] { 1, 2, 3, 4, 5 };
            string value_string = "12345678901234567890";

            CxByteBuffer normalByteBuffer = null;
            CxByteBuffer autoByteBuffer = null;

            Stopwatch normalStopwatch = new Stopwatch();
            Stopwatch autoStopwatch = new Stopwatch();

            int testCount = 10000;

            normalStopwatch.Start();
            for (int i = 0; i < testCount; i++)
            {
                normalByteBuffer = new CxByteBuffer();
                normalByteBuffer.Push(value_byte).Pop_byte();
                normalByteBuffer.Push(value_sbyte).Pop_sbyte();
                normalByteBuffer.Push(value_bool).Pop_bool();
                normalByteBuffer.Push(value_short).Pop_short();
                normalByteBuffer.Push(value_ushort).Pop_ushort();
                normalByteBuffer.Push(value_char).Pop_char();
                normalByteBuffer.Push(value_int).Pop_int();
                normalByteBuffer.Push(value_uint).Pop_uint();
                normalByteBuffer.Push(value_float).Pop_float();
                normalByteBuffer.Push(value_long).Pop_long();
                normalByteBuffer.Push(value_ulong).Pop_ulong();
                normalByteBuffer.Push(value_double).Pop_double();
                normalByteBuffer.Push(value_bytes).Pop_bytes();
                normalByteBuffer.Push(value_string).Pop_string();
            }
            normalStopwatch.Stop();
            PrintL("Normal 测试{0}次,耗时:{1}毫秒", testCount, normalStopwatch.ElapsedMilliseconds);

            autoStopwatch.Start();
            for (int i = 0; i < testCount; i++)
            {
                autoByteBuffer = new CxByteBuffer();
                autoByteBuffer.AutoPush(value_byte);
                var temp1 = (byte)autoByteBuffer.AutoPop(typeof(byte));
                autoByteBuffer.AutoPush(value_sbyte);
                var temp2 = (sbyte)autoByteBuffer.AutoPop(typeof(sbyte));
                autoByteBuffer.AutoPush(value_bool);
                var temp3 = (bool)autoByteBuffer.AutoPop(typeof(bool));
                autoByteBuffer.AutoPush(value_short);
                var temp4 = (short)autoByteBuffer.AutoPop(typeof(short));
                autoByteBuffer.AutoPush(value_ushort);
                var temp5 = (ushort)autoByteBuffer.AutoPop(typeof(ushort));
                autoByteBuffer.AutoPush(value_char);
                var temp6 = (char)autoByteBuffer.AutoPop(typeof(char));
                autoByteBuffer.AutoPush(value_int);
                var temp7 = (int)autoByteBuffer.AutoPop(typeof(int));
                autoByteBuffer.AutoPush(value_uint);
                var temp8 = (uint)autoByteBuffer.AutoPop(typeof(uint));
                autoByteBuffer.AutoPush(value_float);
                var temp9 = (float)autoByteBuffer.AutoPop(typeof(float));
                autoByteBuffer.AutoPush(value_long);
                var temp10 = (long)autoByteBuffer.AutoPop(typeof(long));
                autoByteBuffer.AutoPush(value_ulong);
                var temp11 = (ulong)autoByteBuffer.AutoPop(typeof(ulong));
                autoByteBuffer.AutoPush(value_double);
                var temp12 = (double)autoByteBuffer.AutoPop(typeof(double));
                autoByteBuffer.AutoPush(value_bytes);
                var temp13 = (byte[])autoByteBuffer.AutoPop(typeof(byte[]));
                autoByteBuffer.AutoPush(value_string);
                var temp14 = (string)autoByteBuffer.AutoPop(typeof(string));
            }
            autoStopwatch.Stop();
            PrintL("Auto 测试{0}次,耗时:{1}毫秒", testCount, autoStopwatch.ElapsedMilliseconds);

            PrintL("Normal的速度是Auto的{0}倍", 1f * autoStopwatch.ElapsedMilliseconds / normalStopwatch.ElapsedMilliseconds);
        }
    }
}

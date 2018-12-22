using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CxRouRou.Collections;

namespace CXRouRouTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CxByteBuffer_Test();
            Console.ReadKey();
        }

        static void PrintL(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
        static void CxByteBuffer_Test()
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
            PrintL("value:{0} result:{1}", value_byte, byteBuffer.Pop_byte() == value_byte);
            PrintL("value:{0} result:{1}", value_sbyte, byteBuffer.Pop_sbyte() == value_sbyte);
            PrintL("value:{0} result:{1}", value_bool, byteBuffer.Pop_bool() == value_bool);
            PrintL("value:{0} result:{1}", value_short, byteBuffer.Pop_short() == value_short);
            PrintL("value:{0} result:{1}", value_ushort, byteBuffer.Pop_ushort() == value_ushort);
            PrintL("value:{0} result:{1}", value_char, byteBuffer.Pop_char() == value_char);
            PrintL("value:{0} result:{1}", value_int, byteBuffer.Pop_int() == value_int);
            PrintL("value:{0} result:{1}", value_uint, byteBuffer.Pop_uint() == value_uint);
            PrintL("value:{0} result:{1}", value_float, byteBuffer.Pop_float() == value_float);
            PrintL("value:{0} result:{1}", value_long, byteBuffer.Pop_long() == value_long);
            PrintL("value:{0} result:{1}", value_ulong, byteBuffer.Pop_ulong() == value_ulong);
            PrintL("value:{0} result:{1}", value_double, byteBuffer.Pop_double() == value_double);
            PrintL("value:{0} result:{1}", value_bytes.ToStringEx(), byteBuffer.Pop_bytes().EqualsEx(value_bytes));
            PrintL("value:{0} result:{1}", value_string, byteBuffer.Pop_string() == value_string);
        }
    }
}

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
            CxByteBuffer cxByteBuffer = new CxByteBuffer(0);
            cxByteBuffer.Push(false);
            cxByteBuffer.Push((byte)1);
            cxByteBuffer.Push((sbyte)2);
            cxByteBuffer.Push((short)3);
            cxByteBuffer.Push((ushort)4);
            cxByteBuffer.Push((char)5);
            cxByteBuffer.Push((int)6);
            cxByteBuffer.Push((uint)7);
            cxByteBuffer.Push((float)8.8);
            cxByteBuffer.Push((long)9);
            cxByteBuffer.Push((ulong)10);
            cxByteBuffer.Push((double)11.11);
            cxByteBuffer.Push(new byte[] { 12, 13, 14 });
            cxByteBuffer.Push("1516171819202");
            Console.WriteLine(cxByteBuffer.Capacity);
            Console.WriteLine(cxByteBuffer.Length);
            Console.WriteLine(Encoding.UTF8.GetString(cxByteBuffer.ToArray()));
            Console.ReadLine();
        }
    }
}

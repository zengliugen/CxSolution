﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CxRouRou.Collections;
using CxRouRou.Util;
using CXRouRouTest.Collections;
using CXRouRouTest.Net.Sockets.Tcp;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using CxRouRou.Attributes;

namespace CXRouRouTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CxByteBufferTest byteBufferTest = new CxByteBufferTest("CxByteBufferTest");
            byteBufferTest.Run();
            //CxNetTest netTest = new CxNetTest("CxNetTest");
            //netTest.Run();

            Console.ReadKey();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CxSolution.CxRouRou.Collections;
using CxSolution.CxRouRou.Util;
using CxSolution.CxRouRouTest.Collections;
using CxSolution.CxRouRouTest.Net.Sockets.Tcp;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using CxSolution.CxRouRou.Attributes;
using System.Net.Sockets;
using System.Net;

namespace CXRouRouTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //CxByteBufferTest byteBufferTest = new CxByteBufferTest("CxByteBufferTest");
            //byteBufferTest.Run();
            CxNetTest netTest = new CxNetTest("CxNetTest");
            netTest.Run();

            //CxConsole.WriteLine("%d", 0x4aa210b4);

            Console.ReadKey();
        }
    }
}

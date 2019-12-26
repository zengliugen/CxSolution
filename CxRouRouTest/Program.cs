using System;
using CxSolution.CxRouRouTest.Net.Sockets.Tcp;

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

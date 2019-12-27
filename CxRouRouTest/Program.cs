using System;

namespace CxSolution.CxRouRouTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Collections.CxByteBufferTest bytebufferTest = new Collections.CxByteBufferTest("CxByteBufferTest");
            //byteBufferTest.Run();
            //Net.Sockets.Tcp.CxNetTest netTest = new Net.Sockets.Tcp.CxNetTest("CxNetTest");
            //netTest.Run();
            //Security.CxRSATest rsaTest = new CxRouRouTest.Security.CxRSATest("CxRSATest");
            //rsaTest.Run();
            Security.CxAESTest aesTest = new Security.CxAESTest("CxAESTest");
            aesTest.Run();

            Console.ReadKey();
        }
    }
}

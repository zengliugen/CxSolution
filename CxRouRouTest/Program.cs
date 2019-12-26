using System;

namespace CxSolution.CXRouRouTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //CxByteBufferTest byteBufferTest = new CxByteBufferTest("CxByteBufferTest");
            //byteBufferTest.Run();
            //CxNetTest netTest = new CxNetTest("CxNetTest");
            //netTest.Run();
            CxRouRouTest.Security.CxRSATest rsaTest = new CxRouRouTest.Security.CxRSATest("CxRSATest");
            rsaTest.Run();

            Console.ReadKey();
        }
    }
}

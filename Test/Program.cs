using System;
using System.Diagnostics;

using CxSolution.CxRouRou.Expand;
using CxSolution.CxRouRou.Security;

namespace CxSolution.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test();
            var AESKey = CxAES.CreateAESKey();
            Console.WriteLine(AESKey.Key.ToHexString());
            Console.WriteLine(AESKey.IV.ToHexString());

            Console.ReadKey();
        }

        public static void Test()
        {
            var testLength = 1024 * 1024 * 100;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            long ivalue = 1;
            long icount = 0;
            for (int i = 0; i < testLength; i++)
            {
                icount += ivalue;
                icount -= ivalue;
            }
            stopwatch.Stop();
            Console.WriteLine("test 1:" + stopwatch.ElapsedTicks);

            stopwatch.Restart();
            int svalue = 1;
            int scount = 0;
            for (int i = 0; i < testLength; i++)
            {
                scount += svalue;
                scount -= svalue;
            }
            stopwatch.Stop();
            Console.WriteLine("test 2:" + stopwatch.ElapsedTicks);
        }
    }
}

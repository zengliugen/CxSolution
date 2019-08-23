using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CxSolution.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();

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

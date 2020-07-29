using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Reflection;
using Newtonsoft.Json;
using System.Threading.Tasks;

using static System.Console;
using System.Threading;
using CxSolution.CxRouRou.Net.Downloads;
using System.Net.Http.Headers;
using CxSolution.CxRouRou.Maths;
using System.Linq;
using System.Text.RegularExpressions;

namespace CxSolution.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int testCount = 1024;
            int width = 512, height = 512;
            var image = new byte[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    image[i, j] = (byte)((i * j + j) % byte.MaxValue);
                }
            }


            WriteLine(Test1(image, width, height, testCount));
            WriteLine(Test2(image, width, height, testCount));
            ReadKey();
        }
        static long Test1(byte[,] image, int width, int height, int testCount)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < testCount; i++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        image[y, x] += 1;
                    }
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
        static long Test2(byte[,] image, int width, int height, int testCount)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < testCount; i++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        image[x, y] += 1;
                    }
                }
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}

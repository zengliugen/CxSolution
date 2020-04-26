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
            WriteLine(CxBaseConversion.Other2PowerToBinary(new UInt64[] { 15, 8 }, 16).Concat());
            ReadKey();
        }
    }
}

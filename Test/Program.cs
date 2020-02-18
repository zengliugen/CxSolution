using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using CxSolution.CxRouRou.Reflection;
using Newtonsoft.Json;
namespace CxSolution.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblyInfo = CxAssembly.GetAssemblyInfo("CxReflectionTest");
            var assemblyJson = JsonConvert.SerializeObject(assemblyInfo);
            Console.WriteLine(assemblyJson);
            Console.ReadKey();
        }
    }
}

using System;
using System.Diagnostics;
using System.Reflection;

using CxSolution.CxRouRou.Reflection;

namespace CxSolution.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.Load("CxReflectionTest");
            Debug.WriteLine(assembly.FullName);
            Console.WriteLine(assembly.FullName);
            var p1 = assembly.GetType("CxSolution.CxReflectionTest.TestBase");
            var p2 = p1.GetField("base_public_int");
            var i1 = assembly.CreateInstance("CxSolution.CxReflectionTest.TestBase");
            var p3 = p2.GetValue(i1);
            Console.WriteLine(p3);
            Debug.WriteLine("------------------");
            CxReflectionTest.TestBase.int_static = 50;
            p2.SetValue(i1, 50);
            var p4 = p2.GetValue(i1);
            Console.WriteLine(p4);

            CxReflectionTest.TestBase.int_static = 20;

            var v1 = CxReflection.GetFieldValue(null, "CxReflectionTest", "CxSolution.CxReflectionTest.TestBase", "int_static");
            Console.WriteLine(v1);



            Console.ReadKey();
        }
    }
}

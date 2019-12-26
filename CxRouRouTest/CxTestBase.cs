using System;

using CxSolution.CxRouRou.Util;

namespace CxSolution.CxRouRouTest
{
    /// <summary>
    /// 测试基类
    /// </summary>
    public abstract class CxTestBase : IDisposable
    {
        /// <summary>
        /// 测试名称
        /// </summary>
        public string TestName;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="testName"></param>
        public CxTestBase(string testName)
        {
            TestName = testName;
        }
        /// <summary>
        /// 测试内容
        /// </summary>
        /// <returns></returns>
        public abstract bool Test();
        /// <summary>
        /// 控制台输出
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected static void PrintL(string format, params object[] args)
        {
            CxConsole.WriteLine(format, args);
        }
        /// <summary>
        /// 运行测试
        /// </summary>
        public void Run()
        {
            if (Test())
            {
                PrintL("测试:{0} 通过", TestName);
            }
            else
            {
                PrintL("测试:{0} 未通过", TestName);
            }
        }
        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public abstract void Dispose();
    }
}

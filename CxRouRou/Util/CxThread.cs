using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CxRouRou.Util
{
    /// <summary>
    /// 线程操作
    /// </summary>
    public static class CxThread
    {
        /// <summary>
        /// 开始一个线程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="useThreadPool"></param>
        /// <returns></returns>
        public static Thread StartThread(Action handler, bool useThreadPool = true)
        {
            Thread thread = null;
            if (handler == null)
            {
                return thread;
            }
            if (useThreadPool)//线程池
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    handler.Invoke();
                });
            }
            else//立即线程
            {
                thread = new Thread(handler.Invoke);
                thread.Start();
            }
            return thread;
        }
    }
}

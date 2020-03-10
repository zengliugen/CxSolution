using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Net.Downloads
{
    /// <summary>
    /// 错误码 1-10000 致命错误 10001-20000 提示性错误
    /// </summary>
    public static class __ErrorCode
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        public static int Success = 0;
    }
    /// <summary>
    /// 错误码信息
    /// </summary>
    public static class __ErrorCodeMsg
    {
        /// <summary>
        /// 错误码信息表
        /// </summary>
        private static Dictionary<int, string> __MSG = new Dictionary<int, string>
        {
            {0,"操作成功" },
        };
        /// <summary>
        /// 获取错误码对应的信息
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static string GetMsg(int errorCode)
        {
            if (!__MSG.TryGetValue(errorCode, out string msg))
            {
                msg = "";
            }
            return msg;
        }
    }
}

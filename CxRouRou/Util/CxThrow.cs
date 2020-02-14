using System;
using System.Collections.Generic;
using System.Text;
using CxSolution.CxRouRou.Exceptions;

namespace CxSolution.CxRouRou.Util
{
    /// <summary>
    /// 异常抛出工具
    /// </summary>
    public static class CxThrow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isError"></param>
        public static void ArgumentNullException(bool isError)
        {
            if (isError)
                throw new ArgumentNullException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="paramName"></param>
        public static void ArgumentNullException(bool isError, string paramName)
        {
            if (isError)
                throw new ArgumentNullException(paramName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public static void ArgumentNullException(bool isError, string message, Exception innerException)
        {
            if (isError)
                throw new ArgumentNullException(message, innerException);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        public static void ArgumentNullException(bool isError, string paramName, string message)
        {
            if (isError)
                throw new ArgumentNullException(paramName, message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isError"></param>
        public static void ArgumentNullOrEmptyException(bool isError)
        {
            if (isError)
                throw new ArgumentNullOrEmptyException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="paramName"></param>
        public static void ArgumentNullOrEmptyException(bool isError, string paramName)
        {
            if (isError)
                throw new ArgumentNullOrEmptyException(paramName);
        }
    }
}

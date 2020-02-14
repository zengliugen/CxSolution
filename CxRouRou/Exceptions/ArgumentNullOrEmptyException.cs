using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CxSolution.CxRouRou.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ArgumentNullOrEmptyException : ArgumentException
    {
        /// <summary>
        /// 
        /// </summary>
        public ArgumentNullOrEmptyException() : base()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        public ArgumentNullOrEmptyException(string paramName) : base("Value cannot be null or empty.", paramName)
        {

        }
    }
}

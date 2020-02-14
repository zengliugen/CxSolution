using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Throw = CxSolution.CxRouRou.Util.CxThrow;

namespace CxSolution.CxRouRou.Reflection
{
    /// <summary>
    /// 反射
    /// </summary>
    public class CxReflection
    {
        /// <summary>
        /// 默认绑定标志
        /// </summary>
        public const BindingFlags DefaultBindingFlag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="instance">Type对应的实例对象,static的属性可以传NULL</param>
        /// <param name="assembly">程序集</param>
        /// <param name="typeFullName">Type全部名称</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="bindingFlags">绑定标志</param>
        /// <returns></returns>
        public static object GetProperty(object instance, Assembly assembly, string typeFullName, string propertyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            Throw.ArgumentNullException(assembly == null, "assembly");
            Throw.ArgumentNullOrEmptyException(string.IsNullOrEmpty(typeFullName), "typeFullName");
            Throw.ArgumentNullOrEmptyException(string.IsNullOrEmpty(propertyName), "propertyName");
            var type = assembly.GetType(typeFullName, false);
            if (type == null)
                return null;
            var property = type.GetProperty(propertyName, bindingFlags);
            if (property == null)
                return null;
            var value = property.GetValue(instance);
            return value;
        }
        public static void SetProperty()
        {

        }
        public static object GetField()
        {
            return null;
        }
        public static void SetField()
        {

        }
        public static void AddEvent()
        {

        }
        public static void RemoveEvent()
        {

        }
        public static object CallMethod()
        {
            return null;
        }
    }
}

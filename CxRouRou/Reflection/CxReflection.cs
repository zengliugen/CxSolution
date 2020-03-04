using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CxSolution.CxRouRou.Exceptions;

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
        /// 获取程序集(内存)
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullOrEmptyException("assemblyName");
            }
            var assembly = Assembly.Load(assemblyName);
            return assembly;
        }
        /// <summary>
        /// 获取属性信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="propertyName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(Assembly assembly, string typeFullName, string propertyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullOrEmptyException("typeFullName");
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullOrEmptyException("propertyName");
            }
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            var propertyInfo = type.GetProperty(propertyName, bindingFlags);
            return propertyInfo;
        }
        /// <summary>
        /// 获取属性信息
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="propertyName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(string assemblyName, string typeFullName, string propertyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullException("assemblyName");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullException("typeFullName");
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            var assembly = GetAssembly(assemblyName);
            if (assembly == null) return null;
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            var propertyInfo = type.GetProperty(propertyName, bindingFlags);
            return propertyInfo;
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="propertyName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object instance, Assembly assembly, string typeFullName, string propertyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var propertyInfo = GetPropertyInfo(assembly, typeFullName, propertyName, bindingFlags);
            if (propertyInfo == null) return null;
            var value = propertyInfo.GetValue(instance);
            return value;
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="propertyName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object instance, string assemblyName, string typeFullName, string propertyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var propertyInfo = GetPropertyInfo(assemblyName, typeFullName, propertyName, bindingFlags);
            if (propertyInfo == null) return null;
            var value = propertyInfo.GetValue(instance);
            return value;
        }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="propertyName"></param>
        /// <param name="bindingFlags"></param>
        public static void SetPropertyValue(object instance, object value, Assembly assembly, string typeFullName, string propertyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var propertyInfo = GetPropertyInfo(assembly, typeFullName, propertyName, bindingFlags);
            if (propertyInfo == null) return;
            propertyInfo.SetValue(instance, value);
        }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="propertyName"></param>
        /// <param name="bindingFlags"></param>
        public static void SetPropertyValue(object instance, object value, string assemblyName, string typeFullName, string propertyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var propertyInfo = GetPropertyInfo(assemblyName, typeFullName, propertyName, bindingFlags);
            if (propertyInfo == null) return;
            propertyInfo.SetValue(instance, value);
        }
        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="fieldName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(Assembly assembly, string typeFullName, string fieldName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullException("typeFullName");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            var fieldInfo = type.GetField(fieldName, bindingFlags);
            return fieldInfo;
        }
        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="fieldName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(string assemblyName, string typeFullName, string fieldName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullException("assemblyName");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullException("typeFullName");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            var assembly = GetAssembly(assemblyName);
            if (assembly == null) return null;
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            var fieldInfo = type.GetField(fieldName, bindingFlags);
            return fieldInfo;
        }
        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="fieldName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static object GetFieldValue(object instance, Assembly assembly, string typeFullName, string fieldName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var fieldInfo = GetFieldInfo(assembly, typeFullName, fieldName, bindingFlags);
            if (fieldInfo == null) return null;
            var value = fieldInfo.GetValue(instance);
            return value;
        }
        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="fieldName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static object GetFieldValue(object instance, string assemblyName, string typeFullName, string fieldName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var fieldInfo = GetFieldInfo(assemblyName, typeFullName, fieldName, bindingFlags);
            if (fieldInfo == null) return null;
            var value = fieldInfo.GetValue(instance);
            return value;
        }
        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="fieldName"></param>
        /// <param name="bindingFlags"></param>
        public static void SetFieldValue(object instance, object value, Assembly assembly, string typeFullName, string fieldName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var fieldInfo = GetFieldInfo(assembly, typeFullName, fieldName, bindingFlags);
            if (fieldInfo == null) return;
            fieldInfo.SetValue(instance, value);
        }
        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="fieldName"></param>
        /// <param name="bindingFlags"></param>
        public static void SetFieldValue(object instance, object value, string assemblyName, string typeFullName, string fieldName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var fieldInfo = GetFieldInfo(assemblyName, typeFullName, fieldName, bindingFlags);
            if (fieldInfo == null) return;
            fieldInfo.SetValue(instance, value);
        }
        /// <summary>
        /// 获取事件信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="eventName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static EventInfo GetEventInfo(Assembly assembly, string typeFullName, string eventName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullOrEmptyException("typeFullName");
            }
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullOrEmptyException("eventName");
            }
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            var eventInfo = type.GetEvent(eventName, bindingFlags);
            return eventInfo;
        }
        /// <summary>
        /// 获取事件信息
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="eventName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static EventInfo GetEventInfo(string assemblyName, string typeFullName, string eventName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullOrEmptyException("assemblyName");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullOrEmptyException("typeFullName");
            }
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullOrEmptyException("eventName");
            }
            var assembly = GetAssembly(assemblyName);
            if (assembly == null) return null;
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            var eventInfo = type.GetEvent(eventName, bindingFlags);
            return eventInfo;
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="eventName"></param>
        /// <param name="bindingFlags"></param>
        public static void AddEvent(object instance, Delegate value, Assembly assembly, string typeFullName, string eventName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var eventInfo = GetEventInfo(assembly, typeFullName, eventName, bindingFlags);
            if (eventInfo == null) return;
            eventInfo.AddEventHandler(instance, value);
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="eventName"></param>
        /// <param name="bindingFlags"></param>
        public static void AddEvent(object instance, Delegate value, string assemblyName, string typeFullName, string eventName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var eventInfo = GetEventInfo(assemblyName, typeFullName, eventName, bindingFlags);
            if (eventInfo == null) return;
            eventInfo.AddEventHandler(instance, value);
        }
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="eventName"></param>
        /// <param name="bindingFlags"></param>
        public static void RemoveEvent(object instance, Delegate value, Assembly assembly, string typeFullName, string eventName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var eventInfo = GetEventInfo(assembly, typeFullName, eventName, bindingFlags);
            if (eventInfo == null) return;
            eventInfo.RemoveEventHandler(instance, value);
        }
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="eventName"></param>
        /// <param name="bindingFlags"></param>
        public static void RemoveEvent(object instance, Delegate value, string assemblyName, string typeFullName, string eventName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var eventInfo = GetEventInfo(assemblyName, typeFullName, eventName, bindingFlags);
            if (eventInfo == null) return;
            eventInfo.RemoveEventHandler(instance, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="binder"></param>
        /// <param name="types"></param>
        /// <param name="parameterModifiers"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Assembly assembly, string typeFullName, string methodName, BindingFlags bindingFlags = DefaultBindingFlag, Binder binder = null, Type[] types = null, ParameterModifier[] parameterModifiers = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullOrEmptyException("assembly");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullOrEmptyException("typeFullName");
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentNullOrEmptyException("methodName");
            }
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            MethodInfo methodInfo = null;
            if (binder == null && types == null && parameterModifiers == null)
                methodInfo = type.GetMethod(methodName, bindingFlags);
            else
                methodInfo = type.GetMethod(methodName, bindingFlags, binder, types, parameterModifiers);
            return methodInfo;
        }
        /// <summary>
        /// 获取函数信息
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="binder"></param>
        /// <param name="types"></param>
        /// <param name="parameterModifiers"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(string assemblyName, string typeFullName, string methodName, BindingFlags bindingFlags = DefaultBindingFlag, Binder binder = null, Type[] types = null, ParameterModifier[] parameterModifiers = null)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullOrEmptyException("assemblyName");
            }
            if (string.IsNullOrEmpty(typeFullName))
            {
                throw new ArgumentNullOrEmptyException("typeFullName");
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentNullOrEmptyException("methodName");
            }
            var assembly = GetAssembly(assemblyName);
            if (assembly == null) return null;
            var type = assembly.GetType(typeFullName, false);
            if (type == null) return null;
            MethodInfo methodInfo = null;
            if (binder == null && types == null && parameterModifiers == null)
                methodInfo = type.GetMethod(methodName, bindingFlags);
            else
                methodInfo = type.GetMethod(methodName, bindingFlags, binder, types, parameterModifiers);
            return methodInfo;
        }
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="agrs"></param>
        /// <param name="assembly"></param>
        /// <param name="typeFullName"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static object CallMethod(object instance, object[] agrs, Assembly assembly, string typeFullName, string methodName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            MethodInfo methodInfo = null;
            if (agrs == null || agrs.Length == 0)
            {
                methodInfo = GetMethodInfo(assembly, typeFullName, methodName, bindingFlags);
            }
            else
            {
                var types = new Type[agrs.Length];
                for (int i = 0; i < agrs.Length; i++)
                {
                    types[i] = agrs.GetType();
                }
                methodInfo = GetMethodInfo(assembly, typeFullName, methodName, bindingFlags, null, types, null);
            }
            if (methodInfo == null) return null;
            object returnValue = methodInfo.Invoke(instance, agrs);
            return returnValue;
        }
        /// <summary>
        /// 调用函数
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="agrs"></param>
        /// <param name="assemblyName"></param>
        /// <param name="typeFullName"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static object CallMethod(object instance, object[] agrs, string assemblyName, string typeFullName, string methodName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            MethodInfo methodInfo = null;
            if (agrs == null || agrs.Length == 0)
            {
                methodInfo = GetMethodInfo(assemblyName, typeFullName, methodName, bindingFlags);
            }
            else
            {
                var types = new Type[agrs.Length];
                for (int i = 0; i < agrs.Length; i++)
                {
                    types[i] = agrs.GetType();
                }
                methodInfo = GetMethodInfo(assemblyName, typeFullName, methodName, bindingFlags, null, types, null);
            }
            if (methodInfo == null) return null;
            object returnValue = methodInfo.Invoke(instance, agrs);
            return returnValue;
        }
    }
}

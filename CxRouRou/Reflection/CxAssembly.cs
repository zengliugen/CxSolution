using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using CxSolution.CxRouRou.Expands;

using Throw = CxSolution.CxRouRou.Util.CxThrow;

namespace CxSolution.CxRouRou.Reflection
{
    /// <summary>
    /// 反射
    /// </summary>
    public class CxAssembly
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
            Throw.ArgumentNullOrEmptyException(string.IsNullOrEmpty(assemblyName), "assemblyName");
            var assembly = Assembly.Load(assemblyName);
            return assembly;
        }
        /// <summary>
        /// 获取程序集信息
        /// </summary>
        /// <param name="assembly"></param>
        public static string GetAssemblyInfo(Assembly assembly)
        {
            if (assembly == null)
            {
                return "Assembly is NULL";
            }
            var infoSb = new StringBuilder();
            infoSb.AppendFormatLine("Assembly:{0}", assembly.FullName);
            try
            {
                var types = assembly.GetTypes();
                infoSb.AppendFormatLine("Types Count:{0}", types.Length);
                foreach (var type in types)
                {
                    infoSb.AppendFormatLine(" Type:{0}", type.FullName);
                    try
                    {
                        //属性
                        var propertyInfos = type.GetProperties();
                        infoSb.AppendFormatLine("  Properties Count:{0}", propertyInfos.Length);
                        foreach (var propertyInfo in propertyInfos)
                        {
                            infoSb.AppendFormatLine("   PropertyInfo:{0}", propertyInfo.Name);
                        }
                    }
                    catch (Exception getPropertiesError)
                    {
                        infoSb.AppendFormatLine("  GetProperties Error:{0}", getPropertiesError.Message);
                    }
                    try
                    {
                        //字段
                        var fieldInfos = type.GetFields();
                        infoSb.AppendFormatLine("  Fields Count:{0}", fieldInfos.Length);
                        foreach (var fieldInfo in fieldInfos)
                        {
                            infoSb.AppendFormatLine("   FieldInfo:{0}", fieldInfo.Name);
                        }
                    }
                    catch (Exception getFieldsError)
                    {
                        infoSb.AppendFormatLine("  GetFieldsError Error:{0}", getFieldsError.Message);
                    }
                    try
                    {
                        //事件
                        var eventInfos = type.GetEvents();
                        infoSb.AppendFormatLine("  Events Count:{0}", eventInfos.Length);
                        foreach (var eventInfo in eventInfos)
                        {
                            infoSb.AppendFormatLine("   EventInfo:{0}", eventInfo.Name);
                        }
                    }
                    catch (Exception getEventsError)
                    {
                        infoSb.AppendFormatLine("  GetEventsError Error:{0}", getEventsError.Message);
                    }
                    try
                    {
                        //函数
                        var methodInfos = type.GetMethods();
                        infoSb.AppendFormatLine("  Methods Count:{0}", methodInfos.Length);
                        foreach (var methodInfo in methodInfos)
                        {
                            infoSb.AppendFormatLine("   MethodInfo:{0}", methodInfo.Name);
                        }
                    }
                    catch (Exception getMethodsError)
                    {
                        infoSb.AppendFormatLine("  GetMethods Error:{0}", getMethodsError.Message);
                    }
                }
            }
            catch (Exception getTypesError)
            {
                infoSb.AppendFormatLine("GetTypes Error:{0}", getTypesError.Message);
            }
            return infoSb.ToString();
        }
        /// <summary>
        /// 获取程序集信息
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static CxAssemblyInfo GetAssemblyInfo(string assemblyName, BindingFlags bindingFlags = DefaultBindingFlag)
        {
            var assembly = GetAssembly(assemblyName);
            if (assembly == null) return null;
            CxAssemblyInfo assemblyInfo = new CxAssemblyInfo
            {
                Name = assembly.FullName,
            };
            var types = assembly.GetTypes();
            if (types == null) return assemblyInfo;
            assemblyInfo.TypeInfoList = new List<CxTypeInfo>(types.Length);
            foreach (var type in types)
            {
                var typeInfo = new CxTypeInfo
                {
                    Name = type.FullName,
                };
                var properties = type.GetProperties(bindingFlags);
                if (properties != null)
                {
                    typeInfo.PropertyInfoList = new List<CxPropertyInfo>(properties.Length);
                    foreach (var property in properties)
                    {
                        var propertyInfo = new CxPropertyInfo
                        {
                            Name = property.Name,
                            TypeName = property.PropertyType.FullName,
                        };
                        typeInfo.PropertyInfoList.Add(propertyInfo);
                    }
                }
                var fields = type.GetFields(bindingFlags);
                if (fields != null)
                {
                    typeInfo.FieldInfoList = new List<CxFieldInfo>(fields.Length);
                    foreach (var field in fields)
                    {
                        var fieldInfo = new CxFieldInfo
                        {
                            Name = field.Name,
                            TypeName = field.FieldType.FullName,
                        };
                        typeInfo.FieldInfoList.Add(fieldInfo);
                    }
                }
                var events = type.GetEvents(bindingFlags);
                if (events != null)
                {
                    typeInfo.EventInfoList = new List<CxEventInfo>(events.Length);
                    foreach (var _event in events)
                    {
                        var eventInfo = new CxEventInfo()
                        {
                            Name = _event.Name,
                            TypeName = _event.EventHandlerType.FullName,
                        };
                    }
                }
                var methods = type.GetMethods();
                if (methods != null)
                {
                    typeInfo.MethodInfoList = new List<CxMethodInfo>(methods.Length);
                    foreach (var method in methods)
                    {
                        var methodInfo = new CxMethodInfo()
                        {
                            Name = method.Name,
                            ReturnTypeName = method.ReturnType.FullName,
                        };
                        var parameters = method.GetParameters();
                        if (parameters != null)
                        {
                            methodInfo.ParameterTypeNameList = new List<string>();
                            foreach (var parameter in parameters)
                            {
                                methodInfo.ParameterTypeNameList.Add(parameter.ParameterType.FullName);
                            }
                        }
                        typeInfo.MethodInfoList.Add(methodInfo);
                    }
                }
                assemblyInfo.TypeInfoList.Add(typeInfo);
            }
            return assemblyInfo;
        }
    }
    /// <summary>
    /// 程序集信息
    /// </summary>
    public class CxAssemblyInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 程序集下的类型信息列表
        /// </summary>
        public List<CxTypeInfo> TypeInfoList;
    }
    /// <summary>
    /// 类型信息
    /// </summary>
    public class CxTypeInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型下的属性信息列表
        /// </summary>
        public List<CxPropertyInfo> PropertyInfoList;
        /// <summary>
        /// 类型下的字段信息列表
        /// </summary>
        public List<CxFieldInfo> FieldInfoList;
        /// <summary>
        /// 类型下的事件信息列表
        /// </summary>
        public List<CxEventInfo> EventInfoList;
        /// <summary>
        /// 类型下的函数信息列表
        /// </summary>
        public List<CxMethodInfo> MethodInfoList;
    }
    /// <summary>
    /// 属性信息
    /// </summary>
    public class CxPropertyInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName;
    }
    /// <summary>
    /// 字段信息
    /// </summary>
    public class CxFieldInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName;
    }
    /// <summary>
    /// 事件信息
    /// </summary>
    public class CxEventInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName;
    }
    /// <summary>
    /// 函数信息
    /// </summary>
    public class CxMethodInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 参数类型名称列表
        /// </summary>
        public List<string> ParameterTypeNameList;
        /// <summary>
        /// 返回类型名称
        /// </summary>
        public string ReturnTypeName;
    }
}

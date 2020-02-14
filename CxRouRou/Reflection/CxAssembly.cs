using System;
using System.Reflection;
using System.Text;

using CxSolution.CxRouRou.Expands;

namespace CxSolution.CxRouRou.Reflection
{
    /// <summary>
    /// 反射
    /// </summary>
    public class CxAssembly
    {
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
    }
}

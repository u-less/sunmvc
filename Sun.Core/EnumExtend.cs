using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace Sun.Core
{
    /// <summary>
    /// 枚举操作拓展类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EnumExtend<T>
    {
        /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>value+name</returns>
        public static Dictionary<int, string> GetEnumKeyName()
        {
            Type enumType = typeof(T);
            Dictionary<int, string> dic = new Dictionary<int, string>();
            Type typeDescription = typeof(DescriptionAttribute);
            FieldInfo[] fields = enumType.GetFields();
            int key =0;
            string name = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute des = (DescriptionAttribute)arr[0];
                        name = des.Description;
                        T t = (T)Enum.Parse(enumType, field.Name, true);
                        key = Convert.ToInt32(t);
                    }
                    else
                    {
                        name = "未知";
                        T t = (T)Enum.Parse(enumType, field.Name, true);
                        key = Convert.ToInt32(t);
                    }
                    dic.Add(key, name);
                }
            }
            return dic;
        }
    }
}

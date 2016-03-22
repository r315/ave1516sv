using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

// to get private or protected fields, use FieldInfo

namespace Jsonzai.Reflect
{
    public class Jsonfier
    {
        private static String getPrimitiveField(object srcObj)
        {
            if (srcObj == null) return "\0";
            String jsonString = "{";
            Type klass = srcObj.GetType();
            PropertyInfo[] props = klass.GetProperties(
                BindingFlags.Public |
                //BindingFlags.NonPublic |
                BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {                
                object val = prop.GetGetMethod().Invoke(srcObj, new object[0]);
                if (val == null) return "";

                Type propType = prop.PropertyType;
                String key = "\"" + prop.Name + "\": ";
                String value;

                if (propType.IsPrimitive)
                    value = val.ToString() + ", ";
                else if (propType == typeof(String))
                    value = "\"" + val.ToString() + "\", ";
                else
                    value = getMembers(val);

                if (value != null)
                    jsonString += key + value;
            }
            return jsonString + "}";
        }

        private static String getMembers(object srcObj)
        {
            if (srcObj == null) return "\0";
            String jsonString = "{";
            Type klass = srcObj.GetType();
            PropertyInfo[] props = klass.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                object val = prop.GetGetMethod().Invoke(srcObj, new object[0]);
                if (val == null) return "";

                Type propType = prop.PropertyType;
                String key = "\"" + prop.Name + "\": ";
                String value = "";

                if (propType.IsArray)
                {
                    Array data = prop.GetValue(srcObj) as Array;
                    foreach (var element in data)
                        value += element.ToString();
                }
                else {

                    if (propType.IsPrimitive)
                        value = val.ToString() + ", ";
                    else if (propType == typeof(String))
                        value = "\"" + val.ToString() + "\", ";
                    else
                        value = getMembers(val);
                }
                if (value != null)
                    jsonString += key + value;
            }
            return jsonString + "}";
        }



        public static string ToJson(object srcObj)
        {                        
            if (srcObj == null) return null;
            String tmp = getMembers(srcObj);
            System.Diagnostics.Debug.Write(tmp);
            return tmp;
            
        }
    }

}

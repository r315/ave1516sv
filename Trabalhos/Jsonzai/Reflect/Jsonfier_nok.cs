using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

// to get private or protected fields, use FieldInfo

namespace Jsonzai.Reflect
{

    public class Jsonfier
    {
        private static String ToJsonProperties(object srcObj)
        {
            StringBuilder json = new StringBuilder();
            Type objType = srcObj.GetType();

            PropertyInfo[] props = objType.GetProperties(
              BindingFlags.Public |
              BindingFlags.NonPublic |
              BindingFlags.Instance);

            //TODO: invoke getMethod???
            foreach (PropertyInfo prop in props)
            {
                json.Append("\"" + prop.Name + "\":");
                json.Append(ToJson(prop.GetValue(srcObj)) + ',');
            }
            return json.ToString();
        }

        private static String ToJsonFields(object srcObj)
        {
            StringBuilder json = new StringBuilder();
            Type objType = srcObj.GetType();
            FieldInfo[] fields = objType.GetFields(
               BindingFlags.Public |
               BindingFlags.NonPublic |
               BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                json.Append("\"" + field.Name + "\":");
                json.Append(ToJson(field.GetValue(srcObj)) + ',');
            }
            return json.ToString();
        }

        private static String ToJsonMethods(object srcObj)
        {
            StringBuilder json = new StringBuilder();
            Type objType = srcObj.GetType();

            MethodInfo[] methods = objType.GetMethods(
               BindingFlags.Public |
               BindingFlags.NonPublic |
               BindingFlags.Instance);

            //TODO: filter methods for return type
            foreach (MethodInfo method in methods)
            {
                json.Append("\"" + method.Name + "\":");
                json.Append(ToJson(method.ReturnType.ToString()) + ',');
            }
            return json.ToString();
        }

        public static string ToJson(object srcObj)
        {
            if (srcObj == null) return "null";

            Type objtype = srcObj.GetType();

            if (objtype.IsPrimitive)
            {
                if (objtype == typeof(Char))
                {
                    return "\"" + srcObj.ToString() + "\"";
                }

                if (objtype == typeof(Boolean))
                {
                    return ((Boolean)srcObj).ToString().ToLower();
                }

                return srcObj.ToString();
            }

            if (objtype == typeof(String))
                return "\"" + srcObj.ToString() + "\"";

            if (srcObj is IEnumerable)
            {
                StringBuilder arr = new StringBuilder("[");
                foreach (object obj in (IEnumerable)srcObj)
                    arr.Append(ToJson(obj) + ',');
                arr.Remove(arr.ToString().LastIndexOf(','), 1);
                arr.Append("]");
                return arr.ToString();
            }

            //object is not a built-in type
            StringBuilder json = new StringBuilder("{");

            json.Append(ToJsonFields(srcObj));
            json.Append(ToJsonProperties(srcObj));            

            json.Remove(json.ToString().LastIndexOf(','), 1);
            json.Append('}');

            return json.ToString();
        }
    }

}


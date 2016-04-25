using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Reflect
{
    public class Jsonfier
    {
        private static readonly string quote = "\"";
        private static readonly string iniBracket = "{";
        private static readonly string endBracket = "}";
        private static readonly string iniSquareBracket = "[";
        private static readonly string endSquareBracket = "]";
        private static readonly BindingFlags bindFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
        private static Dictionary<char, Func<MemberInfo, Boolean>> filters = new Dictionary<char, Func<MemberInfo, Boolean>>();
        private static Func<MemberInfo, Boolean> filterByMethods = (x) => x is MethodInfo && !(((MethodInfo)x).ReturnType.Name.Equals("void")) && ((MethodInfo)x).GetParameters().Length==0;
        private static Func<MemberInfo, Boolean> filterByFields = (x) => x is FieldInfo;
        private static Func<MemberInfo, Boolean> filterByProperties = (x) => x is PropertyInfo;
        private static Func<MemberInfo, Boolean> filter = (x) => filterByFields(x) || filterByProperties(x) || filterByMethods(x);

        public static string ToJson(object src)
        {
            if (src == null)
            {
                return "null";
            }
            
            Type t = src.GetType();

            if (IsBuiltInType(t))
            {
                if (t.Name.Equals("Char") || t.Name.Equals("String") || t.Name.Equals("DateTime"))
                {
                    return EncloseString(src.ToString(), quote, quote);
                }
                else{
                    return src.ToString().ToLower().Replace(',','.');
                } 
            }
            else if(src is IEnumerable)
            {
                StringBuilder sb = new StringBuilder();
            
                foreach (object item in (IEnumerable)src)
                {
                    sb.Append(Jsonfier.ToJson(item) + ",");
                }

                sb.Remove(sb.Length - 1, 1);

                return EncloseString(sb.ToString(), iniSquareBracket, endSquareBracket);
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                MemberInfo[] members = t.GetMembers(bindFlags).Where(filter).ToArray();

                foreach (MemberInfo item in members)
                {
                    sb.Append(EncloseString(item.Name.Replace("get_", ""), quote, quote) + ":");

                    if (item is PropertyInfo)
                    {
                        sb.Append(Jsonfier.ToJson(((PropertyInfo)item).GetValue(src)));
                    }
                    else if (item is FieldInfo)
                    {
                        sb.Append(Jsonfier.ToJson(((FieldInfo)item).GetValue(src)));
                    }
                    else if (item is MethodInfo && ((MethodInfo)item).GetParameters().Length == 0)
                    {
                        sb.Append(Jsonfier.ToJson(((MethodInfo)item).Invoke(src, new object[0])));
                    }
                    sb.Append(",");
                }

                sb.Remove(sb.Length - 1, 1);

                return EncloseString(sb.ToString(), iniBracket, endBracket);
            }
        }

        private static bool IsBuiltInType(Type t)
        {
            return t.IsPrimitive || t.Name.Equals("String") || t.Name.Equals("DateTime");
        }

        public static string ToJson(object src, string criteriaFlags)
        {
            InitFilterDictionary();

            Func<MemberInfo, Boolean> defaultFilter = filter;

            if (criteriaFlags!= null && !criteriaFlags.Equals(""))
            {
                CreateCustomFilter(criteriaFlags);
            }

            string json = ToJson(src);

            filter = defaultFilter;

            return json;
        }

        private static void CreateCustomFilter(string criteriaFlags)
        {
            Func<MemberInfo, Boolean> tempFilter;
            int processedFlags = 0;

            foreach (char c in criteriaFlags.ToLower())
            {
                if (filters.TryGetValue(c, out tempFilter)){
                    if (processedFlags == 0)
                    {
                        filter = tempFilter;
                    }
                    else
                    {
                        filter = CombineFunc(filter, tempFilter);
                    }
                    processedFlags++;
                }
            }
        }

        private static void  InitFilterDictionary ()
        {
            if (filters.Count == 0)
            {
                filters.Add('m', filterByMethods);
                filters.Add('p', filterByProperties);
                filters.Add('f', filterByFields);
            }
        }

        private static string EncloseString(String s, String iniClose, String endClose)
        {
            return String.Format("{0}{1}{2}", iniClose, s, endClose);
        }

        private static Func<MemberInfo, Boolean> CombineFunc(Func<MemberInfo, Boolean> f1, Func<MemberInfo, Boolean> f2)
        {
           return (x) => f1(x) || f2(x);
        }
    }
}

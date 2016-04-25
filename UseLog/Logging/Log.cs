using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Logging
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class DontLogAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class LogLevelAttribute : Attribute
    {
        public int Level { get; private set; }
        public LogLevelAttribute(int level) { Level = level; }
    }

    public interface ILogger
    {
        void Log(object obj, int reqLevel);
    }

    public class Logger
    {
        public static void Log(object obj, int reqLevel)
        {
            if (obj != null)
            {
                GetLogger(obj.GetType()).Log(obj, reqLevel);
            }
        }
        
        private static ILogger GetLogger(Type type)
        {
            //return new ReflectionLogger(type);
            // OR
            return LoggerEmitter.CreateFor(type);
        }
    }

    public static class LogOps
    {
        public const BindingFlags PUBLIC_INSTANCE = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        public static bool IsLogAllowed(MemberInfo m)
        {
            return !m.IsDefined(typeof(DontLogAttribute));
        }
        
        public static int GetLogLevel(MemberInfo m)
        {
            Attribute[] attrs = (Attribute[])m.GetCustomAttributes(typeof(LogLevelAttribute), true);
            return attrs.Length > 0 ? ((LogLevelAttribute)attrs[0]).Level : -1;
        }
    }
}

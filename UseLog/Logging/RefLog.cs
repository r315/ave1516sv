using System;
using System.Reflection;

namespace Logging
{
    public class ReflectionLogger : ILogger
    {
        private Type type;
        private string name;
        
        public ReflectionLogger(Type type)
        {
            this.type = type;
            this.name = type.Name;
        }

        public void Log(object obj, int reqLevel)
        {
            Console.Write("{0} {{ ", name);
            foreach (FieldInfo f in type.GetFields(LogOps.PUBLIC_INSTANCE))
            {
                if (LogOps.IsLogAllowed(f)) {
                    int defLevel = LogOps.GetLogLevel(f);
                    if (defLevel <= reqLevel) {
                        Console.Write("{0}: {1}; ", f.Name, f.GetValue(obj));
                    }
                }
            }
            Console.WriteLine('}');
        }
    }
}

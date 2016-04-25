using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Logging
{
    public class LoggerEmitter
    {
        public static ILogger CreateFor(Type type)
        {
            string TheName = "LoggerFor" + type.Name;

            string ASM_NAME = TheName;
            string MOD_NAME = TheName;
            string TYP_NAME = TheName;
            
            string DLL_NAME = TheName + ".dll";

            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ASM_NAME), AssemblyBuilderAccess.RunAndSave);
                
            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(MOD_NAME, DLL_NAME);
                
            TypeBuilder typBuilder = modBuilder.DefineType(TYP_NAME);
            typBuilder.AddInterfaceImplementation(typeof(ILogger));
                
            MethodBuilder LogMethodBuilder =
                typBuilder.DefineMethod(
                    "Log",
                    MethodAttributes.Public  |
                    MethodAttributes.Virtual |
                    MethodAttributes.ReuseSlot,
                    null,
                    new Type[2] { typeof(object), typeof(int) }
                );

            ImplementLogMethod(LogMethodBuilder, type);

            Type loggerType = typBuilder.CreateType();
            asmBuilder.Save(DLL_NAME);

            return (ILogger)Activator.CreateInstance(loggerType);
        }
            
        private static void ImplementLogMethod(MethodBuilder metBuilder, Type type)
        {
            ILGenerator il = metBuilder.GetILGenerator();
            LocalBuilder tobj = il.DeclareLocal(type);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc, tobj);
            
            il.Emit(OpCodes.Ldstr, type.Name + " { ");
            il.Emit(OpCodes.Call, typeof(System.Console).GetMethod("Write", new Type[] { typeof(string) }));

            foreach (FieldInfo f in type.GetFields(LogOps.PUBLIC_INSTANCE))
            {
                if (LogOps.IsLogAllowed(f)) {
                    int defLevel = LogOps.GetLogLevel(f);
                    Label noLog = il.DefineLabel();
                    if (defLevel > 0) {
                        il.Emit(OpCodes.Ldc_I4, defLevel);
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Cgt);
                        il.Emit(OpCodes.Brtrue, noLog);
                    }
                    il.Emit(OpCodes.Ldstr, f.Name + ": {0}; ");
                    il.Emit(OpCodes.Ldloc, tobj);
                    il.Emit(OpCodes.Ldfld, f);
                    if (f.FieldType.IsValueType) {
                        il.Emit(OpCodes.Box, f.FieldType);
                    }
                    il.Emit(OpCodes.Call, typeof(System.Console).GetMethod("Write", new Type[] { typeof(string), typeof(object) }));
                    il.MarkLabel(noLog);
                }
            }

            il.Emit(OpCodes.Ldc_I4_S, '}');
            il.Emit(OpCodes.Call, typeof(System.Console).GetMethod("WriteLine", new Type[] { typeof(char) }));

            il.Emit(OpCodes.Ret);
        }
    }
}

﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Instr
{



    public interface IJson
    {
        string ToJson(object obj);
    }

    public class Jsoninstr
    {
        private static Dictionary<Type, IJson> serializers = new Dictionary<Type, IJson>();

        // usar um Dictionaty<Object, Object>

        public static string ToJson(object obj)
        {
            if (obj == null)
            {
                return "null";
            }

            return GetSerializer(obj.GetType()).ToJson(obj);

            //throw new NotImplementedException();
        }

        private static IJson GetSerializer(Type type)
        {
            IJson serializer;

            if (serializers.TryGetValue(type, out serializer))
            {
                return serializer;
            }
            else
            {
                return CreateSerializer(type);
            }
        }

        private static IJson LoadSerializer(string dllPath)
        {
            Assembly asSerializer = Assembly.LoadFrom(dllPath);
            return (IJson)asSerializer.CreateInstance(asSerializer.GetType().AssemblyQualifiedName);
        }

        private static IJson CreateSerializer(Type type)
        {

            string dllPath = null;

            string SerialName = "SerializerFor" + type.Name;

            string ASM_NAME = SerialName;
            string MOD_NAME = SerialName;
            string TYP_NAME = SerialName;

            string DLL_NAME = SerialName + ".dll";


            AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ASM_NAME), AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(MOD_NAME, DLL_NAME);

            TypeBuilder typBuilder = modBuilder.DefineType(TYP_NAME);
            typBuilder.AddInterfaceImplementation(typeof(IJson));

            MethodBuilder ToJsonMethodBuilder =
                typBuilder.DefineMethod(
                    "ToJson",
                    MethodAttributes.Public |
                    MethodAttributes.Virtual |
                    MethodAttributes.ReuseSlot,
                    typeof(string),
                    new Type[1] { typeof(object) }
                );

            ImplementToJsonMethod(ToJsonMethodBuilder, type);

            Type serialType = typBuilder.CreateType();
            asmBuilder.Save(DLL_NAME);

            IJson createdSerial = (IJson)Activator.CreateInstance(serialType);

            serializers.Add(type, createdSerial);

            return createdSerial;
        }

        private static void ImplementToJsonMethod(MethodBuilder toJsonMethodBuilder, Type type)
        {
            ILGenerator il = toJsonMethodBuilder.GetILGenerator();
            LocalBuilder tobj = il.DeclareLocal(type);

            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {



            }

            throw new NotImplementedException();
        }
    }
}

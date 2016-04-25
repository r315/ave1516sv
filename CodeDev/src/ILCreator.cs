using System;
using System.Reflection;
using System.Reflection.Emit;

public interface IJson
{
    string ToJson(object obj);
}

public class IlCreator
{
    public string ToJson(object obj)
    {
        return "ToJson";
    }

    private static void ImplementToJsonMethod(MethodBuilder toJsonMethodBuilder, Type type)
    {
        ILGenerator il = toJsonMethodBuilder.GetILGenerator();
        LocalBuilder tobj = il.DeclareLocal(type);
        LocalBuilder sb = il.DeclareLocal(typeof(string));
        LocalBuilder ttype = il.DeclareLocal(typeof(Type));
        FieldInfo[] fields = type.GetFields();

        il.Emit(OpCodes.Ldarg_1);               //get object reference from stack
        il.Emit(OpCodes.Castclass, type);       //do cast
        il.Emit(OpCodes.Stloc, tobj);

        il.Emit(OpCodes.Ldstr, "{");
        il.Emit(OpCodes.Newobj, typeof(System.Text.StringBuilder).GetConstructors()[2]);
        il.Emit(OpCodes.Stloc, sb);

        foreach (FieldInfo field in fields)
        {
            if (IsBuiltInType(field.GetType()))
            {
                //sb.Append(field.Name + ":");
                //MethodInfo mb = typeof(IlCreator).GetMethod("toString", new Type[] { type });
                //sb.Append((string)mb.Invoke(null, new Object[] { obj }));
                il.Emit(OpCodes.Ldstr, field.Name + ":");
                il.Emit(OpCodes.Ldarg_0); //this
                il.Emit(OpCodes.Call, typeof(System.Type).GetMethod("GetType", new Type[] { typeof(object) }));
                il.Emit(OpCodes.Ldstr, "ToString");
                il.Emit(OpCodes.Ldc_I4, 1);
                il.Emit(OpCodes.Newarr, typeof(Type));
                il.Emit(OpCodes.Stloc, ttype);
                il.Emit(OpCodes.Ldloc, ttype);
                il.Emit(OpCodes.Ldc_I4, 0);
                il.Emit(OpCodes.Ldloc, tobj);


                il.Emit(OpCodes.Pop);



            }

            else
            {
                //sb.Append(GetSerializer(type).ToJson(obj));
                il.Emit(OpCodes.Ldloc, sb);
                il.Emit(OpCodes.Ldloc, tobj);
                il.Emit(OpCodes.Call, typeof(IlCreator).GetMethod("GetSerializer",new Type[] { typeof(Type)}));
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, typeof(IlCreator).GetMethod("ToJson", new Type[] { typeof(object) }));
                il.Emit(OpCodes.Callvirt, typeof(System.Text.StringBuilder).GetMethod("Append", new Type[] { typeof(string) }));
                il.Emit(OpCodes.Pop);
            }

            //sb.Append(",");
            il.Emit(OpCodes.Ldloc, sb);            
            il.Emit(OpCodes.Ldstr, ",");           
            il.Emit(OpCodes.Callvirt, typeof(System.Text.StringBuilder).GetMethod("Append", new Type[] { typeof(string) }));
            il.Emit(OpCodes.Pop);
        }
        //sb.Remove(sb.Length - 1, 1);
        il.Emit(OpCodes.Ldloc, sb);
        il.Emit(OpCodes.Ldloc, sb);
        il.Emit(OpCodes.Callvirt, typeof(System.Text.StringBuilder).GetMethod("get_Length"));
        il.Emit(OpCodes.Ldc_I4, 1);
        il.Emit(OpCodes.Sub);
        il.Emit(OpCodes.Ldc_I4, 1);
        il.Emit(OpCodes.Callvirt, typeof(System.Text.StringBuilder).GetMethod("Remove", new Type[] { typeof(int), typeof(int) }));
        il.Emit(OpCodes.Pop);
        //sb.Append("}")
        il.Emit(OpCodes.Ldloc, sb);
        il.Emit(OpCodes.Ldstr, "}");
        il.Emit(OpCodes.Callvirt, typeof(System.Text.StringBuilder).GetMethod("Append", new Type[] { typeof(string) }));
        il.Emit(OpCodes.Pop);
        //sb.toString()
        il.Emit(OpCodes.Ldloc, sb);
        il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
        // return        
        il.Emit(OpCodes.Ret);
    }


    private static bool IsBuiltInType(Type t)
    {
        return t.IsPrimitive || t.Name.Equals("String") || t.Name.Equals("DateTime");
    }


    public static IJson GetSerializer(Type type)
    {
        
        return null;
    }
    public static IJson CreateSerializer(Type type)
    {
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
        return (IJson)Activator.CreateInstance(serialType);
    }
}


public class onefield
{
    public int campoA = 1;
    public string campoB = "ola";
}


public class Test
{

    public static void Main()
    {
        object obj = new onefield();
        IJson j = IlCreator.CreateSerializer(obj.GetType());
        String t = j.ToJson(obj);

        Console.Write(t);


    }
}



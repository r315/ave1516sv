using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Dynamic;
using System.Linq.Expressions;

namespace Jsonzai.Instr
{
    public interface IJson
    {
        string ToJson(object obj);
    }

    public class Jsoninstr
    {
        private const string quote = "\"";
        private const string iniBracket = "{";
        private const string endBracket = "}";
        private const string iniSquareBracket = "[";
        private const string endSquareBracket = "]";
        private static Dictionary<Type, IJson> serializers = new Dictionary<Type, IJson>();

        public static string ToJson(object obj)
        {        
            return GetSerializer(obj.GetType()).ToJson(obj);
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
            return (IJson) asSerializer.CreateInstance(asSerializer.GetType().AssemblyQualifiedName);
        }

        private static IJson CreateSerializer(Type type)
        {

            //string dllPath = null;

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
            LocalBuilder jsonStr = il.DeclareLocal(typeof(string));
            LocalBuilder fieldValues = il.DeclareLocal(typeof(string[]));

             il.Emit(OpCodes.Ldarg_1);
             il.Emit(OpCodes.Castclass, type);
             il.Emit(OpCodes.Stloc, tobj);

             FieldInfo[] fields = type.GetFields();
             string[] fieldNames = new string[fields.Length];

             StringBuilder sb = new StringBuilder("{{");
             for (int i=0; i<fields.Length; i++)
             {
                 fieldNames[i]=fields[i].Name;
                 if (i != 0)
                 {
                     sb.Append(",");
                 }
                 sb.Append(String.Format("\"{0}\":{{{1}}}", fieldNames[i], i));
             }
            sb.Append("}}");

             il.Emit(OpCodes.Ldstr, sb.ToString());
             il.Emit(OpCodes.Stloc, jsonStr);             
             il.Emit(OpCodes.Ldc_I4, fieldNames.Length); //Coloca na stack constant type Int32 (dimensao do array)
             il.Emit(OpCodes.Newarr, typeof(string)); //cria o array fieldValues com a dimensao passada. Ref para o array fica na stack
             il.Emit(OpCodes.Stloc, fieldValues);//Guarda o novo array na variável local fieldValues            
            
            //Ciclo que emite instrucoes IL equivalente a fieldValues[i]=valor do campo

            for (int i=0; i<fieldNames.Length; i++)
            {
                //il.Emit(OpCodes.Ldstr, fields[i].FieldType.ToString());
                //il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
              

                il.Emit(OpCodes.Ldloc, fieldValues); //Carrega o array na stack
                il.Emit(OpCodes.Ldc_I4, i); //Carrega o indice do array na stack
                il.Emit(OpCodes.Ldloc, tobj);//empilha na stack a referência para o objecto            
                il.Emit(OpCodes.Ldfld, fields[i]);//Carrega para a stack o valor do campo passado o FieldInfo fields[i]               
                il.Emit(OpCodes.Call, typeof(Jsoninstr).GetMethod("ToJson", new Type[] { fields[i].FieldType }));//Call para a implementacao especifica do TJson 
                il.Emit(OpCodes.Stelem_Ref);//Guarda a String com o valor do campo no array fieldValues
            }

            //No fim chama atraves de IL o StringFormat, passando o jsonString e fieldValues como
            //parametros

            il.Emit(OpCodes.Ldloc, jsonStr); //Empilha na stack a string
            il.Emit(OpCodes.Ldloc, fieldValues); //Empilha na stack a ref para String[] fieldValues
            il.Emit(OpCodes.Call, typeof(String).GetMethod("Format", new Type[] {typeof(String), typeof(object[])})); //call a Format.String
            il.Emit(OpCodes.Ret);
        }

        public static string ToJson(IEnumerable IEnum)
        {
            StringBuilder sb = new StringBuilder();

            foreach (dynamic item in IEnum)
            {
                sb.Append(ToJson(item) + ","); 
            }

            sb.Remove(sb.Length - 1, 1);

            return String.Format("{0}{1}{2}", iniSquareBracket, sb.ToString(), endSquareBracket);
        }

        public static string ToJson(string s)
        {
            return (s == null) ? "null" : String.Format("{0}{1}{2}", quote, s, quote);
        }

        public static string ToJson(char c)
        {
            return String.Format("{0}{1}{2}", quote, c.ToString(), quote);
        }

        public static string ToJson(DateTime dt)
        {
            return String.Format("{0}{1}{2}", quote, dt.ToString(), quote);
        }

        public static string ToJson(bool b)
        {
            return b.ToString().ToLower();
        }

        public static string ToJson(decimal d)
        {
            return d.ToString().Replace(',','.');
        }

        public static string ToJson(float f)
        {
            return f.ToString().Replace(',', '.');
        }

        public static string ToJson(double d)
        {
            return d.ToString().Replace(',', '.');
        }

        public static string ToJson(byte b)
        {
            return b.ToString();
        }

        public static string ToJson(sbyte sb)
        {
            return sb.ToString();
        }
        public static string ToJson(short sh)
        {
            return sh.ToString();
        }

        public static string ToJson(ushort ush)
        {
            return ush.ToString();
        }
        public static string ToJson(int i)
        {
            return i.ToString();
        }

        public static string ToJson(uint ui)
        {
            return ui.ToString();
        }

        public static string ToJson(long l)
        {
            return l.ToString();
        }

        public static string ToJson(ulong ul)
        {
            return ul.ToString();
        }
    }
}

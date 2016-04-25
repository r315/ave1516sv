using System;
using System.Reflection;
using System.Text;


// csc /t:library Point.cs
public class Point{

	public int x;
	public int y;


	public string ToJson(object obj){
		Type type = obj.GetType();
		StringBuilder sb = new StringBuilder("{");
		

        //foreach (FieldInfo field in fields){
        // if (IsBuiltInType(field.GetType())){

        MethodInfo mb = this.GetType().GetMethod("toString", new Type[] { type });
        sb.Append((string)mb.Invoke(null, new Object[] { obj }));

        //}else{
            //sb.Append(GetSerializer(type).ToJson(obj));
		//}
		
		
			//sb.Append(",");
	
		//}
		//sb.Remove(sb.Length - 1, 1);
		//sb.Append("}");
		return sb.ToString();
	}
	
	public int add()
	{
		return x + y;
	}

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
	
	public string toString(int val){
		return " "+ val;
	}	
	
	public interface IJson
    {
        string ToJson(object obj);
    }
	
	private static IJson GetSerializer(Type type){
		return null;
	}
}
/*
 foreach (FieldInfo field in fields)
            {
                il.Emit(OpCodes.Ldstr, "{");
                il.Emit(OpCodes.Newobj, typeof(StringBuilder));
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldsfld, t);
                il.Emit(OpCodes.Call, t.GetType().GetMethod("GetType"));
                il.Emit(OpCodes.Ldstr, "ToJson");
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Newarr, typeof(Type));
                il.Emit(OpCodes.Dup);

                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Stelem_Ref);
                il.Emit(OpCodes.Callvirt, typeof(System.Reflection.MethodInfo).GetMethod("GetType"));

                il.Emit(OpCodes.Stloc_3);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldloc_3);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Newarr, typeof(object));

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Stelem_Ref);

                il.Emit(OpCodes.Callvirt, typeof(System.Reflection.MethodBase));
                il.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) }));

                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldstr, "}");
                il.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(string) }));

                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Call, typeof(object).GetMethod("ToString"));

                il.Emit(OpCodes.Stloc_S, 4);
            }


*/
//csc /t:library Log.cs RefLog.cs IntLog.cs
//csc UseLog.cs /r:Log.dll
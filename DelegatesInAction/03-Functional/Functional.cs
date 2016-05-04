using System;
using System.Collections;

// Delegates in Action: exemplos de utilização de delegates.
//
// Programação Funcional.
//
public static class Functional
{
    public delegate void Action1(Object obj);
    public delegate Object Func1(Object obj);

    //
    // Aplique-se a função 'action' a cada elemento de 'source'.
    //
    public static void Apply(this IEnumerable source, Action1 action)
    {
        foreach (Object obj in source)
        {
            action(obj);
        }
    }

    //
    // Produza-se um IEnumerable em que cada elemento resulta de
    // aplicar 'func' a um elemento de 'source'.
    //
    // Implementação 'eager', que usa uma colecção para guardar o resultado
    // do processamento de toda a sequência de entrada.
    //
    public static IEnumerable EagerMap(this IEnumerable source, Func1 func)
    {
        IList res = new ArrayList();
        foreach (Object obj in source)
        {
            res.Add(func(obj));
        }
        return res;
    }

    //
    // Produza-se um IEnumerable em que cada elemento resulta de
    // aplicar 'func' a um elemento de 'source'.
    //
    // Implementação 'lazy' de algoritmo de Map, que vai calculando os resultados
    // à medida que forem sendo precisos.
    //
    // Usa duas classes auxiliares, com as implementações de IEnumerable e IEnumerator,
    // semelhantes ao par Iterable / Iterator de Java.
    //
    #region LazyMap
    
    class MapEnumerator : IEnumerator
    {
        private IEnumerator source;
        private Func1 func;
        
        private bool finished = false;
        private Object curr;
        
        public MapEnumerator(IEnumerator source, Func1 func)
        {
            this.source = source;
            this.func = func;
        }
        
        public bool MoveNext()
        {
            if (source.MoveNext()) {
                curr = func(source.Current);    // <<< Uso 'lazy' do delegate 'func'.
                return true;
            }
            finished = true;
            return false;
        }
        
        public Object Current
        {
            get {
                if (finished)
                    throw new InvalidOperationException();
                
                return curr;
            }
        }
        
        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
    
    class MapEnumerable : IEnumerable
    {
        private IEnumerable source;
        private Func1 func;
        
        public MapEnumerable(IEnumerable source, Func1 func)
        {
            this.source = source;
            this.func = func;
        }
        
        public IEnumerator GetEnumerator()
        {
            return new MapEnumerator(source.GetEnumerator(), func);
        }
    }
    
    public static IEnumerable Map(this IEnumerable source, Func1 func)
    {
        return new MapEnumerable(source, func);
    }

    #endregion
    
    //
    // Produza-se um IEnumerable em que cada elemento resulta de
    // aplicar 'func' a um elemento de 'source'.
    //
    // Implementação 'lazy' de algoritmo de Map, que vai calculando os resultados
    // à medida que forem sendo precisos.
    //
    // Usa a palavra-chave yield, levando a que o compilador de C# gere automaticamente
    // uma implementação equivalente à apresentada no passo anterior ('LazyMap').
    //
    
    #region LazyMapWithYield
    
    public static IEnumerable YieldedMap(this IEnumerable source, Func1 func)
    {
        foreach (Object obj in source)
        {
            yield return func(obj);
        }
    }

    #endregion

    // 
    // Produza-se uma sub-sequência com os 'n' primeiros elementos da
    // sequência de entrada 'source'.
    //
    // Versão 'eager'.
    //
    public static IEnumerable EagerTake(this IEnumerable source, int n)
    {
        IList res = new ArrayList();
        foreach (Object obj in source)
        {
            if (n <= 0) break;
            n -= 1;

            res.Add(obj);
        }
        return res;
    }

    // 
    // Produza-se uma sub-sequência com os 'n' primeiros elementos da
    // sequência de entrada 'source'.
    //
    // Versão 'lazy'.
    //
    #region LazyTake
    
    class TakeEnumerator : IEnumerator
    {
        private IEnumerator source;
        private int n;
        
        private bool finished = false;
        private Object curr;
        
        public TakeEnumerator(IEnumerator source, int n)
        {
            this.source = source;
            this.n = n;
        }
        
        public bool MoveNext()
        {
            if (n > 0 && source.MoveNext()) {
                curr = source.Current;           // <<< Obtenção 'lazy' de um elemento da sequência.
                n -= 1;
                return true;
            }
            finished = true;
            return false;
        }
        
        public Object Current
        {
            get {
                if (finished)
                    throw new InvalidOperationException();
                
                return curr;
            }
        }
        
        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
    
    class TakeEnumerable : IEnumerable
    {
        private IEnumerable source;
        private int n;
        
        public TakeEnumerable(IEnumerable source, int n)
        {
            this.source = source;
            this.n = n;
        }
        
        public IEnumerator GetEnumerator()
        {
            return new TakeEnumerator(source.GetEnumerator(), n);
        }
    }
    
    public static IEnumerable Take(this IEnumerable source, int n)
    {
        return new TakeEnumerable(source, n);
    }

    #endregion
    
    public static void Main(String[] args)
    {
        if (args.Length == 0)
        {
            args = new String[] { "alpha", "beta", "gamma", "delta", "epsilon" };
        }

        // Estilo imperativo
        foreach (String arg in args)
        {
            Console.WriteLine(arg);
        }
        
        // Estilo funcional
        Apply(args, Console.WriteLine);

        // Utilização de Apply como 'método de extensão'.
        // Permitido devido ao uso da palavra-chave 'this' no primeiro argumento do método Apply.
        args.Apply(Console.WriteLine);
        
        Console.WriteLine();
        
        // Transformação misturada com acção final.
        args.Apply(o => { Console.WriteLine(o.ToString().Length); });
        
        // Separação entre transformação (Map) e acção final (Apply).
        args.Map(o => o.ToString().Length).Apply(Console.WriteLine);
        
        Console.WriteLine();
        
        // Restrição da sequência de saída a 3 elementos.
        // Questão: quantas vezes é invocado o delegate configurado em Map?
        args.Map(o => o.ToString().Length).Take(3).Apply(Console.WriteLine);
        
        Console.WriteLine();
        
        int count;

        // Uso de EagerMap. count será igual ao número de elementos da sequência de entrada.
        count = 0;
        args.EagerMap(o => { count++; return o.ToString().Length; }).Take(3).Apply(Console.WriteLine);
        Console.WriteLine("count: {0}", count);

        // Uso de (lazy) Map. count não será maior do que 3.
        count = 0;
        args.Map(o => { count++; return o.ToString().Length; }).Take(3).Apply(Console.WriteLine);
        Console.WriteLine("count: {0}", count);

        // Uso de (lazy) YieldedMap. count não será maior do que 3.
        count = 0;
        args.YieldedMap(o => { count++; return o.ToString().Length; }).Take(3).Apply(Console.WriteLine);
        Console.WriteLine("count: {0}", count);
    }
}

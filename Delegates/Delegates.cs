using System;

/* Criação de novos tipos delegate. 

   Ver em CIL, classes derivadas de System.MulticastDelegate com:
    * construtor com dois parâmetros  (objecto-alvo e ponteiro para método)
    * método Invoke                   (para invocação síncrona do delegate)
    * métodos BeginInvoke / EndInvoke (para invocação assíncrona do delegate)
*/
    
delegate int BinaryOp(int a, int b); 

delegate bool Predicate(Object obj);

delegate Object Mapper(Object obj);

delegate int Comparer(Object o1, Object o2); 


/* Classes com métodos compatíveis com o tipo delegate BinaryOp. */

class Adder
{
    /* Método estático Adder.Add compatível com BinaryOp. */
    public static int Add(int x, int y)
    {
        Console.WriteLine(":: Adder.Add ::");
        return x + y;
    }
}

class BasedAdder
{
    private int baseVal;
    
    public BasedAdder(int baseVal)
    {
        this.baseVal = baseVal;
    }
    
    /* Método de instância BasedAdder.Add compatível com BinaryOp. */
    public int Add(int x, int y)
    {
        Console.WriteLine(":: BasedAdder.Add ::");
        return baseVal + x + y;
    }
}

public class Delegates
{
    /* Método estático Delegates.Sum compatível com BinaryOp. */
    private static int Sum(int x, int y)
    {
        Console.WriteLine(":: Delegates.Sum ::");
        return x + y;
    }

    /* O método Operate usa uma instância de BinaryOp. */
    private static void Operate(BinaryOp op, int a, int b)
    {
        /* Ver em CIL a chamada a BinaryOp::Invoke. */
        int res = op(a, b);
        Console.WriteLine(res);
    }

    private static void BasicExamples()
    {
        BasedAdder badder1 = new BasedAdder(10);
        BasedAdder badder2 = new BasedAdder(35);

        /* Criação de 4 instâncias de BinaryOp, a partir de métodos
           estáticos ou de instância compatíveis com BinaryOp.
           
           Ver em CIL o uso da instrução ldftn.
        */
        BinaryOp addV1 = new BinaryOp(Adder.Add);
        BinaryOp addV2 = new BinaryOp(Sum);
        BinaryOp addV3 = new BinaryOp(badder1.Add);
        BinaryOp addV4 = new BinaryOp(badder2.Add);
        
        /* Invocação das 4 instâncias de delegate.
           
           Ver em CIL como as 4 invocações são semelhantes. 
        */
        Operate(addV1, 3, 4);
        Operate(addV2, 3, 4);
        Operate(addV3, 3, 4);
        Operate(addV4, 3, 4);
    }

    private static void AnonymousMethodExample(int offset)
    {
        /* Desde o C# 2.0, é possível criar uma instância de delegate indicando
           explicitamente a implementação do método a executar, sem lhe atribuir
           um nome. Daí a designação de método anónimo.
           À semelhança do que acontece com os tipos anónimos em Java, é possível
           referir argumentos e variáveis locais, que não precisam de ser constantes.
           
           Ver em CIL:
            * Tipo auxiliar contendo campo para variável 'offset' e método com a
              implementação do método anónimo. 
            * Criação de instância de tipo auxiliar logo ao início de
              AnonymousMethodExample, seguido da cópia do argumento 'offset' para
              campo da instância criada.
            * Criação de instância de BinaryOp com newobj apesar de não aparecer
              'new BinaryOp(...)' no código C#.
            * O acesso a 'offset' no método anónimo é feito com ldfld
        */

        Operate(delegate (int x, int y) { return x * y + offset; }, 3, 4);
    }

    private static void LambdaExpressionExample()
    {
        /* A partir do C# 3.0, a utilização de expressões-lambda oferece uma sintaxe
           ainda mais sucinta para a criação de delegates específicos.
           Note-se como, tal como nos métodos anónimos, é possível utilizar argumentos
           e variáveis locais não-constantes do contexto em que a expressão-lambda é
           criada.
        
           Ver em CIL:
            * Semelhanças com o que já se tinha observado em AnonymousMethodExample
            * 'offset' nunca chega a existir como variável local, mas apenas como campo
            * Criação da instância de delegate BinaryOp apenas na primeira iteração.
        */
           
        int offset = 300;
        for (int i = 0; i < 5; ++i) {
            Operate((x,y) => x * y + offset++, 3, 4);
        }
    }
    
    public static void Main(String[] args)
    {
        BasicExamples();
        
        Console.WriteLine();
        
        AnonymousMethodExample(100);
        
        Console.WriteLine();
        
        LambdaExpressionExample();
    }
}
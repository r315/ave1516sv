using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates
{
    class DelegateOp
    {
        delegate int Operation(int a, int b);

        private static int sum(int a, int b) { return a + b; }
        private static int sub(int a, int b) { return a - b; }
        private static int mul(int a, int b) { return a * b; }

        private static int operation(Operation oper, int a, int b)
        {
            return oper(a, b);
        }

        static void Main(string[] args)
        {
            int res;
            Console.Write("Delegate sum as object ");
            Operation op = new Operation(sum);
            res = op(10, 20);
            Console.WriteLine(res);

            Console.Write("Delegate sub as lambda ");
            res = operation((a, b) => a - b, 20, 5);
            Console.WriteLine(res);




        }
    }
}

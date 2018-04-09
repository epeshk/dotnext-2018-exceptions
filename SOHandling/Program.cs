using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SOHandling
{
    internal class Program
    {
        private static int F(int n)
        {
            if (n % 1000 == 0)
                Console.WriteLine(n);
            return  F(n + 1) + 1;
        }
        public static void Main(string[] args)
        {
            Console.WriteLine("Before");
            TryCatchSO();
            TryCatchSO();
            Console.WriteLine("Survive after stack overflow");
        }

        private static void TryCatchSO()
        {
            try
            {
                CSharpExcHandler.HandleSO(() => F(0));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Handle :)");
            }
        }
    }
}
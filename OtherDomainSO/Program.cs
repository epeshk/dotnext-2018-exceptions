 using System;
 using System.Reflection;
 using System.Runtime.CompilerServices;
 using System.Runtime.InteropServices;
 using System.Threading;

namespace OtherDomainSO
{
    internal class Program
    {
        private static int RecursiveMethod(int n)
        {
            if (n % 1000 == 0)
                Console.WriteLine(n);
            return RecursiveMethod(n + 1) + 1;
        }
        
        public static void Main(string[] args)
        {
            try
            {
                var appd = AppDomain.CreateDomain(Guid.NewGuid().ToString());
                appd.DoCallBack(() =>
                {
                    
                    var thread = new Thread(() => RecursiveMethod(0));
                    Thread.Sleep(100);
                    thread.Start();
                    thread.Join();
                });
                
                AppDomain.Unload(appd);
            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("AppDomain unexpectedly unloaded. Probably, StackOverflow happens");
            }
            Console.WriteLine("Execute some other our code...");
        }
    }
}
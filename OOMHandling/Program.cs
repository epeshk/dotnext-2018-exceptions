using System;
using System.Collections.Generic;
using System.Threading;
using Utilities;

namespace ExceptionsHandling
{
    internal class OutOfMemory
    {
        public static void Run()
        {
            MemoryLimiter.LimitCommittedMemory(64.Mb());

            var list = new List<byte[]>();
            try
            {
                while(true)
                    list.Add(new byte[100000]);
            }
            catch (Exception e)
            {
                Console.WriteLine("in catch block...");
                Console.WriteLine(e);
            }
            Console.WriteLine("OOM handled");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            OutOfMemory.Run();
        }
    }

}
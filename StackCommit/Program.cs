using System;
using System.Collections.Generic;
using System.Threading;
using Utilities;

namespace StackCommit
{
    internal class Program
    {
        private static int F(int n)
        {
            if (n % 100 == 0)
                Console.WriteLine(n);
            return F(n + 1) + 1;
        }

        public static void Main(string[] args)
        {
            var t = new Thread(() =>
            {
                var limit = 64.Mb();
                MemoryLimiter.LimitCommittedMemory(limit);

                var list = new List<byte[]>();
                while (MemoryMeter.PrivateBytes() < limit - 128.Kb())
                    list.Add(new byte[4096]);
                try
                {
                    F(0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                GC.KeepAlive(list);
            }); // , 4 * 1024 * 1024);
            t.Start();
            t.Join();
        }
    }
}
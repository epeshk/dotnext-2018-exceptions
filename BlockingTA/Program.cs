using System;
using System.Threading;

namespace BlockingTA
{
    internal class Program
    {
        static ManualResetEvent mre = new ManualResetEvent(false);
        
        public static void A()
        {
            try
            {
                try
                {
                }
                finally // <-- No ThreadAbortException in catch/finally block
                {
                    mre.Set();
                    while (true)
                    {
                        Thread.SpinWait(10000); // do some work
                        Thread.Sleep(100); // or sleep/wait
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public static void Main(string[] args)
        {
            var thread = new Thread(A);
            thread.Start();
            mre.WaitOne();
            Console.WriteLine("Abort thread...");
            thread.Abort(); // <-- Never returns
            Console.WriteLine("Never prints!");
        }
    }
}
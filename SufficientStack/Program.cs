using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SufficientStack
{
    internal class Program
    {
        private static IntPtr lowLimit;
        private static ulong minStackTop;

        private static unsafe int ConsumeStack()
        {
            var t = 0;
            var stackTop = (ulong)&t;
            minStackTop = Math.Min(stackTop, minStackTop);
            RuntimeHelpers.EnsureSufficientExecutionStack();
            return ConsumeStack() + 1;
        }
        
        public static void Main(string[] args)
        {
            GetCurrentThreadStackLimits(out lowLimit, out var highLimit);
            minStackTop = (ulong) highLimit;
            var stackSize = (ulong) highLimit - (ulong) lowLimit;
            Console.WriteLine("Stack size: " + stackSize / 1024 + " KB");
            try
            {
                ConsumeStack();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType().Name + ": " + e.Message); // StackTrace is so huge!
            }
            Console.WriteLine("Min remaining stack space: " + (minStackTop - (ulong) lowLimit) / 1024 + " KB");
        }
        
        // HighLimit > LowLimit
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void GetCurrentThreadStackLimits(
            out IntPtr LowLimit,
            out IntPtr HighLimit
        );

    }
}
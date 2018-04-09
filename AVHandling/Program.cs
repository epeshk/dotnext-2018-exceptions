using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace AVHandling
{
    internal class Program
    {
        // [HandleProcessCorruptedStateExceptions]
        public static void Main(string[] args)
        {
            try
            {
               Marshal.WriteByte((IntPtr) 1000, 42);
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("AV in WriteByte handled");
            }
            
            try
            {
                var bytes = new byte[] {42};
                Marshal.Copy(bytes, 0, (IntPtr) 1000, bytes.Length);
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("AV in Copy handled");
            }
        }
    }
}
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SOHandling
{
    public enum VEH : long
    {
        EXCEPTION_CONTINUE_SEARCH = 0,
        EXCEPTION_EXECUTE_HANDLER = 1,
        EXCEPTION_CONTINUE_EXECUTION = -1
    }

    public class CSharpExcHandler
    {
        private const uint ExceptionStackOverflow = 0xc00000fd;
        private const uint Err = 0xf00000fd;

        public static void HandleSO(Action action)
            => HandleSO<object>(() => { action(); return null; });
        
        public static unsafe T HandleSO<T>(Func<T> action)
        {
            var exc = false;
            var handler = Kernel32.AddVectoredExceptionHandler(IntPtr.Zero, Handler);
            
            if (handler == IntPtr.Zero)
                throw new Win32Exception("AddVectoredExceptionHandler failed");
            
            var size = 32768;
            if (!Kernel32.SetThreadStackGuarantee(&size))
                throw new InsufficientExecutionStackException("SetThreadStackGuarantee failed", new Win32Exception());
            var result = default(T);
            try
            { 
                result = action();
            }
            catch (SEHException) when ((uint)Marshal.GetExceptionCode() == Err)
            {
                exc = true;
            }
            if (handler != IntPtr.Zero)
                Kernel32.RemoveVectoredExceptionHandler(handler);
            if (!exc)
                return result;
            if (Msvcrt._resetstkoflw() == 0)
                throw new InvalidOperationException("_resetstkoflw failed");
            throw new StackOverflowException();
        }

        private static unsafe VEH Handler(ref EXCEPTION_POINTERS exceptionPointers)
        {
            if (exceptionPointers.ExceptionRecord == null)
                return VEH.EXCEPTION_CONTINUE_SEARCH;
            
            var record = exceptionPointers.ExceptionRecord;
            Console.WriteLine(record->ExceptionCode);
            if (record->ExceptionCode != ExceptionStackOverflow)
                return VEH.EXCEPTION_CONTINUE_SEARCH;
            
            record->ExceptionCode = Err;
            return VEH.EXCEPTION_EXECUTE_HANDLER;
        }
    }

    public delegate VEH PVECTORED_EXCEPTION_HANDLER(ref EXCEPTION_POINTERS exceptionPointers);

    public static class Msvcrt
    {
        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern int _resetstkoflw();
    }

    public static class Kernel32
    {        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr AddVectoredExceptionHandler(IntPtr FirstHandler, PVECTORED_EXCEPTION_HANDLER VectoredHandler);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr RemoveVectoredExceptionHandler(IntPtr InstalledHandler);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe bool SetThreadStackGuarantee(int* StackSizeInBytes);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EXCEPTION_POINTERS
    {
        public EXCEPTION_RECORD* ExceptionRecord;
        public IntPtr Context;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EXCEPTION_RECORD
    {
        public uint ExceptionCode;
        public uint ExceptionFlags;
        public EXCEPTION_RECORD* ExceptionRecord;
        public IntPtr ExceptionAddress;
        public uint NumberParameters;
        public IntPtr* ExceptionInformation;
    }
}
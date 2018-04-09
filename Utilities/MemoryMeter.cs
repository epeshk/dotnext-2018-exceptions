using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Utilities
{
    public static class MemoryMeter
    {
        private static readonly Process process = Process.GetCurrentProcess();

        public static long PrivateBytes()
        {
            var sizeOfCountersEx = Marshal.SizeOf<PROCESS_MEMORY_COUNTERS_EX>();
            return PInvoke.GetProcessMemoryInfo(process.Handle, out var counters, sizeOfCountersEx)
                ? counters.PrivateUsage.ToInt64()
                : 0;
        }
    }
}
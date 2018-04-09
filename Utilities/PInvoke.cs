using System;
using System.Runtime.InteropServices;

namespace Utilities
{
    public static class PInvoke
    {
        private const string Psapi = "Psapi.dll";
        private const string Kernel32 = "kernel32.dll";

        // ReSharper disable BuiltInTypeReferenceStyle
        // ReSharper disable InconsistentNaming

        [DllImport(Psapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS_EX counters, Int32 cb);
        
        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool QueryInformationJobObject(
            IntPtr hJob,
            JOBOBJECTINFOCLASS JobObjectInformationClass,
            IntPtr lpJobObjectInfo,
            uint cbJobObjectInfoLength,
            IntPtr lpReturnLength);
        [DllImport(Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport(Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetInformationJobObject(IntPtr hJob, JOBOBJECTINFOCLASS JobObjectInfoClass, IntPtr lpJobObjectInfo, int cbJobObjectInfoLength);
        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();
        // ReSharper restore BuiltInTypeReferenceStyle
        // ReSharper restore InconsistentNaming
    }

    // ReSharper disable InconsistentNaming
    // ReSharper disable BuiltInTypeReferenceStyle

    public enum JOBOBJECTINFOCLASS
    {
        JobObjectExtendedLimitInformation = 9,
    }
    [Flags]
    public enum JOBOBJECT_BASIC_LIMIT_FLAGS
    {
        JOB_OBJECT_LIMIT_PROCESS_MEMORY = 0x00000100,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public IntPtr ProcessMemoryLimit;
        public IntPtr JobMemoryLimit;
        public IntPtr PeakProcessMemoryUsed;
        public IntPtr PeakJobMemoryUsed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public Int64 PerProcessUserTimeLimit;
        public Int64 PerJobUserTimeLimit;
        public JOBOBJECT_BASIC_LIMIT_FLAGS LimitFlags;
        public IntPtr MinimumWorkingSetSize;
        public IntPtr MaximumWorkingSetSize;
        public Int32 ActiveProcessLimit;
        public IntPtr Affinity;
        public Int32 PriorityClass;
        public Int32 SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IO_COUNTERS
    {
        public Int64 ReadOperationCount;
        public Int64 WriteOperationCount;
        public Int64 OtherOperationCount;
        public Int64 ReadTransferCount;
        public Int64 WriteTransferCount;
        public Int64 OtherTransferCount;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_MEMORY_COUNTERS_EX
    {
        public Int32 cb;
        public Int32 PageFaultCount;
        public IntPtr PeakWorkingSetSize;
        public IntPtr WorkingSetSize;
        public IntPtr QuotaPeakPagedPoolUsage;
        public IntPtr QuotaPagedPoolUsage;
        public IntPtr QuotaPeakNonPagedPoolUsage;
        public IntPtr QuotaNonPagedPoolUsage;
        public IntPtr PagefileUsage;
        public IntPtr PeakPagefileUsage;
        public IntPtr PrivateUsage;
    }
    // ReSharper restore BuiltInTypeReferenceStyle
    // ReSharper restore InconsistentNaming
}

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Utilities
{
    public static class MemoryLimiter
    {
        public static void LimitCommittedMemory(long bytes)
        {
            var job = PInvoke.CreateJobObject(IntPtr.Zero, null);
            if (job == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (!PInvoke.AssignProcessToJobObject(job, PInvoke.GetCurrentProcess()))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            var exInfo = JobTuner.GetExtendedLimitInfo(job);
            exInfo.ProcessMemoryLimit = (IntPtr) bytes;
            exInfo.BasicLimitInformation.LimitFlags |= JOBOBJECT_BASIC_LIMIT_FLAGS.JOB_OBJECT_LIMIT_PROCESS_MEMORY;
            JobTuner.ApplyJobObjectInfo(job, ref exInfo, JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation);
        }
    }
}
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Utilities
{
    public static class JobTuner
    {
        public static JOBOBJECT_EXTENDED_LIMIT_INFORMATION GetExtendedLimitInfo(IntPtr jobObject)
            => GetInfoFromJobObject<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>(jobObject,
                JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation);

        public static T GetInfoFromJobObject<T>(IntPtr jobObject, JOBOBJECTINFOCLASS jobObjectInfoClass)
            where T : struct
        {
            var infoLength = Marshal.SizeOf(typeof(T));
            var infoPtr = IntPtr.Zero;
            try
            {
                infoPtr = Marshal.AllocHGlobal(infoLength);
                if (!PInvoke.QueryInformationJobObject(jobObject, jobObjectInfoClass, infoPtr, (uint) infoLength,
                    IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return Marshal.PtrToStructure<T>(infoPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(infoPtr);
            }
        }

        public static void ApplyJobObjectInfo<T>(IntPtr jobObject, ref T info, JOBOBJECTINFOCLASS jobObjectInfoClass)
            where T : struct
        {
            var infoLength = Marshal.SizeOf(typeof(T));
            var infoPtr = IntPtr.Zero;

            try
            {
                infoPtr = Marshal.AllocHGlobal(infoLength);
                Marshal.StructureToPtr(info, infoPtr, false);
                if (!PInvoke.SetInformationJobObject(jobObject, jobObjectInfoClass, infoPtr, infoLength))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                Marshal.FreeHGlobal(infoPtr);
            }
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace AnyStatus
{
    public static class RamInformation
    {
        private const string PsapiDLL = "psapi.dll";
        private const int MegabyteFactor = 1024 * 1024;
        private static readonly int piSize = Marshal.SizeOf(new PerformanceInformation());

        [DllImport(PsapiDLL, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        public static decimal GetPercentageOfMemoryInUseMiB()
        {
            if (!GetPerformanceInfo(out PerformanceInformation pi, piSize))
            {
                throw new Exception("An error occurred while getting performance information.");
            }

            long pageSize = pi.PageSize.ToInt64();

            long availableMemory = pi.PhysicalAvailable.ToInt64() * pageSize / MegabyteFactor;
            long totalMemory = pi.PhysicalTotal.ToInt64() * pageSize / MegabyteFactor;
            long usedMemory = totalMemory - availableMemory;

            return (decimal)usedMemory / totalMemory * 100;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace AnyStatus
{
    public class DriveInformation
    {
        private const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx([In] string directoryName,
                                                 [Out] out ulong freeBytesAvailable,
                                                 [Out] out ulong totalNumberOfBytes,
                                                 [Out] out ulong totalNumberOfFreeBytes);

        public static int GetDriveAvailablePercentage(string drive)
        {
            ulong totalNumberOfFreeBytes;
            ulong totalNumberOfBytes;

            if (!GetDiskFreeSpaceEx(drive, out _, out totalNumberOfBytes, out totalNumberOfFreeBytes))
            {
                return -1;
            }

            return (int)Math.Round((totalNumberOfFreeBytes / (double)totalNumberOfBytes) * 100);
        }

        public static int GetDriveUsedPercentage(string drive)
        {
            ulong totalNumberOfFreeBytes;
            ulong totalNumberOfBytes;

            if (!GetDiskFreeSpaceEx(drive, out ulong _, out totalNumberOfBytes, out totalNumberOfFreeBytes))
            {
                return -1;
            }

            ulong totalNumberOfUsedBytes = totalNumberOfBytes - totalNumberOfFreeBytes;

            return (int)Math.Round((totalNumberOfUsedBytes / (double)totalNumberOfBytes) * 100);
        }
    }
}

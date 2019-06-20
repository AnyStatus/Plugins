using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AnyStatus
{
    public class DriveInformation
    {
        private const string Kernel32 = "kernel32.dll";
        private const int GigabyteFactor = 1024 * 1024 * 1024;

        private DriveInformation()
        {
        }

        public string Drive { get; private set; }

        public ulong TotalNumberOfFreeBytes { get; private set; }

        public ulong TotalNumberOfBytes { get; private set; }

        public int TotalNumberOfGigabytes
        {
            get => (int)Math.Round((double)TotalNumberOfBytes / GigabyteFactor);
        }

        public int TotalNumberOfFreeGigabytes
        {
            get => (int)Math.Round((double)TotalNumberOfFreeBytes / GigabyteFactor);
        }

        public int TotalNumberOfUsedGigabytes
        {
            get => (int)Math.Round((double)(TotalNumberOfBytes - TotalNumberOfFreeBytes) / GigabyteFactor);
        }

        public int AvailablePercentage
        {
            get => (int)Math.Round((TotalNumberOfFreeBytes / (double)TotalNumberOfBytes) * 100);
        }

        public int UsedPercentage
        {
            get => (int)Math.Round(((TotalNumberOfBytes - TotalNumberOfFreeBytes) / (double)TotalNumberOfBytes) * 100);
        }

        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx([In] string directoryName,
                                                      [Out] out ulong freeBytesAvailable,
                                                      [Out] out ulong totalNumberOfBytes,
                                                      [Out] out ulong totalNumberOfFreeBytes);

        public static DriveInformation ReadDrive(string drive)
        {
            if (!GetDiskFreeSpaceEx(drive, out _, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes))
            {
                throw new DriveNotFoundException($"An error occurred while getting drive information. No drive can be found with the specified name: {drive}.");
            }

            return new DriveInformation
            {
                Drive = drive,
                TotalNumberOfBytes = totalNumberOfBytes,
                TotalNumberOfFreeBytes = totalNumberOfFreeBytes
            };
        }
    }
}

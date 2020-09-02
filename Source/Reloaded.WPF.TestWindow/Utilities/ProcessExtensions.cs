using System;
using System.Runtime.InteropServices;

namespace Reloaded.WPF.TestWindow.Utilities
{
    public static class ProcessExtensions
    {
        /* Thread safety. */
        private static object _getProcessIdLock = new object();
        private static int[] _processes = new int[1000];

        /* Buffer for communication. */

        public static unsafe int[] GetProcessIds()
        {
            lock (_getProcessIdLock)
            {
                return GetProcessIdsInternal();
            }
        }

        private static unsafe int[] GetProcessIdsInternal()
        {
            // Get the list of process identifiers.
            int sizeOfProcesses = _processes.Length * sizeof(int);
            int bytesReturned;

            fixed (int* firstElement = _processes)
            {
                if (!EnumProcesses(firstElement, sizeOfProcesses, out bytesReturned))
                    return new int[0];
            }

            // Print the name and process identifier for each process.
            if (sizeOfProcesses <= bytesReturned)
            {
                _processes = new int[_processes.Length * 2];
                return GetProcessIds();
            }

            // Calculate how many process identifiers were returned.
            int processNumber = bytesReturned / sizeof(uint);
            int[] process = new int[processNumber];
            Buffer.BlockCopy(_processes, 0, process, 0, bytesReturned);
            return process;
        }


        /* Definitions */
        [DllImport("Psapi.dll", SetLastError = true)]
        public static extern unsafe bool EnumProcesses(int* processIds, Int32 arraySizeBytes, out Int32 bytesCopied);
    }
}

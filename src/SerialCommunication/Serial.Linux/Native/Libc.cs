using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Serial.Linux.Native
{
    class Libc
    {
        public enum OpenFlags
        {
            O_RDONLY = 0,
            O_WRONLY = 1,
            O_RDWR = 2,
            O_CREAT = 4,
            O_EXCL = 8,
            O_NOCTTY = 16,
            O_TRUNC = 32,
            O_APPEND = 64,
            O_NONBLOCK = 128,
            O_SYNC = 256
        }

        [DllImport("libc", SetLastError = true)]
        public static extern int open(string pathname, OpenFlags flags);

        [DllImport("libc")]
        public static extern int close(int fileDescriptor);

        [DllImport("libc")]
        public static extern IntPtr read(int fileDescriptor, IntPtr buf, IntPtr count);

        [DllImport("libc")]
        public static extern IntPtr write(int fileDescriptor, IntPtr buf, IntPtr count);
    }
}

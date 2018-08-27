using System;
using System.Collections.Generic;
using System.Text;

namespace Serial.Linux.Native
{
    public interface ILibc
    {
        int Open(string pathname, Libc.OpenFlags flags);

        int Close(int fileDescriptor);

        IntPtr Read(int fileDescriptor, IntPtr buf, IntPtr count);
        
        IntPtr Write(int fileDescriptor, IntPtr buf, IntPtr count);
    }
}

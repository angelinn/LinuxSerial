using Serial.Linux.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Serial.Linux
{
    public class Serial : IDisposable
    {
        public event EventHandler<string> OnDataReceived;
        private int hSerial;

        private bool isReading;

        private IntPtr buffer;
        private IntPtr size;
        private IntPtr response;
        private IntPtr responseSize;

        public Serial(string name)
        {
            hSerial = Libc.open(name, Libc.OpenFlags.O_NOCTTY | Libc.OpenFlags.O_RDWR | Libc.OpenFlags.O_SYNC);

            buffer = Marshal.AllocHGlobal(64);
            size = new IntPtr(64);
            response = Marshal.StringToHGlobalAnsi("OK>\r\n");
            responseSize = new IntPtr("OK>\r\n".Length);
        }

        public void Write(string text)
        {
            IntPtr message = Marshal.AllocHGlobal(text.Length);
            IntPtr messageSize = new IntPtr(text.Length);

            Libc.write(hSerial, message, messageSize);

            Marshal.FreeHGlobal(messageSize);
        }

        public void Write(IntPtr message, IntPtr size)
        {
            Libc.write(hSerial, message, size);
        }

        public void Read()
        {
            isReading = true;
            
            while (isReading)
            {
                IntPtr res = Libc.read(hSerial, buffer, size);
                string read = Marshal.PtrToStringAnsi(buffer);
                OnDataReceived?.Invoke(this, read);

                Libc.write(hSerial, response, responseSize);

                Task.Delay(500).Wait();
            }

        }

        public async Task ReadAsync()
        {
            await Task.Run(() => Read()).ConfigureAwait(false);
        }

        public void StopReading()
        {
            isReading = false;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(buffer);
            Marshal.FreeHGlobal(response);
        }
    }
}

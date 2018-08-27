using Serial.Linux.Exceptions;
using Serial.Linux.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Serial.Linux
{
    public class SerialCommunication : IDisposable
    {
        public event EventHandler<string> OnDataReceived;
        private int hSerial;

        private bool isReading;

        private IntPtr buffer;
        private IntPtr size;
        private IntPtr response;
        private IntPtr responseSize;
        
        private ILibc libc;

        public SerialCommunication(ILibc libc = null)
        {
            this.libc = libc ?? new Libc();
        }

        public void Open(string name)
        {
            hSerial = libc.Open(name, Libc.OpenFlags.O_NOCTTY | Libc.OpenFlags.O_RDWR | Libc.OpenFlags.O_SYNC);
            if (hSerial == -1)
                throw new SerialNotFoundException(name);

            buffer = Marshal.AllocHGlobal(64);
            size = new IntPtr(64);
            response = Marshal.StringToHGlobalAnsi("OK>\r\n");
            responseSize = new IntPtr("OK>\r\n".Length);
        }

        public void Write(string text)
        {
            IntPtr message = Marshal.AllocHGlobal(text.Length);
            IntPtr messageSize = new IntPtr(text.Length);

            libc.Write(hSerial, message, messageSize);

            Marshal.FreeHGlobal(messageSize);
        }

        public void Write(IntPtr message, IntPtr size)
        {
            libc.Write(hSerial, message, size);
        }

        public void Read()
        {
            isReading = true;
            
            while (isReading)
            {
                IntPtr res = libc.Read(hSerial, buffer, size);
                string read = Marshal.PtrToStringAnsi(buffer);
                OnDataReceived?.Invoke(this, read);

                libc.Write(hSerial, response, responseSize);

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

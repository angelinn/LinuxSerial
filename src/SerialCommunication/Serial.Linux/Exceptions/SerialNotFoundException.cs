using System;
using System.Collections.Generic;
using System.Text;

namespace Serial.Linux.Exceptions
{
    public class SerialNotFoundException : Exception
    {
        public SerialNotFoundException(string message = "") : base(message)
        {   }
    }
}

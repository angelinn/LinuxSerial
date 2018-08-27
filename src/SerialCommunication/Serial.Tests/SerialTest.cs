using Moq;
using NUnit.Framework;
using Serial.Linux;
using Serial.Linux.Exceptions;
using Serial.Linux.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Serial.Tests
{
    [TestFixture]
    public class SerialTest
    {
        [Test]
        public void SanityCheck()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void RunsWithExistingSerial()
        {
            Mock<ILibc> mock = new Mock<ILibc>();
            mock.Setup(o => o.Open(It.IsAny<string>(), It.IsAny<Libc.OpenFlags>())).Returns(1);

            SerialCommunication serial = new SerialCommunication(mock.Object);
            Assert.DoesNotThrow(() => serial.Open("dummy"));
        }
        
        [Test]
        public void ThrowsExceptionOnNonExistingSerial()
        {
            Mock<ILibc> mock = new Mock<ILibc>();
            mock.Setup(o => o.Open(It.IsAny<string>(), It.IsAny<Libc.OpenFlags>())).Returns(-1);

            SerialCommunication serial = new SerialCommunication(mock.Object);
            Assert.Throws<SerialNotFoundException>(() => serial.Open("dummy"));
        }

        [Test]
        public void ReadsData()
        {
            string targetString = "OK";
            byte[] bytes = Encoding.ASCII.GetBytes(targetString + '\0');

            Mock<ILibc> mock = new Mock<ILibc>();
            mock.Setup(o => o.Read(It.IsAny<int>(), It.IsAny<IntPtr>(), It.IsAny<IntPtr>()))
                .Returns(new IntPtr(1))
                .Callback<int, IntPtr, IntPtr>((i, p, c) => Marshal.Copy(bytes, 0, p, bytes.Length));

            
            SerialCommunication serial = new SerialCommunication(mock.Object);
            serial.Open("dummy");
            
            string received = serial.ReadOnce();
            Assert.AreEqual(targetString, received);
        }
    }
}

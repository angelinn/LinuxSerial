using Moq;
using NUnit.Framework;
using Serial.Linux;
using Serial.Linux.Exceptions;
using Serial.Linux.Native;
using System;
using System.Collections.Generic;
using System.Text;

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
            mock.Setup(o => o.Open(String.Empty, Libc.OpenFlags.O_RDWR)).Returns(1);

            SerialCommunication serial = new SerialCommunication(mock.Object);
            Assert.DoesNotThrow(() => serial.Open("dummy"));
        }
        
        [Test]
        public void ThrowsExceptionOnNonExistingSerial()
        {
            Mock<ILibc> mock = new Mock<ILibc>();
            mock.Setup(o => o.Open(String.Empty, Libc.OpenFlags.O_RDWR)).Returns(-1);

            SerialCommunication serial = new SerialCommunication(mock.Object);
            Assert.Throws<SerialNotFoundException>(() => serial.Open("dummy"));
        }
    }
}

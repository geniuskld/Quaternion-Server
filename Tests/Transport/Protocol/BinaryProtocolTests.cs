using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using Xunit;

namespace Tests.Transport.Protocol
{
    public class BinaryProtocolTests
    {
        [Fact]
        public void CreatePacket()
        {
            const string str = "not a very big deal";
            var bytes = new [] {byte.MinValue, byte.MaxValue, byte.MaxValue};
            var protocol = new BinaryProtocol(bytes,str);
            var protocolBytes = protocol.GetDataToSend();
            var protocolRestored = new BinaryProtocol(protocolBytes);
            var size = BinaryProtocol.GetMessageLength(protocolBytes);

            Assert.True(protocolRestored.IsCheckSumValid());
            Assert.True(protocolBytes.Length == size);
        }

        [Fact]
        public void BinaryEncryptableTest()
        {
            const string str = "not very big deal";
            var bytes = new[] { byte.MinValue, byte.MaxValue, byte.MaxValue };
            var protocol = new BinaryEncryptableProtocol(bytes, str);
            byte[] privateKey, publicKey;
            BinaryEncryptableProtocol.GenerateKeys(out privateKey, out publicKey);
            protocol.Encrypt(privateKey);
            var bytesReceived = protocol.GetDataToSend();
            var protocolRepacked = new BinaryEncryptableProtocol(bytesReceived);
            protocolRepacked.Decrypt(privateKey);
            var cleanData = protocolRepacked.Data;

            Assert.True(bytes.SequenceEqual(cleanData));
        }
    }
}

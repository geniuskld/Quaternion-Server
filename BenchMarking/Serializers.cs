using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BosonProtocol.Protocol;
using Tests.Emulators.ServerPlugin.Commands;
using Tests.Emulators.Transport;

namespace BenchMarking
{
    public class Serializers
    {
        private LogIn _loginCmd = new LogIn {Email = "Me@me.ru",Password = "pwd", PublicKey = new byte[10]};
        public Serializers()
        {
            
        }
        [Benchmark]
        public void ProtoBuf()
        {
            var serializer = new ProtoBufSerializer<LogIn>();
            var bytes = BinaryProtocol.FromObject(_loginCmd, "LogIn",serializer);
            var receivedPacket = new BinaryProtocol(bytes.GetDataToSend());
            var obj = serializer.Deserialize(receivedPacket.Data);
        }
        [Benchmark]
        public void GroBuf()
        {
            var serializer = new GrobufBinarySerializer<LogIn>();
            var bytes = BinaryProtocol.FromObject(_loginCmd, "LogIn", serializer);
            var receivedPacket = new BinaryProtocol(bytes.GetDataToSend());
            var obj = serializer.Deserialize(receivedPacket.Data);
        }
    }
}

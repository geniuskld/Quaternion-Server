using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using QuaternionProtocol.Serializers;
using Tests.Emulators.ServerPlugin.Commands;
using Tests.Emulators.Transport;
using Xunit;

namespace Tests.BenchMarking
{
    public class SerializersBenchMark
    {
        private readonly LogInRequest _loginCmd = new LogInRequest { Email = "Me@me.ru", Password = "pwd", PublicKey = new byte[10] };
        
        [Benchmark]
        public void ProtoBuf()
        {
            var serializer = new ProtoBufGenericSerializer<LogInRequest>();
            var bytes = BinaryProtocol.FromData(_loginCmd, "LogInRequest", serializer);
            var receivedPacket = new BinaryProtocol(bytes.GetDataToSend());
            var obj = serializer.Deserialize(receivedPacket.Data);
        }

        [Benchmark]
        public void Native()
        {
            var serializer = new BinaryFormaterGenericSerializer<LogInRequest>();
            var bytes = BinaryProtocol.FromData(_loginCmd, "LogInRequest", serializer);
            var receivedPacket = new BinaryProtocol(bytes.GetDataToSend());
            var obj = serializer.Deserialize(receivedPacket.Data);
        }

        [Fact]
        public void Run()
        {
            var benchmarkresults = BenchmarkRunner.Run<SerializersBenchMark>();
        }

    }
}
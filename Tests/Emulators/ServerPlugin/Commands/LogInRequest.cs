using System;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using ProtoBuf;
using Tests.Emulators.Transport;
using Transport.CommandsBase;
using Transport.Connections;

namespace Tests.Emulators.ServerPlugin.Commands
{
    [Serializable, ProtoContract]
    public class LogInRequest :IServerCommand
    {
        [ProtoMember(1)]
        public string Email { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }
        [ProtoMember(3)]
        public byte[] PublicKey { get; set; }

        public void Execute<T>(ServerConnection<T> connection, IServerCommand entity)
        {
            var loginValue = entity as LogInRequest;

            var response = new LogInResponse { IsOk = true };
            var protocol = BinaryProtocol.FromData(response, new ProtoBufGenericSerializer<LogInResponse>());
            connection.SendData(protocol);
        }
    }
}
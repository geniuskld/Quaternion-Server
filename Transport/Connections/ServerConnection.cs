
using System;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Serializers;
using Transport.CommandsBase;
using Transport.Server;

namespace Transport.Connections
{
    public class ServerConnection<TTransportDestType> : ServerConnectionBase
    {
        public readonly ProtocolBase<TTransportDestType> Protocol;
        public ISerializer<TTransportDestType> DeSerializer;
        public ServerConnection(ServerTransportBase<TTransportDestType> serverTransport, ProtocolBase<TTransportDestType> protocol, string connectionId) : base(serverTransport, connectionId)
        {
            Protocol = protocol;
        }

        public void OnDataReceived(ProtocolBase<TTransportDestType> protocol, Transport.CommandsBase.IServerCommand action, Type actiontype)
        {
            var data = protocol.ProtocolName != Protocol.ProtocolName ? Protocol.CleanData(protocol.GetDataToSend()) : protocol.GetBody();

            var obj = DeSerializer.Deserialize(actiontype, data);
            action.Execute(this,obj as IServerCommand);
        }

        public override string SerializerName => DeSerializer.Name;
    }
}
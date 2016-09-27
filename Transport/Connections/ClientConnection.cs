using System;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Serializers;
using Transport.Client;
using Transport.CommandsBase;

namespace Transport.Connections
{
    public class ClientConnection<TTransportDestType> : ClientConnectionBase
    {
        private readonly ClientTransportBase<TTransportDestType> _transport;
        private readonly ProtocolBase<TTransportDestType> _protocol;
        public ISerializer<TTransportDestType> DeSerializer;

        public ClientConnection(ClientTransportBase<TTransportDestType> transport, ProtocolBase<TTransportDestType> protocol) : base(transport)
        {
            _transport = transport;
            _protocol = protocol;
        }

        public void OnDataReceived(ProtocolBase<TTransportDestType> protocol, IClientCommand action, Type actiontype)
        {
            var data = protocol.ProtocolName != _protocol.ProtocolName
                ? _protocol.CleanData(protocol.GetDataToSend())
                : protocol.GetBody();

            var obj = DeSerializer.Deserialize(actiontype, data);
            action.Execute(this, obj as IClientCommand);
        }
    }
}
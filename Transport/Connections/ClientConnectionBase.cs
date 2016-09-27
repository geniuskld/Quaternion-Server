#region

using System;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Serializers;
using Transport.Client;
using Transport.CommandsBase;
using Transport.Connections.Peers;

#endregion

namespace Transport.Connections
{
    public abstract class ClientConnectionBase: IDisposable
    {
        public ClientPeerBase PeerRef;
        public readonly TransportStats TransportStats;
        private readonly IClientTransport _transport;

        protected ClientConnectionBase(IClientTransport transport)
        {
            _transport = transport;
            TransportStats = new TransportStats(transport.TransportName);
        }

        public string ConnectionName { get; set; }

        public OperationState SendData<T>(ProtocolBase<T> data)
        {
            return _transport.SendData(data);
        }
        
        public void Dispose()
        {
            //_transport.Dispose();
        }
    }
}
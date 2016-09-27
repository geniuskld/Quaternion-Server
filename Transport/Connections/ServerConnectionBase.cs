using System;
using System.Diagnostics;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using Transport.Connections.Peers;
using Transport.Server;

namespace Transport.Connections
{
    /// <summary>
    /// Represents info about current ConnectionBase to concrete trasport feature
    /// </summary>
    [DebuggerDisplay("{ConnectionId}:{ServerTransport.TransportName}:{RemoteAddressInfo}:{LastPingTimeUtc}")]
    public abstract class ServerConnectionBase : IDisposable
    {
        protected ServerConnectionBase(Server.IServerTransport serverTransport, string connectionId)
        {
            if (serverTransport == null) throw new ArgumentNullException(nameof(serverTransport));

            if (string.IsNullOrEmpty(connectionId)) connectionId = Guid.NewGuid().ToString();

            ConnectionId = connectionId;
            ServerTransport = serverTransport;
        }

        public abstract string SerializerName { get; }
        public string ConnectionId
        {
            get { return _connectionId; }
            set
            {
                if (!string.IsNullOrEmpty(ConnectionId))
                    ServerTransport.ChangeConnectionId(_connectionId, value);
                _connectionId = value;
            }
        }

        public string TransportType => ServerTransport.TransportName;
        internal Server.IServerTransport ServerTransport { get; private set; }

        public DateTime LastPingTimeUtc
        {
            get { return ServerTransport.GetLastPingTimeFor(ConnectionId); }
            set
            {
                ServerTransport.Touch(ConnectionId);
            }
        }

        public string RemoteAddressInfo => ServerTransport.GetClientAddressInfo(ConnectionId);
        public TransportStats TransportStats => ServerTransport.GetStats(ConnectionId);

        //Null if client not found
        public PeerBase Peer;
        private string _connectionId;

        public OperationState SendData<T>(ProtocolBase<T> data)
        {
            return ServerTransport.SendData(ConnectionId, data);
        }
        public void CloseConnection()
        {
            ServerTransport.DisconnectClient(ConnectionId);
        }

        public void Dispose()
        {
            CloseConnection();
            Peer = null;
            ServerTransport = null;
            ConnectionId = "";
        }

        public override string ToString()
        {
            return $"{ConnectionId}:{ServerTransport.TransportName}";
        }
    }
}
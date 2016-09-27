using System;
using System.Collections.Generic;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;

namespace Transport.Connections.Peers
{
    public abstract class PeerBase : IDisposable
    {
        protected PeerBase(string appId)
        {
            if (string.IsNullOrEmpty(appId)) throw new ArgumentNullException(nameof(appId));
            ApplicationId = appId;
            ConnectionId = Guid.NewGuid().ToString();
        }
        public string ApplicationId { get; private set; }
        public string ConnectionId { get; set; }

        /// <summary>
        /// transportname,ConnectionBase
        /// </summary>
        public readonly Dictionary<string, ServerConnectionBase> Connections  = new Dictionary<string, ServerConnectionBase>();

        protected void AddConnection(ServerConnectionBase serverConnectionBase)
        {
            if (serverConnectionBase == null) throw new ArgumentNullException(nameof(serverConnectionBase));
            serverConnectionBase.ServerTransport.ChangeConnectionId(serverConnectionBase.ConnectionId, ConnectionId);
            if (Connections.ContainsKey(serverConnectionBase.ServerTransport.TransportName))
            {
                var currentConnection = Connections[serverConnectionBase.ServerTransport.TransportName];
                currentConnection.CloseConnection();
                Connections.Remove(serverConnectionBase.ServerTransport.TransportName);
            }
            serverConnectionBase.Peer = this;
            Connections.Add(serverConnectionBase.ServerTransport.TransportName, serverConnectionBase);
        }

        public OperationState Send<T>(string protocolName, ProtocolBase<T> data) 
        {
            ServerConnectionBase serverConnectionBase;
            if (Connections.TryGetValue(protocolName, out serverConnectionBase))
            {
                return Send(serverConnectionBase, data);
            }

            return new OperationState { IsSuccessful = false, Details = $"Protocol {protocolName} not found for current peer" };
        }

        public OperationState Send<T>(ServerConnectionBase serverConnectionBase, ProtocolBase<T> data) 
        {
            var operationState = serverConnectionBase.ServerTransport.SendData(ConnectionId, data);

            if (operationState.IsSuccessful)
                serverConnectionBase.TransportStats.AddBytesSent(operationState.Counter);
            else return operationState;
            return new OperationState { IsSuccessful = true };
        }

        public void Dispose()
        {
            foreach (var connection in Connections)
            {
                connection.Value.ServerTransport.DisconnectClient(ConnectionId);
            }
        }
    }
}
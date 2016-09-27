using System;
using System.Collections.Generic;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;

namespace Transport.Connections.Peers
{
    public class ClientPeerBase
    {
        public string AppId { get; set; }
        protected readonly Dictionary<string, ClientConnectionBase> Connections = new Dictionary<string, ClientConnectionBase>();

        public void AddClientConnection(ClientConnectionBase connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(connection.ConnectionName)) throw new ArgumentNullException(nameof(connection.ConnectionName));

            if (Connections.ContainsKey(connection.ConnectionName))
            {
                var currentConnection = Connections[connection.ConnectionName];
                currentConnection.Dispose();
                Connections.Remove(connection.ConnectionName);
            }
            connection.PeerRef = this;
            Connections.Add(connection.ConnectionName, connection);
        }

        public OperationState Send<T>(string connectionName, ProtocolBase<T> data)
        {
            ClientConnectionBase connectionBase;
            if (Connections.TryGetValue(connectionName, out connectionBase))
            {
                return Send(connectionBase, data);
            }

            return new OperationState { IsSuccessful = false, Details = $"Protocol {connectionName} not found for current peer" };
        }

        public OperationState Send<T>(ClientConnectionBase connection, ProtocolBase<T> data)
        {
            var operationState = connection.SendData(data);

            if (operationState.IsSuccessful)
                connection.TransportStats.AddBytesSent(operationState.Counter);
            else return operationState;
            return new OperationState { IsSuccessful = true };
        }

        public void Dispose()
        {
            foreach (var connection in Connections)
            {
                connection.Value.Dispose();
            }
        }
    }
}
using System;
using System.IO;
using System.Net;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using Transport.Connections;
using Transport.Events;

namespace Transport.Server
{
    public interface IServerTransport : Transport.IServerTransport
    {
        event EventHandler<ServerGenericEventArgs<ServerConnectionBase>> OnConnected;
        event EventHandler<ServerGenericEventArgs<ServerConnectionBase>> OnDisconnected;
        string ServerAddress { get; }
        void DisconnectClient(string connectionId);
        void Start();
        string GetClientAddressInfo(string connectionId);
        DateTime GetLastPingTimeFor(string connectionId);
        void Touch(string connectionId);
        void ChangeConnectionId(string oldConnectionId, string newConnectionId);
        TransportStats GetStats(string connectionId);
        OperationState SendData<T>(string connectionId, ProtocolBase<T> data);
        OperationState SendData(Stream data, string connectionId);
        OperationState SetPort(int portNo);
    }
}
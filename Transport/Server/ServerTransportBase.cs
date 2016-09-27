using System;
using System.IO;
using System.Net;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Events;

namespace Transport.Server
{
    public abstract class ServerTransportBase<TTransportDataType> : IDisposable, IServerTransport
    {
        protected bool IsStarted;
        public abstract event EventHandler<ServerGenericEventArgs<TTransportDataType>> OnReceive;
        public event EventHandler<ServerGenericEventArgs<ServerConnectionBase>> OnConnected;
        public event EventHandler<ServerGenericEventArgs<ServerConnectionBase>> OnDisconnected;
        public event EventHandler<DataEventArgs<Exception>> OnError;

        protected ServerTransportBase()
        {
            RegisterCommand("Ping", new Ping());
        }
        
        public abstract string TransportName { get; }
        public abstract string Address { get; }
        public abstract void Start();

        public abstract string GetClientAddressInfo(string connectionId);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionId"></param>
        /// <param name="data">Shoud inherit ISerializableTo/<TTransportDataType/></param>
        /// <returns>Bytes sent</returns>
        /// 
        public abstract OperationState SendData<T>(string connectionId, ProtocolBase<T> data);

        public abstract OperationState SendData(Stream data, string connectionId);
        public abstract OperationState SetPort(int portNo);
        public string ServerAddress => Address;

        public void CloseConnection()
        {
           Dispose();
        }
        
        public abstract DateTime GetLastPingTimeFor(string connectionId);
        public abstract void Touch(string connectionId);

        public abstract TransportStats GetStats(string connectionId);
        public abstract void ChangeConnectionId(string oldConnectionId, string newConnectionId);

        public abstract OperationState RegisterCommand<T>(string commandName, T executeAction) where T:IServerCommand;

        public abstract void DisconnectClient(string connectionId);

        #region event invocators


        protected void OnClientConnectedInvoke(ServerConnectionBase e)
        {
            OnConnected?.Invoke(this, new ServerGenericEventArgs<ServerConnectionBase> {ServerConnectionBase = e});
        }

        protected void OnClientDisconnectedInvoke(ServerGenericEventArgs<ServerConnectionBase> e)
        {
            OnDisconnected?.Invoke(this, e);
        }

        protected void OnErrorInvoke(ServerGenericEventArgs<Exception> e)
        {
            OnError?.Invoke(this, e);
        }
        #endregion

        public abstract void Dispose();
    }
}

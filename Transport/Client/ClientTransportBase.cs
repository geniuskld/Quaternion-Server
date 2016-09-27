using System;
using System.IO;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Events;

namespace Transport.Client
{
    public abstract class ClientTransportBase<TTransportDataType> : IDisposable, IClientTransport
    {

        public event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnConnected;
        public event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnDisconected;

        public event EventHandler<ClientGenericEventArgs<TTransportDataType>> OnReceive;
        public event EventHandler<DataEventArgs<Exception>> OnError;
        public abstract string TransportName { get; }
        public abstract string Address { get; }
        public abstract OperationState SendData<T>(ProtocolBase<T> data);
        public abstract OperationState SendData(Stream data);
        public abstract OperationState RegisterCommand<T>(string commandName, T executeAction) where T : IClientCommand;
        public abstract OperationState Connect(string endpointAddress);
        public abstract void Dispose();
        public void CloseConnection()
        {
            Dispose();
        }
        #region event invokators
        protected  void RaiseReceive(ClientGenericEventArgs<TTransportDataType> e)
        {
            OnReceive?.Invoke(this, e);
        }

        protected void RaiseError(ClientGenericEventArgs<Exception> e)
        {
            OnError?.Invoke(this, e);
        }

        protected void RaiseConnected(ClientGenericEventArgs<ClientConnectionBase> args )
        {
            OnConnected?.Invoke(this,args);
        }

        protected void RaiseDisconnected()
        {
            //OnDisconnected?.Invoke(this, EventArgs.Empty);
        }
        #endregion


    }
}
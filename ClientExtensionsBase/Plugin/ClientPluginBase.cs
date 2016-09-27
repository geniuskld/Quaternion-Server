using System;
using System.Collections.Generic;
using QuaternionProtocol;
using Transport.Client;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Events;

namespace ClientExtensionsBase.Plugin
{
    public class ClientPluginBase : IClientPluginBase
    {
        public event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnClientConnected;
        public event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnClientDisconnected;
        public event EventHandler<DataEventArgs<Exception>> OnTransportException;
        protected readonly List<IClientTransport> Transports = new List<IClientTransport>();
        public virtual string AppId { get; }
        public virtual IEnumerable<IClientTransport> InitTransports(IEnumerable<IClientTransport> aviableTransports)
        {
            foreach (var transport in aviableTransports)
            {
                switch (transport.TransportName)
                {
                    case "TCPIPV4":
                        Transports.Add(transport);
                        //transport.SetPort(1000);
                        //transport.OnConnected += (sender, args) => { RaiseClientConnected(args); };
                        //transport.OnDisconected += (sender, connectionArgs) => { RaiseClientDisconnected(connectionArgs);};
                        transport.OnError += (sender, exceptionArgs) => { RaiseTransportException(exceptionArgs); };
                        break;
                    case "UDPIPV4":
                        Transports.Add(transport);
                        ////transport.SetPort(10100);
                        //transport.OnConnected += (sender, connectionArgs) => { ClientConnected(connectionArgs.DataReceived); };
                        //transport.OnDisconected += (sender, connectionArgs) => { ClientDisconnected(connectionArgs.DataReceived); };
                        //transport.OnError += (sender, exceptionArgs) => { TransportException(new Tuple<IServerTransport, Exception>(transport, exceptionArgs.DataReceived)); };
                        break;
                }
            }

            return Transports.ToArray();
        }

        public OperationState RegisterReceiveCommand<T>(IClientTransport transport, string commandName, T execute) where T : IClientCommand
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));
            if (string.IsNullOrEmpty(commandName)) throw new ArgumentNullException(nameof(commandName));
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            return transport.RegisterCommand(commandName, execute);
        }

        public OperationState RegisterReceiveCommand<T>(IClientTransport transport, T execute) where T : IClientCommand
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            return RegisterReceiveCommand(transport, typeof(T).Name, execute);
        }

#region Event Invokators
        protected virtual void RaiseClientConnected(ClientGenericEventArgs<ClientConnectionBase> e)
        {
            OnClientConnected?.Invoke(this, e);
        }

        protected virtual void RaiseClientDisconnected(ClientGenericEventArgs<ClientConnectionBase> e)
        {
            OnClientDisconnected?.Invoke(this, e);
        }

        protected virtual void RaiseTransportException(DataEventArgs<Exception> e)
        {
            OnTransportException?.Invoke(this, e);
        }

#endregion
    }
}
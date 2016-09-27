using System;
using System.Collections.Generic;
using QuaternionProtocol;
using Transport;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Server;

namespace PluginBase
{
    public abstract class PluginBase<TTransportBase> where TTransportBase: ITransport , IPlugin
    {
        public event EventHandler<ServerConnectionBase> OnClientConnected;
        public event EventHandler<ServerConnectionBase> OnClientDisconnected;
        public event EventHandler<Tuple<TTransportBase, Exception>> OnTransportException;
        protected readonly List<TTransportBase> Transports = new List<TTransportBase>();

        public abstract string PluginName { get; }
        public abstract string AppId { get; }

        public virtual IEnumerable<TTransportBase> InitTransports(IEnumerable<IServerTransport> aviableTransports)
        {
            foreach (var transport in aviableTransports)
            {
                switch (transport.TransportName)
                {
                    case "TCPIPV4":
                        Transports.Add(transport);
                        //transport.SetPort(1000);
                        transport.Start();
                        transport.OnConnected += (sender, connectionArgs) => { ClientConnected(connectionArgs.DataReceived); };
                        transport.OnDisconnected += (sender, connectionArgs) => { ClientDisconnected(connectionArgs.DataReceived); };
                        transport.OnError += (sender, exceptionArgs) => { TransportException(new Tuple<IServerTransport, Exception>(transport, exceptionArgs.DataReceived)); };
                        break;
                    case "UDPIPV4":
                        Transports.Add(transport);
                        //transport.SetPort(10100);
                        transport.Start();
                        transport.OnConnected += (sender, connectionArgs) => { ClientConnected(connectionArgs.DataReceived); };
                        transport.OnDisconnected += (sender, connectionArgs) => { ClientDisconnected(connectionArgs.DataReceived); };
                        transport.OnError += (sender, exceptionArgs) => { TransportException(new Tuple<IServerTransport, Exception>(transport, exceptionArgs.DataReceived)); };
                        break;
                }
            }

            return Transports.ToArray();
        }

        public TTransportBase GetTransportByName(string transportName)
        {
            return Transports.FirstOrDefault(x => x.TransportName == transportName);
        }

        public OperationState RegisterReceiveCommand<TCommand>(TTransportBase serverTransport, string commandName, TCommand execute) where TCommand: ICommand
        {
            if (serverTransport == null) throw new ArgumentNullException(nameof(serverTransport));
            if (string.IsNullOrEmpty(commandName)) throw new ArgumentNullException(nameof(commandName));
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            return serverTransport.RegisterCommand(commandName, execute);
        }

        public OperationState RegisterReceiveCommand<TCommand>(TTransportBase serverTransport, TCommand execute) where TCommand : ICommand
        {
            if (serverTransport == null) throw new ArgumentNullException(nameof(serverTransport));
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            return RegisterReceiveCommand(serverTransport, typeof(TTransportBase).Name, execute);
        }

        #region Invoke


        protected void ClientConnected(ServerConnectionBase e)
        {
            OnClientConnected?.Invoke(this, e);
        }

        protected void ClientDisconnected(ServerConnectionBase e)
        {
            OnClientDisconnected?.Invoke(this, e);
        }

        protected void TransportException(Tuple<TTransportBase, Exception> e)
        {
            OnTransportException?.Invoke(this, e);
        }
        #endregion
    }
}
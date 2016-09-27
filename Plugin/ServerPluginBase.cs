using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using QuaternionProtocol;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Server;

namespace PluginBase
{
    public abstract class ServerPluginBase : IServerPlugin
    {
        public event EventHandler<ServerConnectionBase> OnClientConnected;
        public event EventHandler<ServerConnectionBase> OnClientDisconnected;
        public event EventHandler<Tuple<IServerTransport, Exception>> OnTransportException;
        protected readonly List<IServerTransport> Transports = new List<IServerTransport>();

        public abstract string PluginName { get; }
        public abstract string AppId { get; }

        public virtual IEnumerable<IServerTransport> InitTransports(IEnumerable<IServerTransport> aviableTransports)
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

        public IServerTransport GetTransportByName(string transportName)
        {
            return Transports.FirstOrDefault(x => x.TransportName == transportName);
        }

        public OperationState RegisterReceiveCommand<T>(IServerTransport serverTransport, string commandName, T execute) where T: IServerCommand
        {
            if (serverTransport == null) throw new ArgumentNullException(nameof(serverTransport));
            if (string.IsNullOrEmpty(commandName)) throw new ArgumentNullException(nameof(commandName));
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            return serverTransport.RegisterCommand(commandName, execute);
        }

        public OperationState RegisterReceiveCommand<T>(IServerTransport serverTransport, T execute) where T : IServerCommand
        {
            if (serverTransport == null) throw new ArgumentNullException(nameof(serverTransport));
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            return RegisterReceiveCommand(serverTransport, typeof(T).Name, execute);
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

        protected void TransportException(Tuple<IServerTransport, Exception> e)
        {
            OnTransportException?.Invoke(this, e);
        }
        #endregion
    }
}
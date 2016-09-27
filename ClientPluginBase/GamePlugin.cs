using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ClientExtensionsBase.Binary;
using ClientExtensionsBase.Plugin;
using ClientGamePlugin.Commands.TCP;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using Transport.Client;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Connections.Peers;
using Transport.Events;

namespace ClientGamePlugin
{
    public class GamePlugin : ClientPluginBase
    {
        public event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnClientConnected;
        public event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnClientDisconnected;
        public event EventHandler<ClientGenericEventArgs<Exception>> OnTransportException;
        //public virtual string AppId { get; }
        private ClientTransportBase<byte[]> _tcp; 
        public void Start()
        {
            var transports = new List<IClientTransport>();
            _tcp = new TcpClient();
            transports.Add(_tcp);
            InitTransports(transports);
            RegisterTCPCommands(_tcp);
            _tcp.OnReceive += TcpOnReceive;
        }

        private void TcpOnReceive(object sender, ClientGenericEventArgs<byte[]> clientGenericEventArgs)
        {
            var proto = new BinaryProtocol(clientGenericEventArgs.DataReceived);
           Trace.WriteLine($"Package received {proto.GetCommandBytes()}");
        }

        public void Connect(IPEndPoint serverSide)
        {
            _tcp.Connect($"{serverSide.Address}:{serverSide.Port}");
        }

        public void JoinGame(string name)
        {
            var cmd = new JoinRequest {GameName = name};
            var proto = BinaryProcolHelper<JoinRequest>.GetProtocol(cmd);
            _tcp.SendData(proto);
        }

        public void Leave()
        {
            var cmd = new LeaveRequest();
            var proto = BinaryProcolHelper<LeaveRequest>.GetProtocol(cmd);
            _tcp.SendData(proto);
        }

        public void GetUsersInGame(string name)
        {
            var cmd = new GetUsersInGame {GameName = name};
            var proto = BinaryProcolHelper<GetUsersInGame>.GetProtocol(cmd);
            _tcp.SendData(proto);
        }

        public void SendToGame(string gameName, ProtocolBase<byte> command)
        {
            var cmd = new SendToGameUsers() {Message = "Hello"};
            var proto = BinaryProcolHelper<SendToGameUsers>.GetProtocol(cmd);
            _tcp.SendData(proto);
        }

        public void SendToUserInGame(string gameName, string userId, ProtocolBase<byte[]> command)
        {
            if (gameName == null) throw new ArgumentNullException(nameof(gameName));
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (command == null) throw new ArgumentNullException(nameof(command));
        }


        private void RegisterTCPCommands(IClientTransport transport)
        {
            var cmds = new List<IClientCommand> { new JoinRequest(), new LeaveRequest() };

            foreach (var cmd in cmds)
            {
                transport.RegisterCommand(cmd.GetType().Name, cmd);
            }
        }
    }
}
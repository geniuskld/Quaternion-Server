using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using Tests.Emulators.ServerPlugin.Commands;
using Transport;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Events;
using Transport.Server;

namespace Tests.Emulators.Transport
{
    public class ServerTransportEmulator : ServerTransportBase<byte[]>
    {
        private readonly Dictionary<byte[], object> _cmds = new Dictionary<byte[], object>();
        public override event EventHandler<ServerGenericEventArgs<byte[]>> OnReceive;
        
        public override string TransportName => "TCPIPV4";
        public override string Address { get; }

        public void RaiseReceive(Type comd, byte[] bytes, ServerConnectionBase serverConnection)
        {
            var conn = serverConnection as ServerConnection<byte[]>;
            var proto = new BinaryProtocol(bytes);
            conn?.OnDataReceived(proto, new LogInRequest(), comd);
            OnReceive?.Invoke(this,new ServerGenericEventArgs<byte[]> {ServerConnectionBase =  serverConnection, Protocol = new BinaryProtocol(bytes),DataReceived = bytes});
        }
        public override OperationState RegisterCommand<T>(string commandName, T executeAction)
        {
            var cmdhash = StreamHelper.WrapBytes(HashHelper.GeHash(commandName), 6);
            _cmds.Add(cmdhash, executeAction);
            return new OperationState { IsSuccessful = true };
        }

        public override void DisconnectClient(string connectionId)
        {

        }

        public override void Dispose()
        {
        }

        public override void Start()
        {
        }

        public override string GetClientAddressInfo(string connectionId)
        {
            return IPAddress.Any.ToString();
        }

        public override OperationState SendData<T>(string connectionId, ProtocolBase<T> data)
        {
            return new OperationState { IsSuccessful = true };
        }


        public override DateTime GetLastPingTimeFor(string connectionId)
        {
            return DateTime.UtcNow;
        }

        public override void Touch(string connectionId)
        {
            throw new NotImplementedException();
        }

        public override TransportStats GetStats(string connectionId)
        {
            return new TransportStats("TEST");
        }

        public override void ChangeConnectionId(string oldConnectionId, string newConnectionId)
        {
        }

        public override OperationState SendData(Stream data, string connectionId)
        {
            return new OperationState { IsSuccessful = true };
        }

        public override OperationState SetPort(int portNo)
        {
            return new OperationState {IsSuccessful = false, Details = "Emulator has no port number"};
        }
    }
}
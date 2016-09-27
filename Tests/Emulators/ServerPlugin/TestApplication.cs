using System.Collections.Generic;
using PluginBase;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using SocketServer;
using Tests.Emulators.ServerPlugin.Commands;
using Tests.Emulators.Transport;
using Transport;
using Transport.Connections;
using Transport.Server;
using IServerTransport = Transport.Server.IServerTransport;

namespace Tests.Emulators.ServerPlugin
{
    public class TestApplication :ServerPluginBase
    {
        private readonly ServerTransportEmulator _serverTransport;
        public TestApplication()
        {
            _serverTransport = new ServerTransportEmulator();
            _serverTransport.Start();
            InitTransports(new List<IServerTransport>{_serverTransport});
        }
        
        public override string PluginName => "TestPlugin";
        public override string AppId => "AppId";

        public void RaiseClientConnected(ServerConnectionBase serverConnectionBase)
        {
           ClientConnected(serverConnectionBase);
        }

        public void RaisePacketReceived(ServerConnectionBase serverConnection)
        {
            var l = new LogInRequest {Email = "g@g.g", Password = "123"};
            var bp = BinaryProtocol.FromData(l, new ProtoBufGenericSerializer<LogInRequest>());
            _serverTransport.RaiseReceive(typeof(LogInRequest), bp.GetDataToSend(), serverConnection);
        }
    }
}
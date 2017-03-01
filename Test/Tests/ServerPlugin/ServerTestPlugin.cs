using System;
using System.Collections.Generic;
using Tests.Emulators.ServerPlugin;
using Tests.Emulators.ServerPlugin.Commands;
using Tests.Emulators.Transport;
using Transport;
using Transport.Connections;
using Transport.Connections.Peers;
using Xunit;

namespace Tests.ServerPlugin
{
    public class ServerTestPlugin
    {
        [Fact]
        public void CreatePlugin()
        {
            var plugin = new TestApplication();
            var Clients = new List<PeerBase>();
            var tcp = plugin.GetTransportByName("TCPIPV4");
            plugin.RegisterReceiveCommand(tcp, new LogInRequest());

            var connection = ServerConnectionFactory.Create(plugin.GetTransportByName("TCPIPV4"), Guid.NewGuid().ToString());
            var byteconn = connection as ServerConnection<byte[]>;
            byteconn.DeSerializer = new ProtoBufGenericSerializer<LogInRequest>();

            plugin.OnClientConnected += (sender, conn) =>
            {
                var peer = new TestPeer(conn);
                Clients.Add(peer);
            };

            plugin.RaiseClientConnected(connection);
            plugin.RaisePacketReceived(connection);

        }

        [Fact]
        public void CreateRealServer()
        {
            var realPlugin = new RealApplication();
            var launcher = new PluginLauncher.PluginLauncher();
            launcher.Load(realPlugin);
        }
    }
}

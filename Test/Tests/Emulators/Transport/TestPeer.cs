using System;
using Transport;
using Transport.Connections;
using Transport.Connections.Peers;

namespace Tests.Emulators.Transport
{
    public class TestPeer: PeerBase
    {
        private const string AppId = "12332";
        public TestPeer(ServerConnectionBase serverConnectionBase) : base(AppId)
        {
            AddConnection(serverConnectionBase);
        }
    }
}
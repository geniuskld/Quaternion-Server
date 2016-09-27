#region

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GameBasePlugin.Commands.TCP;
using GameBasePlugin.ProtocolWrapers;
using QuaternionProtocol.Protocol;
using Transport;
using Transport.Connections;
using Transport.Connections.Peers;

#endregion

namespace GameBasePlugin
{
    public class ServerGameBaseServerPlugin : PluginBase.ServerPluginBase
    {
        public override string PluginName => "ServerGameBaseServerPlugin";
        public override string AppId { get; }

        public static readonly ConcurrentDictionary<string, List<PeerBase>> UsersInScenes =
            new ConcurrentDictionary<string, List<PeerBase>>();

        public ServerGameBaseServerPlugin(string appId)
        {
            AppId = appId;
            OnClientDisconnected += GameServerPluginOnClientDisconnected;
        }

        public static void SendToGamePlayers<T>(string gameName, ProtocolBase<T> package,
            string protocolName = "TCPIPV4")
        {
            List<PeerBase> playersPeers;
            if (UsersInScenes.TryGetValue(gameName, out playersPeers))
            {
                foreach (var peer in playersPeers)
                {
                    var conn = peer.Connections.FirstOrDefault(x => x.Key == protocolName).Value;
                    conn?.SendData(package);
                }
            }
        }

        private void GameServerPluginOnClientDisconnected(object sender, ServerConnectionBase connection)
        {
            var peersList = UsersInScenes.FirstOrDefault(x => x.Value.Contains(connection.Peer));

            if (!string.IsNullOrEmpty(peersList.Key))
            {
                peersList.Value?.Remove(connection.Peer);

                var leaveResponse = new LeaveResponse {ConnectionId = connection.ConnectionId, IsOk = true};
                foreach (var peer in peersList.Value)
                {
                    var cmd = BinaryProcolHelper<LeaveResponse>.GetProtocol(leaveResponse);
                    peer.Connections.First().Value?.SendData(cmd);
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameBasePlugin.ProtocolWrapers;
using QuaternionProtocol;
using Transport;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Connections.Peers;

namespace GameBasePlugin.Commands.TCP
{
    [DataContract]
    public class JoinRequest : IServerCommand
    {
        [DataMember]
        public string GameName { get; set; }
        public void Execute<T>(ServerConnection<T> connection, IServerCommand entity)
        {
            var joinCmd = (JoinRequest)entity;

            List<PeerBase> peersList;
            if (ServerGameBaseServerPlugin.UsersInScenes.TryGetValue(joinCmd.GameName, out peersList))
            {
                if (!peersList.Contains(connection.Peer))
                {
                    peersList.Add(connection.Peer);

                    var cmd = new JoinResponse { IsOk = true, ConnectionId = connection.ConnectionId };
                    var response = BinaryProcolHelper<JoinResponse>.GetProtocol(cmd);
                    connection.SendData(response);

                    ServerGameBaseServerPlugin.SendToGamePlayers(joinCmd.GameName, response);
                }
            }
        }
    }
}
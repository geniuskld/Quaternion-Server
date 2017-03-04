using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameBasePlugin.ProtocolWrappers;
using Transport;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Connections.Peers;

namespace GameBasePlugin.Commands.TCP
{
    [DataContract, Serializable]
    public class LeaveRequest: IServerCommand
    {
        [DataMember]
        public string GameName { get; set; }
    
        public void Execute<T>(ServerConnection<T> connection, IServerCommand entity)
        {
            var leaveRequest = (LeaveRequest)entity;
            var leaveResponse = new LeaveResponse();
            List<PeerBase> connections;

            if (ServerGameBaseServerPlugin.UsersInScenes.TryGetValue(leaveRequest.GameName, out connections))
            {
                var connInGame = connections.FirstOrDefault(x => x == connection.Peer);

                if (connInGame != null)
                {
                    leaveResponse.IsOk = true;
                    ServerGameBaseServerPlugin.RaiseGameLeft(connInGame);
                    connections.Remove(connInGame);
                }
                leaveResponse.OperationDetails = "User not in game";
            }
            else
            {
                leaveResponse.OperationDetails = "Game not found";
            }

            leaveResponse.ConnectionId = connection.ConnectionId;
            var cmd = BinaryProtocolHelper<LeaveResponse>.GetProtocol(leaveResponse);
            connection.SendData(cmd);

            ServerGameBaseServerPlugin.SendToGamePlayers(leaveRequest.GameName, cmd);
           
        }
    }
}
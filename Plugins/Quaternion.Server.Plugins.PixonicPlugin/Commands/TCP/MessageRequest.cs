using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using GameBasePlugin.ProtocolWrappers;
using QuaternionProtocol.Protocol.Binary;
using Transport.CommandsBase;
using Transport.Connections;

namespace Quaternion.Server.Plugins.PixonicPlugin.Commands.TCP
{
    public class MessageRequest: IServerCommand
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string RoomName { get; set; }
        public void Execute<T>(ServerConnection<T> connection, IServerCommand entity)
        {
            var cmd = (MessageRequest) entity;
            var response = new MessageResponse {Message = cmd.Message};
            var data = BinaryProtocolHelper<MessageResponse>.GetProtocol(response).Data;

            PixonicServerPlugin.LastRoomAction[cmd.RoomName] = DateTime.UtcNow;
            GameBasePlugin.ServerGameBaseServerPlugin.SendToGamePlayers(RoomName,new BinaryProtocol(data, "MessageResponse"));
        }
    }
}
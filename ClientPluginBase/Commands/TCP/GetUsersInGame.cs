using System;
using System.Collections.Generic;
using Transport.CommandsBase;
using Transport.Connections;

namespace ClientGamePlugin.Commands.TCP
{
    [Serializable]
    public class GetUsersInGame: IClientCommand
    {
        public List<string> Ids;
        public string GameName { get; set; }

        public void Execute(ClientConnectionBase connection, IClientCommand entity)
        {
            var cmd = (GetUsersInGame) entity;

        }
    }
}
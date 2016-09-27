using System;
using Transport.Connections;

namespace Transport.CommandsBase
{
    [Serializable]
    public class Ping:IServerCommand
    {
        public void Execute<T>(ServerConnection<T> serverConnection, IServerCommand entity)
        {

        }
    }
}
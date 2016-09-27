using System;
using System.Diagnostics;
using System.Reflection;
using Transport.CommandsBase;
using Transport.Connections;

namespace ClientGamePlugin.Commands.TCP
{
    [Serializable]
    public class JoinRequest: IClientCommand
    {
        public string GameName { get; set; }
        public void Execute(ClientConnectionBase connection, IClientCommand entity)
        {
          Trace.WriteLine("Join request got");
        }
    }
}
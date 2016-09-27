using System;
using System.Diagnostics;
using Transport.Client;
using Transport.CommandsBase;
using Transport.Connections;

namespace ClientGamePlugin.Commands.TCP
{
    public class SendToGameUsers: IClientCommand
    {
        public string Message { get; set; }
        public void Execute(ClientConnectionBase connection, IClientCommand entity)
        {
            var cmd = (SendToGameUsers) entity;
            Trace.WriteLine($"Cmd sent to Users with message {Message}");
        }
    }
}
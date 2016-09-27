using System.Diagnostics;
using Transport.CommandsBase;
using Transport.Connections;

namespace ClientGamePlugin.Commands.TCP
{
    public class LeaveRequest : IClientCommand
    {
        public void Execute(ClientConnectionBase connection, IClientCommand entity)
        {
           Trace.WriteLine("Leave Req Got");
        }
    }
}
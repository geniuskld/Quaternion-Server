using System.Collections.Generic;
using System.Linq;
using PluginBase;
using Tests.Emulators.ServerPlugin.Commands;
using Transport;
using Transport.Server;
using IServerTransport = Transport.Server.IServerTransport;

namespace Tests.Emulators.ServerPlugin
{
    public class RealApplication: ServerPluginBase
    {
        public override string PluginName => "RealServerApp";
        public override string AppId => "RealServerApp";

        public override IEnumerable<IServerTransport> InitTransports(IEnumerable<IServerTransport> aviableTransports)
        {
            var transports = base.InitTransports(aviableTransports);
            RegisterReceiveCommand(transports.First(), new LogInRequest());

            return transports;
        }
    }
}
using System;
using System.Collections.Generic;
using QuaternionProtocol;
using PluginBase;
using SocketServer;
using IServerTransport = Transport.Server.IServerTransport;

namespace PluginLauncher
{
    public class PluginLauncher
    {
        public OperationState Load(IServerPlugin plugin)
        {
            try
            {
                var tcpServer = new TcpSocketServer();
                plugin.InitTransports(new List<IServerTransport> { tcpServer });
            }
            catch (Exception ex)
            {
                return new OperationState { IsSuccessful = false, Details = ex.ToString() };
            }

            return new OperationState { IsSuccessful = true };
        }
    }
}

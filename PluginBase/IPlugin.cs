using System;
using System.Collections.Generic;
using QuaternionProtocol;
using Transport;
using Transport.Connections;
using Transport.Server;

namespace PluginBase
{
    public interface IPlugin
    {
        event EventHandler<ServerConnectionBase> OnClientConnected;
        event EventHandler<ServerConnectionBase> OnClientDisconnected;
        event EventHandler<Tuple<IServerTransport, Exception>> OnTransportException;
        string PluginName { get; }
        string AppId { get; }

        /// <summary>
        /// All aviable transport instances. SetUp and return it
        /// </summary>
        /// <param name="aviableTransports">All aviable default transports instances</param>
        /// <returns>Return only thouse you want to use</returns>
        IEnumerable<IServerTransport> InitTransports(IEnumerable<IServerTransport> aviableTransports);
    }
}

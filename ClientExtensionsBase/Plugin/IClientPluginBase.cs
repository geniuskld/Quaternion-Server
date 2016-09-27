using System;
using System.Collections.Generic;
using Transport.Client;
using Transport.Connections;
using Transport.Events;

namespace ClientExtensionsBase.Plugin
{
    public interface IClientPluginBase
    {

        event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnClientConnected;
        event EventHandler<ClientGenericEventArgs<ClientConnectionBase>> OnClientDisconnected;
        event EventHandler<DataEventArgs<Exception>> OnTransportException;
        string AppId { get; }

        /// <summary>
        /// All aviable transport instances. SetUp and return it
        /// </summary>
        /// <param name="aviableTransports">All aviable default transports instances</param>
        /// <returns>Return only thouse you want to use</returns>
        IEnumerable<IClientTransport> InitTransports(IEnumerable<IClientTransport> aviableTransports);
    }
}
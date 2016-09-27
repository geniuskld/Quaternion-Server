using System;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using QuaternionProtocol.Serializers;
using Transport.Server;

namespace Transport.Connections
{
    public static class ServerConnectionFactory
    {
        public static ServerConnectionBase Create(Server.IServerTransport serverTransport, string connectionId)
        {
            switch (serverTransport.TransportName)
            {
                case "TCPIPV4":
                case "UDPIPV4":
                    var transp = (ServerTransportBase<byte[]>) serverTransport;
                    var conn = new ServerConnection<byte[]>(transp,new BinaryProtocol(new byte[50]), connectionId)
                    {
                        DeSerializer = new BinaryFormaterGenericSerializer<object>()
                    };
                    return conn;
                case "SignalR":
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverTransport), "Tranport not found");
            }
        }
    }
}
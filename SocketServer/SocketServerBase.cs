using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using ObjectsPool;
using Transport;
using Transport.Events;
using Transport.Server;

namespace SocketServer
{
    public abstract class SocketServerBase : ServerTransportBase<byte[]>
    {
        protected readonly Dictionary<byte[], Tuple<Transport.CommandsBase.IServerCommand, Type>> Commands = new Dictionary<byte[], Tuple<Transport.CommandsBase.IServerCommand, Type>>();
        protected readonly Pool<SocketAsyncEventArgsExtended> ClientPool = new Pool<SocketAsyncEventArgsExtended>();
        protected readonly ConcurrentDictionary<string, SocketAsyncEventArgsExtended> ClientsToSocket = new ConcurrentDictionary<string, SocketAsyncEventArgsExtended>();
        protected Socket ListenSocket; // прослушивающий сокет
        protected CancellationTokenSource CancellationSource;
        protected IPEndPoint ServerEndPoint; // конечная точка сервера
        protected readonly SocketAsyncEventArgs ListenArgs = new SocketAsyncEventArgs(); // информация, связанная с прослушивающим сокетом

        public override string Address
        {
            get
            {
                var addr = (ListenSocket?.LocalEndPoint as IPEndPoint);
                return $"{addr}";
            }
        }

        protected SocketServerBase()
        {
            ServerEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        protected SocketServerBase(IPAddress ip, int port)
        {
            ServerEndPoint = new IPEndPoint(ip, port);
        }

        public override void DisconnectClient(string connectionId)
        {
            SocketAsyncEventArgsExtended clientSocket;

            if (string.IsNullOrEmpty(connectionId)) return;
            ClientsToSocket.TryRemove(connectionId, out clientSocket);

            try
            {
                clientSocket.ConnectSocket.Shutdown(SocketShutdown.Both);
                clientSocket.UserToken = null;
                clientSocket.Dispose();
            }
            catch (Exception ex)
            {
                OnErrorInvoke(new ServerGenericEventArgs<Exception> { DataReceived = ex });
            }
        }

        public override string GetClientAddressInfo(string connectionId)
        {
            SocketAsyncEventArgsExtended socket;
            if (ClientsToSocket.TryGetValue(connectionId, out socket))
            {
                return socket.AcceptSocket.RemoteEndPoint.ToString();
            }

            return null;
        }

        public override OperationState RegisterCommand<T>(string commandName, T executeAction)
        {
            var commandHash = StreamHelper.WrapBytes(HashHelper.GeHash(commandName), 6);

            if (!Commands.ContainsKey(commandHash))
            {
                Commands.Add(commandHash, new Tuple<Transport.CommandsBase.IServerCommand, Type>(executeAction, typeof(T)));
                return new OperationState { IsSuccessful = true };
            }
            return new OperationState { IsSuccessful = false, Details = "Command name already added or reserved. Change command name." };
        }

        public override DateTime GetLastPingTimeFor(string connectionId)
        {
            SocketAsyncEventArgsExtended connection;
            if (ClientsToSocket.TryGetValue(connectionId, out connection))
            {
                return connection.LastResponse;
            }

            return default(DateTime);
        }

        /// <summary>
        /// Sends raw data
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override OperationState SendData(Stream data, string connectionId)
        {
            SocketAsyncEventArgsExtended socket;
            if (ClientsToSocket.TryGetValue(connectionId, out socket))
            {
                try
                {
                    if (socket.WhenSocketReady.WaitOne(1000))
                    {
                        socket.SocketState = SocketState.Busy;
                        var bytesSent = socket.ConnectSocket.Send(StreamHelper.GetBytes(data));
                        socket.SocketState = SocketState.Idle;
                        return new OperationState { IsSuccessful = true, Counter = bytesSent };
                    }

                    return new OperationState { IsSuccessful = false, Details = "Socket timeout" };
                }
                catch (Exception ex)
                {
                    return new OperationState { IsSuccessful = false, Details = ex.ToString() };
                }
            }

            return new OperationState { IsSuccessful = false, Details = "Socket not found" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type 'T' should inherit ISelfSerializableTo of byte[] </typeparam>
        /// <param name="connectionId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override OperationState SendData<T>(string connectionId, ProtocolBase<T> data)
        {
            var protoBytes = data as ProtocolBase<byte[]>;

            if (protoBytes != null)
            {
                using (var str = new MemoryStream(protoBytes.GetDataToSend()))
                {
                    return SendData(str, connectionId);
                }
            }

            return new OperationState { IsSuccessful = false, Details = "ProtocolBase<byte[]> is correct type of Proto" };
        }

        public override void ChangeConnectionId(string oldConnectionId, string newConnectionId)
        {
            SocketAsyncEventArgsExtended connection;
            if (ClientsToSocket.TryRemove(oldConnectionId, out connection))
            {
                ClientsToSocket.TryAdd(newConnectionId, connection);
            }
        }

        public override TransportStats GetStats(string connectionId)
        {
            SocketAsyncEventArgsExtended connection;
            ClientsToSocket.TryGetValue(connectionId, out connection);
            return connection?.CurrentSocketStats;
        }

        public override void Dispose()
        {
            CancellationSource.Cancel();
            ListenSocket.Shutdown(SocketShutdown.Both);
            ListenSocket.Dispose();
        }
    }

}

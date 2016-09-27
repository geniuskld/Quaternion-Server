using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using QuaternionProtocol;
using QuaternionProtocol.Protocol.Binary;
using Transport;
using Transport.Connections;
using Transport.Events;

namespace SocketServer
{
    public class TcpSocketServer : SocketServerBase
    {
        public TcpSocketServer() : base(IPAddress.Any, 1000)
        {

        }

        public TcpSocketServer(IPAddress address, int port) : base(address, port)
        {

        }

        public override event EventHandler<ServerGenericEventArgs<byte[]>> OnReceive;
        public override string TransportName => "TCPIPV4";
        private readonly byte[] _receiveBuffer = new byte[2000];
        public override void Start()
        {
            if (IsStarted) return;
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.AcceptConnection, true);
            ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, true);
            ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            ListenSocket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            ListenSocket.LingerState = new LingerOption(true, 30);
            ListenSocket.Bind(ServerEndPoint);
            ListenSocket.Listen(100);
            ListenSocket.ReceiveTimeout = 1000;
            ListenSocket.SendTimeout = 1000;
            CancellationSource = new CancellationTokenSource();

            if (CancellationSource.IsCancellationRequested) return;
            ListenSocket.BeginAccept(EndAccept, null);
            IsStarted = true;
        }

        public override OperationState SetPort(int portNo)
        {
            if (!IsStarted)
            {
                ServerEndPoint = new IPEndPoint(IPAddress.Any, portNo);
                return new OperationState {IsSuccessful = true};
            }

            return new OperationState {IsSuccessful = false, Details = "Can't set port when server already started"};
        }

        public override void Touch(string connectionId)
        {
            SocketAsyncEventArgsExtended connection;
            if (ClientsToSocket.TryGetValue(connectionId, out connection))
            {
                connection.LastResponse = DateTime.UtcNow;
            }
        }

        private void EndAccept(IAsyncResult ar)
        {
            var listener = ListenSocket.EndAccept(ar);
            var connId = Guid.NewGuid().ToString();
            var args = ClientPool.Take();
            args.AcceptSocket = listener;
            args.UserToken = ServerConnectionFactory.Create(this, connId);
            args.LastResponse = DateTime.UtcNow.AddMinutes(1);
            listener.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, EndReceive, args);
            ListenSocket.BeginAccept(EndAccept, ListenArgs);
            ClientsToSocket.TryAdd(connId, args);
            OnClientConnectedInvoke((ServerConnectionBase)args.UserToken);
        }

        private void EndReceive(IAsyncResult ar)
        {
            var evArgs = (SocketAsyncEventArgs)ar.AsyncState;
            try
            {
                var args = (SocketAsyncEventArgsExtended)evArgs;
                switch (args.SocketError)
                {
                    case SocketError.Success:
                        var bytesReceivedCount = args.AcceptSocket.EndReceive(ar);

                        if (args.AcceptSocket.Connected)
                        {
                            var receivedBytes = new byte[bytesReceivedCount];
                            Buffer.BlockCopy(_receiveBuffer, 0, receivedBytes, 0, bytesReceivedCount);
                            args.CurrentSocketStats.AddBytesReceived(args.BytesTransferred);
                            args.LastResponse = DateTime.UtcNow;

                            args.BytesBuffer.Put(receivedBytes);

                            while (args.BytesBuffer.Size > 4)
                            {
                                var messageLength = BinaryProtocol.GetMessageLength(args.BytesBuffer.Peek(args.BytesBuffer.Size));
                                if (args.BytesBuffer.Size >= messageLength)
                                {
                                    var messageBytes = args.BytesBuffer.Get(messageLength);
                                    var messageBytesCopy = new byte[BinaryProtocol.HeaderLength+1];

                                    Buffer.BlockCopy(messageBytes,0,messageBytesCopy,0,BinaryProtocol.HeaderLength+1);

                                    var protocol = new BinaryProtocol(messageBytesCopy);
                                    var commandHash = protocol.GetCommandBytes();

                                    Tuple<Transport.CommandsBase.IServerCommand, Type> command;
                                    var userConnection = args.UserToken as ServerConnection<byte[]>;

                                    if (Commands.TryGetValue(commandHash, out command))
                                    {
                                        userConnection?.OnDataReceived(protocol, command.Item1, command.Item2);
                                    }
                                    
                                    ReceiveInvoke(new ServerGenericEventArgs<byte[]>
                                    {
                                        ServerConnectionBase = args.UserToken as ServerConnection<byte[]>,
                                        DataReceived = messageBytes,
                                        Protocol = protocol
                                    });
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;

                    case SocketError.ConnectionReset:
                        break;
                    case SocketError.TimedOut:
#if DEBUG
                        Trace.WriteLine("SocketError.TimedOut");
#endif
                        break;
                    default:
                        var connection = args.UserToken as ServerConnection<byte[]>;
                        DisconnectClient(connection?.ConnectionId);
                        break;
                }

                evArgs.AcceptSocket?.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, EndReceive, ListenArgs);
            }
            catch (Exception ex)
            {
                OnErrorInvoke(new ServerGenericEventArgs<Exception>
                {
                    DataReceived = ex
                });
                evArgs.AcceptSocket = null;
            }
        }

        protected virtual void ReceiveInvoke(ServerGenericEventArgs<byte[]> e)
        {
            OnReceive?.Invoke(this, e);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using QuaternionProtocol;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using Transport;
using Transport.Client;
using Transport.CommandsBase;
using Transport.Connections;
using Transport.Events;

namespace ClientExtensionsBase.Binary
{
    public abstract class ClientSocketBase : ClientTransportBase<byte[]>
    {
        protected readonly Dictionary<byte[], CommandContainer<IClientCommand>> Commands = new Dictionary<byte[], CommandContainer<IClientCommand>>();
        protected readonly Socket Socket;
        protected bool IsConnected;
        public readonly TransportStats Stats;
        private readonly CircularBuffer<byte> _messagesBuffer = new CircularBuffer<byte>(2000);

        protected ClientSocketBase(Socket initializedSocket, ProtocolBase<byte[]> protocol)
        {
            if (initializedSocket == null) throw new ArgumentNullException(nameof(initializedSocket));
            if (protocol == null) throw new ArgumentNullException(nameof(protocol));
            Socket = initializedSocket;
            Stats = new TransportStats(TransportName);
        }

        public override string Address => Socket.LocalEndPoint.ToString();
        public override OperationState SendData<T>(ProtocolBase<T> data)
        {
            var protoBytes = data as ProtocolBase<byte[]>;

            if (protoBytes != null)
            {
                using (var str = new MemoryStream(protoBytes.GetDataToSend()))
                {
                    return SendData(str);
                }
            }

            return new OperationState { IsSuccessful = false, Details = "ProtocolBase<byte[]> is correct type of Protocol" };
        }

        public override OperationState SendData(Stream data)
        {
            if (IsConnected)
                try
                {
                    var dataToSend = StreamHelper.GetBytes(data);
                    Socket.BeginSend(dataToSend, 0, dataToSend.Length, SocketFlags.None, SendDataCallback, null);

                    return new OperationState { IsSuccessful = true };
                }
                catch (Exception ex)
                {
                    return new OperationState { IsSuccessful = false, Details = ex.ToString() };
                }
            return new OperationState { IsSuccessful = false, Details = "Socket not connected" };
        }

        private void SendDataCallback(IAsyncResult ar)
        {
            var bytes = Socket.EndSend(ar);
            Stats.AddBytesSent(bytes);
        }

        public override OperationState RegisterCommand<T>(string commandName, T executeAction)
        {
            try
            {
                var cmdNameBytes = StreamHelper.WrapBytes(HashHelper.GeHash(commandName), 6);
                var cmdContainer = new CommandContainer<IClientCommand> { CommandRef = executeAction, CommandType = typeof(T) };
                Commands.Add(cmdNameBytes, cmdContainer);
            }
            catch (Exception wx)
            {
                return new OperationState { Details = wx.ToString(), IsSuccessful = false };
            }
            return new OperationState { IsSuccessful = true };
        }

        public override OperationState Connect(string endpointAddress)
        {
            try
            {
                var adr = endpointAddress.Split(':');
                Socket.BeginConnect(adr[0], int.Parse(adr[1]), ar =>
                {
                    Socket.EndConnect(ar);
                    IsConnected = true;
                    var buff = new byte[1024];
                    Socket.BeginReceive(buff, 0, 1024, SocketFlags.None, ReceiveCallback, buff);
                    RaiseConnected(new ClientGenericEventArgs<ClientConnectionBase>());
                }, null);
            }
            catch (Exception ex)
            {
                return new OperationState { IsSuccessful = false, Details = ex.ToString() };
            }

            return new OperationState { IsSuccessful = true };
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var buff = (byte[])ar.AsyncState;
            try
            {
                var bytesCount = Socket.EndReceive(ar);
                Stats.AddBytesReceived(bytesCount);

                if (bytesCount > 0)
                {
                    var bytesReceived = new byte[bytesCount];
                    Buffer.BlockCopy(buff, 0, bytesReceived, 0, bytesCount);

                    _messagesBuffer.Put(bytesReceived);

                    while (_messagesBuffer.Size > 4)
                    {
                        var messageLength = BinaryProtocol.GetMessageLength(_messagesBuffer.Peek(4));
                        if (_messagesBuffer.Size >= messageLength)
                        {
                            var messageBytes = _messagesBuffer.Get(messageLength);
                            var proto = new BinaryProtocol(messageBytes);
                            var cmdName = proto.GetCommandBytes();
                            CommandContainer<IClientCommand> command;

                            //if (Commands.TryGetValue(cmdName, out command))
                            //{
                            //    command.CommandRef.Execute(null, );
                            //    command.CommandRef
                            //}
                            
                            RaiseReceive(new ClientGenericEventArgs<byte[]> { DataReceived = bytesReceived, Protocol = proto });
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                Socket.BeginReceive(buff, 0, 1024, SocketFlags.None, ReceiveCallback, buff);
            }
            catch (Exception ex)
            {
                RaiseError(new ClientGenericEventArgs<Exception> { DataReceived = ex });
            }
        }

        public override void Dispose()
        {
            Socket.Shutdown(SocketShutdown.Both);
        }

    }
}
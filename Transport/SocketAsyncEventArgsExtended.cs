using System;
using System.Net.Sockets;
using System.Threading;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;

namespace Transport
{
    public class SocketAsyncEventArgsExtended : SocketAsyncEventArgs
    {
        public readonly ManualResetEvent WhenSocketReady = new ManualResetEvent(true);
        public DateTime LastResponse = DateTime.UtcNow;
        private SocketState _socketState;
        public SocketState SocketState
        {
            get { return _socketState; }
            set
            {
                if (value == SocketState.Idle) WhenSocketReady.Set();
                else
                {
                    WhenSocketReady.Reset();
                }

                _socketState = value;
            }
        }
        public readonly TransportStats CurrentSocketStats = new TransportStats("SocketServer");
        public readonly CircularBuffer<byte> BytesBuffer = new CircularBuffer<byte>(1500);
    }

    public enum SocketState
    {
        Idle=0, Busy=1 
    }
}
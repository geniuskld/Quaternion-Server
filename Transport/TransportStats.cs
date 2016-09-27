namespace Transport
{
    public class TransportStats
    {
        private string _transportName;

        public TransportStats(string transportName)
        {
            _transportName = transportName;
        }

        public int SentKb { get; protected set; }
        public int ReceivedKb { get; protected set; }

        public int TotalKb => SentKb + ReceivedKb;

        public void AddBytesSent(int sentInBytes)
        {
            SentKb += sentInBytes/1024;
        }

        public void AddBytesReceived(int receivedInBytes)
        {
            ReceivedKb += receivedInBytes/1024;
        }
    }
}
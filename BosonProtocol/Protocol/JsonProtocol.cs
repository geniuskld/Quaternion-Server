using System.Net.NetworkInformation;

namespace QuaternionProtocol.Protocol
{
    public class JsonProtocol: ProtocolBase<string>
    {
        public JsonProtocol(string data)
        {
            Data = data;
        }
        public override string GetDataToSend()
        {
            return Data;
        }

        public override string CleanData(string receivedData)
        {
            return receivedData;
        }

        public override string GetBody()
        {
            return Data;
        }
    }
}
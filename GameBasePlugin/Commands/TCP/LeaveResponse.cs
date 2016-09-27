using System;
using System.Runtime.Serialization;

namespace GameBasePlugin.Commands.TCP
{
    [DataContract, Serializable]
    public class LeaveResponse
    {
        [DataMember]
        public bool IsOk { get; set; }
        [DataMember]
        public string OperationDetails { get; set; }
        [DataMember]
        public string ConnectionId { get; set; }
    }
}
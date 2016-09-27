using System;
using System.Runtime.Serialization;

namespace GameBasePlugin.Commands.TCP
{
    [DataContract, Serializable]
    public class JoinResponse
    {
        public bool IsOk { get; set; }
        public string ConnectionId { get; set; }
    }
}
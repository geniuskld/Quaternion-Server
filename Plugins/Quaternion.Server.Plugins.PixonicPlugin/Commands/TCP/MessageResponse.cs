using System;
using System.Runtime.Serialization;

namespace Quaternion.Server.Plugins.PixonicPlugin.Commands.TCP
{
    [DataContract, Serializable]
    public class MessageResponse
    {
        public string Message { get; set; }
    }
}
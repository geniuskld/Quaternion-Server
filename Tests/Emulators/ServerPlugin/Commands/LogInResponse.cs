using ProtoBuf;

namespace Tests.Emulators.ServerPlugin.Commands
{
    [ProtoContract]
    public class LogInResponse
    {
        [ProtoMember(1)]
        public bool IsOk;
    }
}
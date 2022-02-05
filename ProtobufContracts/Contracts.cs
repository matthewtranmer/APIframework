using ProtoBuf;

namespace APIcontroller.Protobuf
{
    [ProtoContract]
    public class Login
    {
        [ProtoMember(1)]
        public string URI { get; set; }

        [ProtoMember(2)]
        public string Username { get; set; }
    }
}

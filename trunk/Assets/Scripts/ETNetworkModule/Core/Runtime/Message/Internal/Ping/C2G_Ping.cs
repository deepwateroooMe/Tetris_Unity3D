using ProtoBuf;
namespace ET {

    [ResponseType(nameof(G2C_Ping))] // 返回类型： G2C_Ping. 与网关服的交换消息
    [Message(Opcode.C2G_Ping)]       // 消息类型
    [ProtoContract]
    public partial class C2G_Ping : Object, IRequest {

        [ProtoMember(90)]
        public int RpcId { get; set; }
    }
}
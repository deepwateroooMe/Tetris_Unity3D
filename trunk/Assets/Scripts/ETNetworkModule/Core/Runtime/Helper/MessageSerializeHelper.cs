using System;
using System.IO;
namespace ET { // 项目默认使用的是： Protobuf 消息序列化与反序列化. 拿出来的最基本的方法 

    public static class MessageSerializeHelper {
        private const string TAG = "MessageSerializeHelper";

        public static object DeserializeFrom(ushort opcode, Type type, MemoryStream memoryStream) {
            if (opcode < OpcodeRangeDefine.PbMaxOpcode) {
                return ProtobufHelper.FromStream(type, memoryStream);
            }
            throw new Exception($"client no message: {opcode}");
        }
        public static void SerializeTo(ushort opcode, object obj, MemoryStream memoryStream) {
            try {
                if (opcode < OpcodeRangeDefine.PbMaxOpcode) {
                    ProtobufHelper.ToStream(obj, memoryStream); // 就是说，把这个消息写进这个内存流里
                    return;
                }
                throw new Exception($"client no message: {opcode}");
            }
            catch (Exception e) {
                throw new Exception($"SerializeTo error: {opcode}", e);
            }
        }
        public static MemoryStream GetStream(int count = 0) {
            MemoryStream stream;
            if (count > 0) {
                stream = new MemoryStream(count);
            } else {
                stream = new MemoryStream();
            }
            return stream;
        }
        public static (ushort, MemoryStream) MessageToStream(object message) {
            if (!OpcodeManager.TryGetOpcode(message.GetType(), out var opcode)) {
                throw new Exception($"消息 {message.GetType().Name} 未指定 opcode !");
            }
            int headOffset = Packet.ActorIdLength; // 8
            MemoryStream stream = GetStream(headOffset + Packet.OpcodeLength); // 10
            stream.Seek(headOffset + Packet.OpcodeLength, SeekOrigin.Begin);
            stream.SetLength(headOffset + Packet.OpcodeLength); // 这个长度为10 的内存流，写的算是什么内容呢： ActorID ＋ Opcode, 但是消息体呢？
            stream.GetBuffer().WriteTo(headOffset, opcode);     // 在正确的下标位置 [headOffset] 处，将 Opcode 的值（2 个字节）写进去
            SerializeTo(opcode, message, stream); // 说的是，把消息内容写进了内存流里
            stream.Seek(0, SeekOrigin.Begin); // 重新把流指针放回到头的位置
            return (opcode, stream);
        }
    }
}
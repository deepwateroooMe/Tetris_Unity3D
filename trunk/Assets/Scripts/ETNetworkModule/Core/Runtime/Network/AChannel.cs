using System;
using System.IO;
using System.Net;
namespace ET {
    public enum ChannelType { // 这里也是分门别类区分：连接信道，与接收信道
        Connect,
        Accept,
    }
    public struct Packet {
        public const int MinPacketSize = 2; // 固定的下标，与长度
        public const int OpcodeIndex = 8;
        public const int KcpOpcodeIndex = 0;
        public const int OpcodeLength = 2;
        public const int ActorIdIndex = 0;
        public const int ActorIdLength = 8;
        public const int MessageIndex = 10;
        
        public ushort Opcode; // 可变，消息头变量 
        public long ActorId;
        public MemoryStream MemoryStream; // 使用的是，内存流上发消息，包裹里当然包括一个内存流
    }
    public abstract class AChannel: IDisposable { // 抽象信道
        public long Id;
        public ChannelType ChannelType { get; protected set; } // 类型

        public int Error { get; set; }
        public IPEndPoint RemoteAddress { get; set; } // 远程另一端
        
        public bool IsDisposed { // 是否回收了
            get {
                return this.Id == 0;    
            }
        }
        public abstract void Dispose();
    }
}
using System;
using System.Collections.Generic;
using MessagePack;

namespace Server.Protocol.PacketResponse
{
    public class InitialHandshakeProtocol : IPacketResponse
    {
        public List<List<byte>> GetResponse(List<byte> payload)
        {
            
            //TODO
            return new List<List<byte>>();
        }
    }
    
    
    
    [MessagePackObject(keyAsPropertyName :true)]
    public class InitialHandshakeRequestMessagePack : ProtocolMessagePackBase
    {
        [Obsolete("デシリアライズ用のコンストラクタです。基本的に使用しないでください。")]
        public InitialHandshakeRequestMessagePack()
        {
        }

        public InitialHandshakeRequestMessagePack(int playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
        }

        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
    }
}
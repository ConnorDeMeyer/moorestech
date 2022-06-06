using System;
using System.Collections.Generic;
using System.Linq;
using Game.World.Interface.Event;
using MessagePack;
using Server.Util;

namespace Server.Event.EventReceive
{
    public class PlaceBlockToSetEventPacket
    {
        public const string EventTag = "va:event:blockPlace";
        private readonly EventProtocolProvider _eventProtocolProvider;

        public PlaceBlockToSetEventPacket(IBlockPlaceEvent blockPlaceEvent, EventProtocolProvider eventProtocolProvider)
        {
            blockPlaceEvent.Subscribe(ReceivedEvent);
            _eventProtocolProvider = eventProtocolProvider;
        }

        private void ReceivedEvent(BlockPlaceEventProperties blockPlaceEventProperties)
        {
            var c = blockPlaceEventProperties.Coordinate;
            var blockId = blockPlaceEventProperties.Block.BlockId;
            
            var payload = MessagePackSerializer.Serialize(new PlaceBlockEventMessagePack(
                c.X,c.Y,blockId
            )).ToList();;
            
            _eventProtocolProvider.AddBroadcastEvent(payload);
        }
    }
    
        
    [MessagePackObject(keyAsPropertyName :true)]
    public class PlaceBlockEventMessagePack : EventProtocolMessagePackBase
    {
        [Obsolete("デシリアライズ用のコンストラクタです。基本的に使用しないでください。")]
        public PlaceBlockEventMessagePack() { }

        public PlaceBlockEventMessagePack(int x, int y, int blockId)
        {
            EventTag = PlaceBlockToSetEventPacket.EventTag;
            X = x;
            Y = y;
            BlockId = blockId;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int BlockId { get; set; }
    }
}
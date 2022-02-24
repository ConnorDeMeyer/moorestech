﻿using System.Collections.Generic;
using MainGame.Network.Event;
using MainGame.Network.Receive.EventPacket;
using MainGame.Network.Util;
using UnityEngine;

namespace MainGame.Network.Receive
{
    public class ReceiveEventProtocol : IAnalysisPacket
    {
        List<IAnalysisEventPacket> _eventPacketList = new List<IAnalysisEventPacket>();

        public ReceiveEventProtocol(INetworkReceivedChunkDataEvent networkReceivedChunkDataEvent,IPlayerInventoryUpdateEvent playerInventoryUpdateEvent)
        {
            _eventPacketList.Add(new BlockPlaceEvent(networkReceivedChunkDataEvent));
            _eventPacketList.Add(new PlayerInventorySlotEvent(playerInventoryUpdateEvent));
        }
        
        /// <summary>
        /// イベントのパケットを受け取り、さらに個別の解析クラスに渡す
        /// </summary>
        /// <param name="data"></param>
        public void Analysis(List<byte> data)
        {
            var bytes = new ByteArrayEnumerator(data);
            bytes.MoveNextToGetShort();
            var eventId = bytes.MoveNextToGetShort();
            _eventPacketList[eventId].Analysis(data);
            
            Debug.Log("Event ID " + eventId + " " + _eventPacketList[eventId].GetType().Name);
        }
    }
}
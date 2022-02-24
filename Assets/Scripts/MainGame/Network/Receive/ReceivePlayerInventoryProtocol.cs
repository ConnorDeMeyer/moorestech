﻿using System.Collections.Generic;
using MainGame.Basic;
using MainGame.Network.Event;
using MainGame.Network.Util;

namespace MainGame.Network.Receive
{
    /// <summary>
    /// Analysis player inventory data 
    /// </summary>
    public class ReceivePlayerInventoryProtocol : IAnalysisPacket
    {
        public ReceivePlayerInventoryProtocol(
            IPlayerInventoryUpdateEvent playerInventoryUpdateEvent)
        {
            _playerInventoryUpdateEvent = playerInventoryUpdateEvent as PlayerInventoryUpdateEvent;
        }

        private readonly PlayerInventoryUpdateEvent _playerInventoryUpdateEvent;

        public void Analysis(List<byte> data)
        {
            var bytes = new ByteArrayEnumerator(data);
            //packet id
            bytes.MoveNextToGetShort();
            //player id
            var playerId = bytes.MoveNextToGetInt();
            //padding
            bytes.MoveNextToGetShort();
            var mainItems = new List<ItemStack>();
            for (int i = 0; i < PlayerInventoryConstant.MainInventorySize; i++)
            {
                var id = bytes.MoveNextToGetInt();
                var count = bytes.MoveNextToGetInt();
                mainItems.Add(new ItemStack(id, count));
            }
            
            _playerInventoryUpdateEvent.OnOnPlayerInventoryUpdateEvent(
                new PlayerInventoryUpdateProperties(
                    playerId,
                    mainItems));
        }
    }
}
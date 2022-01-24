﻿using UnityEngine;

namespace MainGame.Network.Interface.Send
{
    public interface ISendBlockInventoryPlayerInventoryMoveItemProtocol
    {
        public void Send(bool toBlock,
            int playerId, int playerInventorySlot,
            Vector2Int blockPosition, int blockInventorySlot,
            int itemCount);
    }
}
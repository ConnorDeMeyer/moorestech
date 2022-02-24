﻿using System.Collections;
using MainGame.Control.UI.Inventory;
using MainGame.GameLogic;
using MainGame.GameLogic.Inventory;
using MainGame.Network;
using MainGame.Network.Event;
using MainGame.Network.Send;
using MainGame.UnityView;
using MainGame.UnityView.UI.Inventory.View;
using UnityEngine;

namespace Test.TestModule.UI
{
    public class PlayerInventoryInputTest : MonoBehaviour
    {
        [SerializeField] private PlayerInventoryInput playerInventoryInput;
        [SerializeField] private PlayerInventoryEquippedItemImageSet playerInventoryEquippedItemImageSet;

        [SerializeField] private PlayerInventoryItemView playerInventoryItem;
        [SerializeField] private BlockInventoryItemView blockInventoryItem;

        private void Start()
        {
            var playerInventory = GetComponent<InventoryViewTestModule>().PlayerInventoryDataCache;
            var itemMove = new BlockInventoryPlayerInventoryItemMoveService(
                new PlayerConnectionSetting(0),
                new BlockInventoryDataCache(new BlockInventoryUpdateEvent(),blockInventoryItem),
                playerInventory,
                new SendBlockInventoryMoveItemProtocol(new TestSocketModule()),
                new SendBlockInventoryPlayerInventoryMoveItemProtocol(new TestSocketModule()),
                new SendPlayerInventoryMoveItemProtocol(new TestSocketModule()));
            
            playerInventoryEquippedItemImageSet.Construct(playerInventoryItem,new PlayerInventoryUpdateEvent());
            playerInventoryInput.Construct(playerInventoryItem,itemMove,playerInventory,playerInventoryEquippedItemImageSet);
        }
    }
}
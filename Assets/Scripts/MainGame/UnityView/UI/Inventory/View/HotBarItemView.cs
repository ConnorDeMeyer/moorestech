﻿using System.Collections.Generic;
using MainGame.Basic;
using MainGame.UnityView.UI.Inventory.Element;
using UnityEngine;
using VContainer;

namespace MainGame.UnityView.UI.Inventory.View
{
    public class HotBarItemView : MonoBehaviour
    {
        
        [SerializeField] private InventoryItemSlot inventoryItemSlot;
        List<InventoryItemSlot> _slots;
        public IReadOnlyList<InventoryItemSlot> Slots => _slots;
        
        private ItemImages _itemImages;
        
        
        [Inject]
        public void Construct(ItemImages itemImages)
        {
            _itemImages = itemImages;
            _slots = new List<InventoryItemSlot>();
            for (int i = 0; i < PlayerInventoryConstant.MainInventoryColumns; i++)
            {
                _slots.Add(Instantiate(inventoryItemSlot.gameObject,transform).GetComponent<InventoryItemSlot>());
            }
        }

        public void OnInventoryUpdate(int slot, int itemId, int count)
        {
            //スロットが一番下の段でなければスルー
            var c = PlayerInventoryConstant.MainInventoryColumns;
            var r = PlayerInventoryConstant.MainInventoryRows;
            var startHotBarSlot = c * (r - 1);
            
            if (slot < startHotBarSlot) return;
            
            var sprite = _itemImages.GetItemImage(itemId);
            slot -= startHotBarSlot;
            _slots[slot].SetItem(sprite,count);

        }
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MainGame.Basic;

namespace MainGame.Network.Event
{
    public class CraftingInventoryUpdateEvent
    {
        public event Action<CraftingInventoryUpdateProperties> OnCraftingInventoryUpdate;
        public event Action<CraftingInventorySlotUpdateProperties> OnCraftingInventorySlotUpdate;

        internal void InvokeCraftingInventorySlotUpdate(CraftingInventorySlotUpdateProperties properties)
        {
            InvokeCraftingInventorySlotUpdateAsync(properties).Forget();
        }
        private async UniTask InvokeCraftingInventorySlotUpdateAsync(CraftingInventorySlotUpdateProperties properties)
        {
            await UniTask.SwitchToMainThread();
            OnCraftingInventorySlotUpdate?.Invoke(properties);
        }
        
        

        internal void InvokeCraftingInventoryUpdate(CraftingInventoryUpdateProperties properties)
        {
            InvokeCraftingInventoryUpdateAsync(properties).Forget();
        }
        private async UniTask InvokeCraftingInventoryUpdateAsync(CraftingInventoryUpdateProperties properties)
        {
            await UniTask.SwitchToMainThread();
            OnCraftingInventoryUpdate?.Invoke(properties);
        }
    }
    
    
    public class CraftingInventoryUpdateProperties
    {
        public readonly int PlayerId;
        public readonly List<ItemStack> ItemStacks;
        public readonly ItemStack ResultItemStack;
        public readonly bool CanCraft;

        public CraftingInventoryUpdateProperties(int playerId, bool canCraft, List<ItemStack> itemStacks, ItemStack resultItemStack)
        {
            PlayerId = playerId;
            ItemStacks = itemStacks;
            ResultItemStack = resultItemStack;
            CanCraft = canCraft;
        }
    }

    public class CraftingInventorySlotUpdateProperties
    {
        public readonly int SlotId;
        public readonly ItemStack ItemStack;
        public readonly ItemStack ResultItemStack;
        public readonly bool CanCraft;

        public CraftingInventorySlotUpdateProperties(int slotId, ItemStack itemStack,ItemStack resultItemStack,bool canCraft)
        {
            SlotId = slotId;
            ItemStack = itemStack;
            ResultItemStack = resultItemStack;
            CanCraft = canCraft;
        }
    }
}
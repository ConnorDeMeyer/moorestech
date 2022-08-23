using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using MainGame.Basic;
using MainGame.UnityView.Block;
using MainGame.UnityView.UI.Builder;
using MainGame.UnityView.UI.Builder.Unity;
using MainGame.UnityView.UI.Inventory.Element;
using MainGame.UnityView.UI.Inventory.View;
using TMPro;
using UnityEngine;
using VContainer;

namespace MainGame.UnityView.UI.CraftRecipe
{
    public class ItemRecipeView : MonoBehaviour
    {
        private ItemImages _itemImages;
        private BlockObjects _blockObjects;
        
        [SerializeField] private GameObject craftingRecipeView;
        [SerializeField] private GameObject machineCraftingRecipeView;
        
        [SerializeField] private List<UIBuilderItemSlotObject> craftingRecipeSlots;
        [SerializeField] private UIBuilderItemSlotObject CraftingResultSlotObject;
        
        [SerializeField] private List<UIBuilderItemSlotObject> machineCraftingRecipeSlots;
        [SerializeField] private UIBuilderItemSlotObject MachineCraftingResultSlotObject;
        [SerializeField] private TMP_Text machineNameText;
        
        public event CraftRecipeItemListViewer.ItemSlotClick OnCraftSlotClick;

        public event Action<ItemStack> OnCursorEnter;
        public event Action<ItemStack> OnCursorExit;
        
        [Inject]
        public void Construct(ItemImages itemImages,BlockObjects blockObjects)
        {
            _blockObjects = blockObjects;
            _itemImages = itemImages;
            foreach (var slot in craftingRecipeSlots)
            {
                slot.OnLeftClickDown += OnClick;
                slot.OnCursorEnter += InvokeCursorEnter;
                slot.OnCursorExit += InvokeCursorExit;
            }
            foreach (var slot in machineCraftingRecipeSlots)
            {
                slot.OnLeftClickDown += OnClick;
                slot.OnCursorEnter += InvokeCursorEnter;
                slot.OnCursorExit += InvokeCursorExit;
            }
            CraftingResultSlotObject.OnCursorEnter += _ => OnCursorEnter?.Invoke(_craftResultItemStack);
            CraftingResultSlotObject.OnCursorExit += _ => OnCursorExit?.Invoke(_craftResultItemStack);
            MachineCraftingResultSlotObject.OnCursorEnter += _ => OnCursorEnter?.Invoke(_machineCraftResultItemStack);
            MachineCraftingResultSlotObject.OnCursorExit += _ => OnCursorExit?.Invoke(_machineCraftResultItemStack);
        }
        
        
        private List<ItemStack> _craftItemStacks = new();
        private ItemStack _craftResultItemStack;
        
        public void SetCraftRecipe(List<ItemStack> itemStacks,ItemStack result)
        {
            craftingRecipeView.SetActive(true);
            machineCraftingRecipeView.SetActive(false);

            for (int i = 0; i < craftingRecipeSlots.Count; i++)
            {
                var item = itemStacks[i];
                craftingRecipeSlots[i].SetItem(_itemImages.GetItemView(item.ID),item.Count);
            }
            CraftingResultSlotObject.SetItem(_itemImages.GetItemView(result.ID),result.Count);
            _craftResultItemStack = result;
            
            _craftItemStacks = itemStacks;
        }
        
        private List<ItemStack> _machineCraftItemStacks = new();
        private ItemStack _machineCraftResultItemStack;
        
        public void SetMachineCraftRecipe(List<ItemStack> itemStacks,ItemStack result,int blockId)
        {
            craftingRecipeView.SetActive(false);
            machineCraftingRecipeView.SetActive(true);

            machineNameText.text = _blockObjects.GetName(blockId);
            
            for (int i = 0; i < machineCraftingRecipeSlots.Count; i++)
            {
                if (itemStacks.Count <= i)
                {
                    machineCraftingRecipeSlots[i].gameObject.SetActive(false);
                    continue;
                }
                
                machineCraftingRecipeSlots[i].gameObject.SetActive(true);
                var item = itemStacks[i];
                machineCraftingRecipeSlots[i].SetItem(_itemImages.GetItemView(item.ID),item.Count);
            }
            _machineCraftResultItemStack = result;
            MachineCraftingResultSlotObject.SetItem(_itemImages.GetItemView(result.ID),result.Count);
            _machineCraftItemStacks = itemStacks;
        }

        private void OnClick(UIBuilderItemSlotObject uiBuilderItemSlotObject) { OnCraftSlotClick?.Invoke(GetItemStack(uiBuilderItemSlotObject).ID); }
        private void InvokeCursorEnter(UIBuilderItemSlotObject uiBuilderItemSlotObject) { OnCursorEnter?.Invoke(GetItemStack(uiBuilderItemSlotObject)); }
        private void InvokeCursorExit(UIBuilderItemSlotObject uiBuilderItemSlotObject) { OnCursorExit?.Invoke(GetItemStack(uiBuilderItemSlotObject)); }


        private ItemStack GetItemStack(UIBuilderItemSlotObject uiBuilderItemSlotObject)
        {
            var craftIndex = craftingRecipeSlots.IndexOf(uiBuilderItemSlotObject);
            var machineCraftIndex = machineCraftingRecipeSlots.IndexOf(uiBuilderItemSlotObject);
            
            if (craftIndex != -1)
            {
                return _craftItemStacks[craftIndex];
            }
            else if (machineCraftIndex != -1)
            {
                return _machineCraftItemStacks[machineCraftIndex];
            }

            return new ItemStack();
        }
    }
}
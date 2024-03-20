﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.EnergySystem;
using Core.Inventory;
using Core.Item;
using Game.Block.Interface;
using Game.Block.BlockInventory;
using Game.Block.Blocks.Machine.InventoryController;
using Game.Block.Blocks.Machine.SaveLoad;
using Game.Block.Interface;
using Game.Block.Interface.State;
using UniRx;

namespace Game.Block.Blocks.Machine
{
    /// <summary>
    ///     機械を表すクラス
    ///     具体的な処理は各コンポーネントに任せて、このクラスはInterfaceの実装だけを行う
    /// </summary>
    public abstract class VanillaMachineBase : IBlock, IBlockInventory, IEnergyConsumer, IOpenableInventory
    {
        public IBlockComponentManager ComponentManager { get; } = new BlockComponentManager();
        public IObservable<ChangedBlockState> BlockStateChange => _vanillaMachineRunProcess.ChangeState;

        private readonly ItemStackFactory _itemStackFactory;
        private readonly VanillaMachineBlockInventory _vanillaMachineBlockInventory;
        private readonly VanillaMachineRunProcess _vanillaMachineRunProcess;
        private readonly VanillaMachineSave _vanillaMachineSave;

        protected VanillaMachineBase(int blockId, int entityId, long blockHash,
            VanillaMachineBlockInventory vanillaMachineBlockInventory,
            VanillaMachineSave vanillaMachineSave, VanillaMachineRunProcess vanillaMachineRunProcess,
            ItemStackFactory itemStackFactory)
        {
            BlockId = blockId;
            _vanillaMachineBlockInventory = vanillaMachineBlockInventory;
            _vanillaMachineSave = vanillaMachineSave;
            _vanillaMachineRunProcess = vanillaMachineRunProcess;
            _itemStackFactory = itemStackFactory;
            BlockHash = blockHash;
            EntityId = entityId;
        }

        public int EntityId { get; }
        public int BlockId { get; }
        public long BlockHash { get; }


        #region IBlock implementation

        public string GetSaveState()
        {
            return _vanillaMachineSave.Save();
        }


        #endregion


        #region IBlockInventory

        public IItemStack InsertItem(IItemStack itemStack)
        {
            return _vanillaMachineBlockInventory.InsertItem(itemStack);
        }

        public IItemStack InsertItem(int itemId, int count)
        {
            return _vanillaMachineBlockInventory.InsertItem(_itemStackFactory.Create(itemId, count));
        }

        public List<IItemStack> InsertItem(List<IItemStack> itemStacks)
        {
            return _vanillaMachineBlockInventory.InsertItem(itemStacks);
        }

        public bool InsertionCheck(List<IItemStack> itemStacks)
        {
            return _vanillaMachineBlockInventory.InsertionCheck(itemStacks);
        }


        public void AddOutputConnector(IBlockInventory blockInventory)
        {
            _vanillaMachineBlockInventory.AddConnector(blockInventory);
        }

        public void RemoveOutputConnector(IBlockInventory blockInventory)
        {
            _vanillaMachineBlockInventory.RemoveConnector(blockInventory);
        }

        #endregion


        #region IOpenableInventory implementation

        public ReadOnlyCollection<IItemStack> Items => _vanillaMachineBlockInventory.Items;

        public IItemStack GetItem(int slot)
        {
            return _vanillaMachineBlockInventory.GetItem(slot);
        }

        public void SetItem(int slot, IItemStack itemStack)
        {
            _vanillaMachineBlockInventory.SetItem(slot, itemStack);
        }

        public void SetItem(int slot, int itemId, int count)
        {
            _vanillaMachineBlockInventory.SetItem(slot, _itemStackFactory.Create(itemId, count));
        }

        public IItemStack ReplaceItem(int slot, IItemStack itemStack)
        {
            return _vanillaMachineBlockInventory.ReplaceItem(slot, itemStack);
        }

        public IItemStack ReplaceItem(int slot, int itemId, int count)
        {
            return ReplaceItem(slot, _itemStackFactory.Create(itemId, count));
        }

        public int GetSlotSize()
        {
            return _vanillaMachineBlockInventory.GetSlotSize();
        }

        #endregion


        #region IBlockElectric implementation

        public int RequestEnergy => _vanillaMachineRunProcess.RequestPower;

        public void SupplyEnergy(int power)
        {
            _vanillaMachineRunProcess.SupplyPower(power);
        }

        #endregion
    }
}
﻿using System;
using System.Text;
using Core.Const;
using Core.Item.Interface;
using Core.Update;
using Game.Block.BlockInventory;
using Game.Block.Component;
using Game.Block.Component.IOConnector;
using Game.Block.Interface;
using Game.Block.Interface.State;
using Game.Context;
using UniRx;

namespace Game.Block.Blocks.BeltConveyor
{
    /// <summary>
    ///     アイテムの搬出入とインベントリの管理を行う
    /// </summary>
    public class VanillaBeltConveyor : IBlock, IBlockInventory
    {
        private readonly BlockComponentManager _blockComponentManager = new();
        private readonly BeltConveyorInventoryItem[] _inventoryItems;

        private readonly Subject<ChangedBlockState> _onBlockStateChange = new();
        public readonly int InventoryItemNum;

        public readonly double TimeOfItemEnterToExit; //ベルトコンベアにアイテムが入って出るまでの時間

        public VanillaBeltConveyor(int blockId, int entityId, long blockHash, int inventoryItemNum, int timeOfItemEnterToExit, BlockPositionInfo blockPositionInfo)
        {
            EntityId = entityId;
            BlockId = blockId;
            InventoryItemNum = inventoryItemNum;
            TimeOfItemEnterToExit = timeOfItemEnterToExit;
            BlockPositionInfo = blockPositionInfo;
            BlockHash = blockHash;

            _inventoryItems = new BeltConveyorInventoryItem[inventoryItemNum];

            GameUpdater.UpdateObservable.Subscribe(_ => Update());

            var component = new InventoryInputConnectorComponent(new IOConnectionSetting(
                // 南、西、東をからの接続を受け、アイテムをインプットする
                new ConnectDirection[] { new(-1, 0, 0), new(0, 1, 0), new(0, -1, 0) },
                //北向きに出力する
                new ConnectDirection[] { new(1, 0, 0) },
                new[]
                {
                    VanillaBlockType.Machine, VanillaBlockType.Chest, VanillaBlockType.Generator,
                    VanillaBlockType.Miner, VanillaBlockType.BeltConveyor,
                }), blockPositionInfo);
            _blockComponentManager.AddComponent(component);
        }

        public VanillaBeltConveyor(int blockId, int entityId, long blockHash, string state, int inventoryItemNum, int timeOfItemEnterToExit, BlockPositionInfo blockPositionInfo) :
            this(blockId, entityId, blockHash, inventoryItemNum, timeOfItemEnterToExit, blockPositionInfo)
        {
            //stateから復元
            //データがないときは何もしない
            if (state == string.Empty) return;
            var stateList = state.Split(',');
            for (var i = 0; i < _inventoryItems.Length; i++)
            {
                var saveIndex = i * 2;
                var id = int.Parse(stateList[saveIndex]);
                var remainTime = double.Parse(stateList[saveIndex + 1]);
                if (id == -1) continue;

                _inventoryItems[i] = new BeltConveyorInventoryItem(id, remainTime, ItemInstanceIdGenerator.Generate());
            }
        }

        public IBlockComponentManager ComponentManager => _blockComponentManager;

        public BlockPositionInfo BlockPositionInfo { get; }

        public IObservable<ChangedBlockState> BlockStateChange => _onBlockStateChange;

        public int EntityId { get; }
        public int BlockId { get; }
        public long BlockHash { get; }

        public string GetSaveState()
        {
            if (_inventoryItems.Length == 0) return string.Empty;

            //stateの定義 ItemId,RemainingTime,LimitTime,InstanceId...
            var state = new StringBuilder();
            foreach (var t in _inventoryItems)
            {
                if (t == null)
                {
                    state.Append("-1,-1,");
                    continue;
                }

                state.Append(t.ItemId);
                state.Append(',');
                state.Append(t.RemainingTime);
                state.Append(',');
            }

            //最後のカンマを削除
            state.Remove(state.Length - 1, 1);
            return state.ToString();
        }



        public bool Equals(IBlock other)
        {
            if (other is null) return false;
            return EntityId == other.EntityId && BlockId == other.BlockId && BlockHash == other.BlockHash;
        }

        public IItemStack InsertItem(IItemStack itemStack)
        {
            //新しく挿入可能か
            if (_inventoryItems[^1] != null)
                //挿入可能でない
                return itemStack;

            _inventoryItems[^1] = new BeltConveyorInventoryItem(itemStack.Id, TimeOfItemEnterToExit, itemStack.ItemInstanceId);

            //挿入したのでアイテムを減らして返す
            return itemStack.SubItem(1);
        }

        public int GetSlotSize()
        {
            return _inventoryItems.Length;
        }

        public IItemStack GetItem(int slot)
        {
            return ServerContext.ItemStackFactory.Create(_inventoryItems[slot].ItemId, 1);
        }

        public void SetItem(int slot, IItemStack itemStack)
        {
            //TODO lockすべき？？
            _inventoryItems[slot] = new BeltConveyorInventoryItem(itemStack.Id, TimeOfItemEnterToExit, itemStack.ItemInstanceId);
        }

        /// <summary>
        ///     アイテムの搬出判定を行う
        ///     判定はUpdateで毎フレーム行われる
        ///     TODO 個々のマルチスレッド対応もいい感じにやりたい
        /// </summary>
        private void Update()
        {
            //TODO lockすべき？？
            var count = _inventoryItems.Length;

            for (var i = 0; i < count; i++)
            {
                var item = _inventoryItems[i];
                if (item == null) continue;

                //次のインデックスに入れる時間かどうかをチェックする
                var nextIndexStartTime = i * (TimeOfItemEnterToExit / InventoryItemNum);
                var isNextInsertable = item.RemainingTime <= nextIndexStartTime;

                //次に空きがあれば次に移動する
                if (isNextInsertable && i != 0)
                {
                    if (_inventoryItems[i - 1] == null)
                    {
                        _inventoryItems[i - 1] = item;
                        _inventoryItems[i] = null;
                    }

                    continue;
                }

                //最後のアイテムの場合は接続先に渡す
                if (i == 0 && item.RemainingTime <= 0)
                {
                    var insertItem = ServerContext.ItemStackFactory.Create(item.ItemId, 1, item.ItemInstanceId);

                    var inputConnector = ComponentManager.GetComponent<InventoryInputConnectorComponent>();
                    if (inputConnector.ConnectInventory.Count == 0) continue;

                    var connector = inputConnector.ConnectInventory[0];
                    var output = connector.InsertItem(insertItem);

                    //渡した結果がnullItemだったらそのアイテムを消す
                    if (output.Id == ItemConst.EmptyItemId) _inventoryItems[i] = null;

                    continue;
                }

                //時間を減らす 
                item.RemainingTime -= GameUpdater.UpdateMillSecondTime;
            }
        }

        public BeltConveyorInventoryItem GetBeltConveyorItem(int index)
        {
            return _inventoryItems[index];
        }

        public override bool Equals(object obj)
        {
            return obj is IBlock other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EntityId, BlockId, BlockHash);
        }
    }
}
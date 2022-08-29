using System;
using System.Collections.Generic;
using System.Text;
using Core.Inventory;
using Core.Item;
using Game.PlayerInventory.Interface;
using Game.World.Interface.DataStore;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Server.Protocol.PacketResponse.Util;
using Server.Util;

namespace Server.Protocol.PacketResponse
{
    /// <summary>
    /// インベントリでマウスを使ってアイテムの移動を操作するプロトコルです
    /// </summary>
    public class InventoryItemMoveProtocol : IPacketResponse
    {
        public const string Tag = "va:invItemMove";
        
        private readonly IWorldBlockComponentDatastore<IOpenableInventory> _openableBlockDatastore;
        private readonly IPlayerInventoryDataStore _playerInventoryDataStore;
        private readonly ItemStackFactory _itemStackFactory;

        public InventoryItemMoveProtocol(ServiceProvider serviceProvider)
        {
            _openableBlockDatastore = serviceProvider.GetService<IWorldBlockComponentDatastore<IOpenableInventory>>();
            _playerInventoryDataStore = serviceProvider.GetService<IPlayerInventoryDataStore>();
            _itemStackFactory = serviceProvider.GetService<ItemStackFactory>();
        }
        public List<List<byte>> GetResponse(List<byte> payload)
        {
            var data = MessagePackSerializer.Deserialize<InventoryItemMoveProtocolMessagePack>(payload.ToArray());
            
            var fromInventory = GetInventory(data.FromInventory.InventoryType, data.PlayerId, data.FromInventory.X, data.FromInventory.Y);
            if (fromInventory == null)return new List<List<byte>>();
            var toInventory = GetInventory(data.ToInventory.InventoryType, data.PlayerId, data.ToInventory.X, data.ToInventory.Y);
            if (toInventory == null)return new List<List<byte>>();


            InventoryItemMoveService.Move(
                    _itemStackFactory,fromInventory,data.FromInventory.Slot,toInventory,data.ToInventory.Slot,data.Count);

            return new List<List<byte>>();
        }

        private IOpenableInventory GetInventory(ItemMoveInventoryType inventoryType,int playerId, int x, int y)
        {
            IOpenableInventory inventory = null;
            switch (inventoryType)
            {
                case ItemMoveInventoryType.MainInventory:
                    inventory = _playerInventoryDataStore.GetInventoryData(playerId).MainOpenableInventory;
                    break;
                case ItemMoveInventoryType.CraftInventory:
                    inventory = _playerInventoryDataStore.GetInventoryData(playerId).CraftingOpenableInventory;
                    break;
                case ItemMoveInventoryType.GrabInventory:
                    inventory = _playerInventoryDataStore.GetInventoryData(playerId).GrabInventory;
                    break;
                case ItemMoveInventoryType.BlockInventory:
                    inventory = _openableBlockDatastore.ExistsComponentBlock(x,y) ? _openableBlockDatastore.GetBlock(x, y) : null;; 
                    break;
            }
            return inventory;
        }
        
        
    }

    
    [MessagePackObject(keyAsPropertyName :true)]
    public class InventoryItemMoveProtocolMessagePack : ProtocolMessagePackBase
    {
        [Obsolete("デシリアライズ用のコンストラクタです。基本的に使用しないでください。")]
        public InventoryItemMoveProtocolMessagePack() { }

        public InventoryItemMoveProtocolMessagePack(int playerId,int count, ItemMoveInventoryInfo fromInventory,ItemMoveInventoryInfo toInventory)
        {
            Tag = InventoryItemMoveProtocol.Tag;
            PlayerId = playerId;
            Count = count;
            
            FromInventory = new ItemMoveInventoryInfoMessagePack(fromInventory);
            ToInventory = new ItemMoveInventoryInfoMessagePack(toInventory);
        }

        public int PlayerId { get; set; }
        public int Count { get; set; }
        
        public ItemMoveInventoryInfoMessagePack FromInventory { get; set; }
        public ItemMoveInventoryInfoMessagePack ToInventory { get; set; }

    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ItemMoveInventoryInfoMessagePack
    {
        [Obsolete("シリアライズ用の値です。InventoryTypeを使用してください。")]
        public int InventoryId { get; set; }
        public ItemMoveInventoryType InventoryType => (ItemMoveInventoryType)Enum.ToObject(typeof(ItemMoveInventoryType), InventoryId);
        
        

        [Obsolete("シリアライズ用の値です。ItemMoveTypeを使用してください")]
        public int ItemMoveId { get; set; }
        public ItemMoveType ItemMoveType => (ItemMoveType)Enum.ToObject(typeof(ItemMoveType), ItemMoveId);
        
        
        public  int Slot{ get; set; }
        public  int X { get; set; }
        public  int Y { get; set; }
        
        [Obsolete("デシリアライズ用のコンストラクタです。基本的に使用しないでください。")]
        public  ItemMoveInventoryInfoMessagePack(){}
        
        public ItemMoveInventoryInfoMessagePack(ItemMoveInventoryInfo info)
        {
            //メッセージパックでenumは重いらしいのでintを使う
            InventoryId = (int)info.ItemMoveInventoryType;
            ItemMoveId = (int)info.ItemMoveType;
            Slot = info.Slot;
            X = info.X;
            Y = info.Y;
        }
    }
}
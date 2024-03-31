using System.Collections.Generic;
using System.IO;
using Core.ConfigJson;
using Core.EnergySystem;
using Core.EnergySystem.Electric;
using Core.Item;
using Core.Item.Config;
using Game.Block.Component;
using Game.Block.Config;
using Game.Block.Event;
using Game.Block.Factory;
using Game.Block.Interface;
using Game.Block.Interface.BlockConfig;
using Game.Block.Interface.Event;
using Game.Block.Interface.RecipeConfig;
using Game.Block.RecipeConfig;
using Game.Crafting.Config;
using Game.Crafting.Interface;
using Game.Entity;
using Game.Entity.Interface;
using Game.Map;
using Game.Map.Interface;
using Game.Map.Interface.Json;
using Game.Map.Interface.Vein;
using Game.PlayerInventory;
using Game.PlayerInventory.Event;
using Game.PlayerInventory.Interface;
using Game.PlayerInventory.Interface.Event;
using Game.SaveLoad.Interface;
using Game.SaveLoad.Json;
using Game.World;
using Game.World.DataStore;
using Game.World.DataStore.WorldSettings;
using Game.World.Event;
using Game.World.EventHandler.EnergyEvent;
using Game.World.EventHandler.EnergyEvent.EnergyService;
using Game.World.Interface;
using Game.World.Interface.DataStore;
using Game.World.Interface.Event;
using Game.WorldMap.EventListener;
using Microsoft.Extensions.DependencyInjection;
using Mod.Config;
using Newtonsoft.Json;
using Server.Event;
using Server.Event.EventReceive;
using Server.Protocol;

namespace Server.Boot
{
    public class MoorestechServerDiContainerGenerator
    {
        //TODO セーブファイルのディレクトリもここで指定できるようにする
        public (PacketResponseCreator, ServiceProvider) Create(string serverDirectory)
        {
            //必要な各種インスタンスを手動で作成
            var initializerCollection = new ServiceCollection();
            var modDirectory = Path.Combine(serverDirectory, "mods");
            var mapPath = Path.Combine(serverDirectory, "map", "map.json");

            var configJsons = ModJsonStringLoader.GetConfigString(modDirectory);
            initializerCollection.AddSingleton(new ConfigJsonFileContainer(configJsons));
            initializerCollection.AddSingleton<IItemConfig, ItemConfig>();
            initializerCollection.AddSingleton<IBlockConfig, BlockConfig>();
            initializerCollection.AddSingleton<IMachineRecipeConfig, MachineRecipeConfig>();
            initializerCollection.AddSingleton<ICraftingConfig, CraftConfig>();
            initializerCollection.AddSingleton<ItemStackFactory>();
            
            initializerCollection.AddSingleton<VanillaIBlockTemplates, VanillaIBlockTemplates>();
            initializerCollection.AddSingleton<IBlockFactory, BlockFactory>();
            initializerCollection.AddSingleton<ComponentFactory, ComponentFactory>();
            
            initializerCollection.AddSingleton<IWorldBlockUpdateEvent, WorldBlockUpdateEvent>();
            initializerCollection.AddSingleton<IWorldBlockDatastore, WorldBlockDatastore>();
            initializerCollection.AddSingleton<IBlockOpenableInventoryUpdateEvent, BlockOpenableInventoryUpdateEvent>();
            
            initializerCollection.AddSingleton<IBlockPlaceEvent, BlockPlaceEvent>();
            initializerCollection.AddSingleton<IBlockRemoveEvent, BlockRemoveEvent>();
            
            var initializerProvider = initializerCollection.BuildServiceProvider();


            //コンフィグ、ファクトリーのインスタンスを登録
            var services = new ServiceCollection();
            
            //TODO のちのち削除する
            services.AddSingleton(initializerProvider.GetService<IMachineRecipeConfig>());
            services.AddSingleton(initializerProvider.GetService<IItemConfig>());
            services.AddSingleton(initializerProvider.GetService<IBlockConfig>());
            services.AddSingleton(initializerProvider.GetService<ICraftingConfig>());
            services.AddSingleton(initializerProvider.GetService<ItemStackFactory>());
            services.AddSingleton(initializerProvider.GetService<IWorldBlockDatastore>());
            services.AddSingleton(initializerProvider.GetService<IWorldBlockUpdateEvent>());
            services.AddSingleton(initializerProvider.GetService<IBlockPlaceEvent>());
            services.AddSingleton(initializerProvider.GetService<IBlockRemoveEvent>());
            services.AddSingleton(initializerProvider.GetService<IBlockFactory>());
            services.AddSingleton(initializerProvider.GetService<ComponentFactory>());
            services.AddSingleton(initializerProvider.GetService<IBlockOpenableInventoryUpdateEvent>());
            
            //ゲームプレイに必要なクラスのインスタンスを生成
            services.AddSingleton<EventProtocolProvider, EventProtocolProvider>();
            services.AddSingleton<IWorldSettingsDatastore, WorldSettingsDatastore>();
            services.AddSingleton<IPlayerInventoryDataStore, PlayerInventoryDataStore>();
            services.AddSingleton<IBlockInventoryOpenStateDataStore, BlockInventoryOpenStateDataStore>();
            services.AddSingleton<IWorldEnergySegmentDatastore<EnergySegment>, WorldEnergySegmentDatastore<EnergySegment>>();
            services.AddSingleton<MaxElectricPoleMachineConnectionRange, MaxElectricPoleMachineConnectionRange>();
            services.AddSingleton<IEntitiesDatastore, EntitiesDatastore>();
            services.AddSingleton<IEntityFactory, EntityFactory>();

            services.AddSingleton<IMapObjectDatastore, MapObjectDatastore>();
            services.AddSingleton<IMapObjectFactory, MapObjectFactory>();
            services.AddSingleton<IMapVeinDatastore, MapVeinDatastore>();


            //JSONファイルのセーブシステムの読み込み
            services.AddSingleton<IWorldSaveDataSaver, WorldSaverForJson>();
            services.AddSingleton<IWorldSaveDataLoader, WorldLoaderFromJson>();
            services.AddSingleton(new SaveJsonFileName("save_1.json"));
            services.AddSingleton(JsonConvert.DeserializeObject<MapInfoJson>(File.ReadAllText(mapPath)));

            //イベントを登録
            services.AddSingleton<IMainInventoryUpdateEvent, MainInventoryUpdateEvent>();
            services.AddSingleton<IGrabInventoryUpdateEvent, GrabInventoryUpdateEvent>();

            //イベントレシーバーを登録
            services.AddSingleton<ChangeBlockStateEventPacket>();
            services.AddSingleton<MainInventoryUpdateEventPacket>();
            services.AddSingleton<OpenableBlockInventoryUpdateEventPacket>();
            services.AddSingleton<GrabInventoryUpdateEventPacket>();
            services.AddSingleton<PlaceBlockEventPacket>();
            services.AddSingleton<RemoveBlockToSetEventPacket>();

            services.AddSingleton<EnergyConnectUpdaterContainer<EnergySegment, IBlockElectricConsumer, IElectricGenerator, IElectricPole>>();

            services.AddSingleton<SetMiningItemToMiner>();
            services.AddSingleton<MapObjectUpdateEventPacket>();

            //データのセーブシステム
            services.AddSingleton<AssembleSaveJsonText, AssembleSaveJsonText>();


            var serviceProvider = services.BuildServiceProvider();
            var packetResponse = new PacketResponseCreator(serviceProvider);

            //イベントレシーバーをインスタンス化する
            //TODO この辺を解決するDIコンテナを探す VContinerのRegisterEntryPoint的な
            serviceProvider.GetService<MainInventoryUpdateEventPacket>();
            serviceProvider.GetService<OpenableBlockInventoryUpdateEventPacket>();
            serviceProvider.GetService<GrabInventoryUpdateEventPacket>();
            serviceProvider.GetService<PlaceBlockEventPacket>();
            serviceProvider.GetService<RemoveBlockToSetEventPacket>();

            serviceProvider.GetService<EnergyConnectUpdaterContainer<EnergySegment, IBlockElectricConsumer, IElectricGenerator, IElectricPole>>();

            serviceProvider.GetService<SetMiningItemToMiner>();
            serviceProvider.GetService<ChangeBlockStateEventPacket>();
            serviceProvider.GetService<MapObjectUpdateEventPacket>();

            return (packetResponse, serviceProvider);
        }
    }
}
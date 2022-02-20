﻿using System;
using Core.Block.BlockFactory;
using Core.Block.Blocks.Machine;
using Core.Block.RecipeConfig;
using Core.Item;
using Core.Item.Config;
using Game.World.Interface.DataStore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server;
using Test.Module.TestConfig;
using EntityId = Game.World.Interface.Util.EntityId;

namespace Test.UnitTest.Game
{
    public class WorldBlockDatastoreTest
    {
        [Test]
        public void RegisteredDataCoordinateFromFetchTest()
        {
            var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var worldData = serviceProvider.GetService<IWorldBlockDatastore>();

            var random = new Random(131513);
            for (int i = 0; i < 10; i++)
            {
                var entityId = EntityId.NewEntityId();
                var ins = CreateMachine(1, entityId);

                int x = random.Next(-1000, 1000);
                int y = random.Next(-1000, 1000);

                worldData.AddBlock(ins, x, y, BlockDirection.North);
                var output = worldData.GetBlock(x, y);
                Assert.AreEqual(entityId, output.GetEntityId());
            }
        }


        [Test]
        public void AlreadyRegisteredEntityIdSecondTimeFailTest()
        {
            var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var worldData = serviceProvider.GetService<IWorldBlockDatastore>();

            var entityId = EntityId.NewEntityId();
            var i = CreateMachine(1, entityId);
            worldData.AddBlock(i, 1, 1, BlockDirection.North);

            //座標だけ変えてintIDは同じ
            var i2 = CreateMachine(1, entityId);
            Assert.False(worldData.AddBlock(i2, 10, 10, BlockDirection.North));
        }

        [Test]
        public void AlreadyCoordinateSecondTimeFailTest()
        {
            var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create();
            var worldData = serviceProvider.GetService<IWorldBlockDatastore>();

            var i = CreateMachine(1, EntityId.NewEntityId());
            worldData.AddBlock(i, 1, 1, BlockDirection.North);

            //座標だけ変えてintIDは同じ
            var i2 = CreateMachine(1, EntityId.NewEntityId());
            Assert.False(worldData.AddBlock(i2, 1, 1, BlockDirection.North));
        }

        private BlockFactory _blockFactory;

        private VanillaMachine CreateMachine(int id, int indId)
        {
            if (_blockFactory == null)
            {
                var itemStackFactory = new ItemStackFactory(new TestItemConfig());
                _blockFactory = new BlockFactory(new AllMachineBlockConfig(),
                    new VanillaIBlockTemplates(new TestMachineRecipeConfig(itemStackFactory), itemStackFactory));
            }

            var machine = _blockFactory.Create(id, indId) as VanillaMachine;
            return machine;
        }
    }
}
#if NET6_0
using Game.World.Interface.DataStore;
using Game.WorldMap;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Server.Boot;
using Test.Module.TestMod;

namespace Test.UnitTest.Game.WorldSetting
{
    /// <summary>
    ///     ワールドのスポーン地点が正しい位置にあるかどうかをテストする
    /// </summary>
    public class WorldSpawnPointTest
    {
        /// <summary>
        ///     スポーンポイントに鉄があればOKなので、それをテストする
        /// </summary>
        [Test]
        public void WorldSpawnPointSearcherTest()
        {
            var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create(TestModDirectory.ForUnitTestModDirectory);
            var worldSettings = serviceProvider.GetService<IWorldSettingsDatastore>();
            var vineGenerator = serviceProvider.GetService<VeinGenerator>();
            worldSettings.Initialize();

            var spawnPoint = worldSettings.WorldSpawnPoint;

            //その座標の鉱石のIDを取得し、それが正しいかどうかをチェックする
            var spawnPointOreId = vineGenerator.GetOreId(spawnPoint.X, spawnPoint.Y);

            Assert.AreEqual(vineGenerator.GetOreId(spawnPoint.X, spawnPoint.Y), spawnPointOreId);
            Assert.AreEqual(1, spawnPointOreId);
        }
    }
}
#endif
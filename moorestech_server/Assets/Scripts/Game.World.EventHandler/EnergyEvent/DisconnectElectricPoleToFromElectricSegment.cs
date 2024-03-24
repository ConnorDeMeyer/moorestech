using System.Collections.Generic;
using Core.EnergySystem;
using Game.Block.Config.LoadConfig.Param;
using Game.Block.Interface.BlockConfig;
using Game.World.EventHandler.EnergyEvent.EnergyService;
using Game.World.Interface.DataStore;
using Game.World.Interface.Event;

namespace Game.World.EventHandler.EnergyEvent
{
    /// <summary>
    ///     電柱などのエネルギー伝達ブロックが破壊されたときに、セグメントを新しく作成するか、既存のセグメントから切り離すかを判断し、実行するクラス
    /// </summary>
    public class DisconnectElectricPoleToFromElectricSegment<TSegment, TConsumer, TGenerator, TTransformer>
        where TSegment : EnergySegment, new()
        where TConsumer : IEnergyConsumer
        where TGenerator : IEnergyGenerator
        where TTransformer : IEnergyTransformer
    {
        private readonly IBlockConfig _blockConfig;
        private readonly EnergyServiceDependencyContainer<TSegment> _dependencyContainer;
        private readonly IWorldBlockDatastore _worldBlockDatastore;
        private readonly IWorldEnergySegmentDatastore<TSegment> _worldEnergySegmentDatastore;


        public DisconnectElectricPoleToFromElectricSegment(
            IBlockRemoveEvent blockRemoveEvent,
            IBlockConfig blockConfig,
            IWorldEnergySegmentDatastore<TSegment> worldEnergySegmentDatastore,
            IWorldBlockDatastore worldBlockDatastore)
        {
            _blockConfig = blockConfig;
            _worldEnergySegmentDatastore = worldEnergySegmentDatastore;
            _worldBlockDatastore = worldBlockDatastore;
            blockRemoveEvent.Subscribe(OnBlockRemove);

            _dependencyContainer =
                new EnergyServiceDependencyContainer<TSegment>(worldEnergySegmentDatastore, worldBlockDatastore,
                    blockConfig);
        }

        private void OnBlockRemove(BlockRemoveEventProperties blockRemoveEvent)
        {
            //電柱かどうか判定
            //電柱だったら接続範囲内周りにある電柱を取得する
            if (!_worldBlockDatastore.TryGetBlock<TTransformer>(blockRemoveEvent.Pos, out var removedElectricPole)) return;


            //接続範囲内の電柱を取得
            List<IEnergyTransformer> electricPoles = FindElectricPoleFromPeripheralService.Find(
                blockRemoveEvent.Pos, _blockConfig.GetBlockConfig(blockRemoveEvent.Block.BlockId).Param as ElectricPoleConfigParam,
                _worldBlockDatastore);

            //削除した電柱のセグメントを取得
            var removedSegment = _worldEnergySegmentDatastore.GetEnergySegment(removedElectricPole);


            switch (electricPoles.Count)
            {
                //周りに電柱がないとき
                case 0:
                    //セグメントを削除する
                    _worldEnergySegmentDatastore.RemoveEnergySegment(removedSegment);
                    return;
                //周りの電柱が1つの時
                case 1:
                    DisconnectOneElectricPoleFromSegmentService<TSegment, TConsumer, TGenerator, TTransformer>
                        .Disconnect(removedElectricPole, _dependencyContainer);
                    return;
                //周りの電柱が2つ以上の時
                case >= 2:
                    DisconnectTwoOrMoreElectricPoleFromSegmentService<TSegment, TConsumer, TGenerator, TTransformer>
                        .Disconnect(removedElectricPole, _dependencyContainer);
                    break;
            }
        }
    }
}
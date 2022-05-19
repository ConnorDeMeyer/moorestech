using System;
using System.Collections.Generic;
using System.IO;
using Core.Block.Config;
using Core.Block.Config.LoadConfig;
using Core.Block.Config.LoadConfig.Param;

namespace Test.Module.TestConfig
{
    public class AllMachineBlockConfig : IBlockConfig
    {
        private readonly List<BlockConfigData>  _blockConfigList;
        private readonly Dictionary<ulong, BlockConfigData> _bockHashToConfig = new();

        public AllMachineBlockConfig()
        {
            _blockConfigList = new BlockConfigJsonLoad().LoadFromJsons(TestModuleConfig.AllMachineBlockConfigJson,TestModuleConfig.Mods);
            foreach (var blockConfig in _blockConfigList)
            {
                _bockHashToConfig.Add(blockConfig.BlockHash, blockConfig);
            }
        }

        public BlockConfigData GetBlockConfig(int id)
        {
            id -= 1;
            if (id < 0)
            {
                throw new ArgumentException("id must be greater than 0 ID:" + id);
            }
            if (id < _blockConfigList.Count)
            {
                return _blockConfigList[id];
            }

            //IDがなかったからインプット、アウトプットスロットが100のブロックを返す
            return new BlockConfigData(id,
                "Generated Block from AllMachineBlockConfig.cs",
                1,
                "Machine",
                new MachineBlockConfigParam(100, 100, 100),10);
        }

        public BlockConfigData GetBlockConfig(ulong blockHash)
        {
            if (_bockHashToConfig.TryGetValue(blockHash, out var blockConfig))
            {
                return blockConfig;
            }

            throw new Exception("BlockHash not found:" + blockHash);
        }

        public int GetBlockConfigCount()
        {
            return _blockConfigList.Count;
        }
    }
}
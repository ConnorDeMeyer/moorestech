﻿using Game.Block.Interface;
using Game.Block.Interface.BlockConfig;
using Game.World.Interface.DataStore;

namespace World.DataStore
{
    public class WorldBlockData
    {
        public WorldBlockData(IBlock block, int originX, int originY, BlockDirection blockDirection,IBlockConfig blockConfig)
        {
            OriginX = originX;
            OriginY = originY;
            BlockDirection = blockDirection;
            Block = block;
            var config = blockConfig.GetBlockConfig(block.BlockId);
            Height = config.BlockSize.Y;
            Weight = config.BlockSize.X;
        }

        public int OriginX { get; }
        public int OriginY { get; }
        public int Height { get; }
        public int Weight { get; }
        
        
        public int MaxX => BlockDirection is BlockDirection.North or BlockDirection.South ? OriginX + Weight : OriginX + Height;
        public int MaxY => BlockDirection is BlockDirection.North or BlockDirection.South ? OriginY + Height : OriginY + Weight;
        
        public IBlock Block { get; }
        public BlockDirection BlockDirection { get; }
    }
}
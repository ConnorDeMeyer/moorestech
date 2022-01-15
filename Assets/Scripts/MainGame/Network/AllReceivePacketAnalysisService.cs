﻿using System.Collections.Generic;
using System.Linq;
using MainGame.GameLogic.Interface;
using MainGame.Network.Receive;
using MainGame.Network.Util;

namespace MainGame.Network
{
    public class AllReceivePacketAnalysisService
    {
        private readonly List<IAnalysisPacket> _analysisPacketList = new List<IAnalysisPacket>();

        public AllReceivePacketAnalysisService(IChunkDataStore chunkDataStore)
        {
            _analysisPacketList.Add(new DummyProtocol());
            _analysisPacketList.Add(new ReceiveChunkDataProtocol(chunkDataStore));
        }

        public void Analysis(byte[] bytes)
        {
            var bytesList = bytes.ToList();
            
            //analysis packet
            _analysisPacketList[new ByteArrayEnumerator(bytesList).MoveNextToGetShort()].Analysis(bytesList);
        }
    }
}
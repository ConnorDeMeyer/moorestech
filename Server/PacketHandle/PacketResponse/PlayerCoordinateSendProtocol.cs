﻿using System.Collections.Generic;
using System.Linq;
using industrialization.Core.Block;
using industrialization.OverallManagement.DataStore;
using industrialization.OverallManagement.Util;
using industrialization.Server.PacketHandle.PacketResponse.Player;
using industrialization.Server.Util;

namespace industrialization.Server.PacketHandle.PacketResponse
{
    /// <summary>
    /// プレイヤー座標のプロトコル
    /// </summary>
    public class PlayerCoordinateSendProtocol
    {
        Dictionary<string,PlayerCoordinateToResponse> _responses = new Dictionary<string, PlayerCoordinateToResponse>();
        public List<byte[]> GetResponse(byte[] payload)
        {
            //プレイヤー座標の解析
            var b = new ByteArrayEnumerator(payload);
            b.MoveNextToGetShort();
            var x = b.MoveNextToGetFloat();
            var y = b.MoveNextToGetFloat();
            var name = b.MoveNextToGetString();
            //新しいプレイヤーの情報ならDictionaryに追加する
            if (!_responses.ContainsKey(name))
            {
                _responses.Add(name,new PlayerCoordinateToResponse());
            }
            
            //プレイヤーの座標から返すチャンクのブロックデータを取得をする
            //byte配列に変換して返す
            return _responses[name].
                GetResponseCoordinate(CoordinateCreator.New((int) x, (int) y)).
                Select(c => BlockToPayload.Convert(CoordinateToChunkBlocks.Convert(c),c)).
                ToList();
        }


        private static PlayerCoordinateSendProtocol _instance;
        public static PlayerCoordinateSendProtocol Instance
        {
            get
            {
                if (_instance is null) _instance = new PlayerCoordinateSendProtocol();
                return _instance;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace Server.PacketHandle.PacketResponse.Event
{
    /// <summary>
    /// 各プレイヤーにパケットとして送る必要があるイベントのキューを管理します
    /// </summary>
    public class EventProtocolQueProvider
    {
        private Dictionary<int, List<byte[]>> _events = new Dictionary<int, List<byte[]>>();

        public void AddEvent(int playerId,byte[] eventByteArray)
        {
        
            if (_events.ContainsKey(playerId))
            {
                _events[playerId].Add(eventByteArray);
            }
            else
            {
                _events.Add(playerId,new List<byte[]>(){eventByteArray});
            }
            
        }

        public List<byte[]> GetEventBytesList(int playerId)
        {
            if (_events.ContainsKey(playerId))
            {
                var data = _events[playerId];
                _events[playerId] = new List<byte[]>();
                return data;
            }

            return new List<byte[]>();
        }
        private static EventProtocolQueProvider _instance;
        public static EventProtocolQueProvider Instance
        {
            get
            {
                if (_instance is null) _instance = new EventProtocolQueProvider();
                return _instance;
            }
        }
    }
}
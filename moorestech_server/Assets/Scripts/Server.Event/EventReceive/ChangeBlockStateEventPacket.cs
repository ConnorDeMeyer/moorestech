﻿using System;
using System.Linq;
using Game.Block.Interface;
using Game.Block.Interface.State;
using Game.World.Interface.DataStore;
using MessagePack;
using Newtonsoft.Json;
using Server.Util.MessagePack;

namespace Server.Event.EventReceive
{
    public class ChangeBlockStateEventPacket
    {
        public const string EventTag = "va:event:changeBlockState";

        private readonly EventProtocolProvider _eventProtocolProvider;

        public ChangeBlockStateEventPacket(EventProtocolProvider eventProtocolProvider,
            IWorldBlockDatastore worldBlockDatastore)
        {
            _eventProtocolProvider = eventProtocolProvider;
            worldBlockDatastore.OnBlockStateChange += ChangeState;
        }

        private void ChangeState((ChangedBlockState state, IBlock block, int x, int y) state)
        {
            var payload = MessagePackSerializer.Serialize(
                new ChangeBlockStateEventMessagePack(state.state, state.x, state.y)).ToList();

            _eventProtocolProvider.AddBroadcastEvent(payload);
        }
    }

    [MessagePackObject(true)]
    public class ChangeBlockStateEventMessagePack : EventProtocolMessagePackBase
    {
        [Obsolete("デシリアライズ用のコンストラクタです。基本的に使用しないでください。")]
        public ChangeBlockStateEventMessagePack()
        {
        }

        public ChangeBlockStateEventMessagePack(ChangedBlockState state, int x, int y)
        {
            EventTag = ChangeBlockStateEventPacket.EventTag;
            CurrentState = state.CurrentState;
            PreviousState = state.PreviousState;

            CurrentStateJsonData = state.CurrentStateJsonData;
            Position = new Vector2IntMessagePack(x, y);
        }

        public string CurrentState { get; set; }
        public string PreviousState { get; set; }
        public string CurrentStateJsonData { get; set; }
        public Vector2IntMessagePack Position { get; set; }

        public TBlockState GetStateDat<TBlockState>()
        {
            return JsonConvert.DeserializeObject<TBlockState>(CurrentStateJsonData);
        }
    }
}
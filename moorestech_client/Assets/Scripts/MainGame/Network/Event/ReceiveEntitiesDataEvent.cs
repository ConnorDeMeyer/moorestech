using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Entity.Interface;
using MainGame.Basic;
using Server.Protocol.PacketResponse.MessagePack;
using Server.Util.MessagePack;
using UnityEngine;

namespace MainGame.Network.Event
{
    public class ReceiveEntitiesDataEvent
    {
        public event Action<List<EntityProperties>> OnEntitiesUpdate;

        internal async UniTask InvokeChunkUpdateEvent(EntitiesResponseMessagePack response)
        {
            await UniTask.SwitchToMainThread();

            var properties = new List<EntityProperties>();
            foreach (var entity in response.Entities)
            {
                properties.Add(new EntityProperties(entity));
            }
            
            OnEntitiesUpdate?.Invoke(properties);
        }
    }

}
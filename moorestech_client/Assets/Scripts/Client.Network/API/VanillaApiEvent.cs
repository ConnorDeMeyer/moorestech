using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MainGame.Network.Settings;
using Server.Protocol;
using Server.Protocol.PacketResponse;
using UnityEngine;

namespace Client.Network.API
{
    public class VanillaApiEvent
    {
        private readonly ServerConnector _serverConnector;
        private readonly PlayerConnectionSetting _playerConnectionSetting;
        
        private readonly Dictionary<string,Action<byte[]>> _eventResponseInfos = new ();
        public VanillaApiEvent(ServerConnector serverConnector, PlayerConnectionSetting playerConnectionSetting)
        {
            _serverConnector = serverConnector;
            _playerConnectionSetting = playerConnectionSetting;
            CollectEvent().Forget();
        }
        
        private async UniTask CollectEvent()
        {
            await VanillaApi.WaiteConnection();
                
            while (true)
            {
                var ct = new CancellationTokenSource().Token;

                try
                {
                    await RequestAndParse(ct);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Event Protocol Error:{e.Message}\n{e.StackTrace}");
                }

                await UniTask.Delay(ServerConst.PollingRateMillSec, cancellationToken: ct);
            }

            #region Internal

            async UniTask RequestAndParse(CancellationToken ct)
            {
                var request = new EventProtocolMessagePack(_playerConnectionSetting.PlayerId);
            
                var response = await _serverConnector.GetInformationData<ResponseEventProtocolMessagePack>(request, ct);
                //TODO 初期化をちゃんとするようにしてnullチェックをなくしたい
                if (response == null)
                {
                    return;
                }
            
                foreach (var eventMessagePack in response.Events)
                {
                    if (_eventResponseInfos.TryGetValue(eventMessagePack.Tag, out var action))
                    {
                        action(eventMessagePack.Payload);
                    }
                }
            }

            #endregion
        }
        
        public void RegisterEventResponse(string tag,Action<byte[]> responseAction)
        {
            _eventResponseInfos.Add(tag, responseAction);
        }
        
        public void UnRegisterEventResponse(string tag)
        {
            _eventResponseInfos.Remove(tag);
        }
    }
}
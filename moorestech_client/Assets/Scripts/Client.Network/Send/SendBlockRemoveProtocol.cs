﻿using System.Linq;
using MainGame.Network.Settings;
using MessagePack;
using Server.Protocol.PacketResponse;

namespace MainGame.Network.Send
{
    public class SendBlockRemoveProtocol
    {
        private readonly int _playerId;
        private readonly ISocketSender _socketSender;


        public SendBlockRemoveProtocol(PlayerConnectionSetting playerConnectionSetting, ISocketSender socketSender)
        {
            _socketSender = socketSender;
            _playerId = playerConnectionSetting.PlayerId;
        }

        public void Send(int x, int y)
        {
            _socketSender.Send(MessagePackSerializer.Serialize(new RemoveBlockProtocolMessagePack(
                _playerId, x, y)).ToList());
        }
    }
}
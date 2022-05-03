using System.Collections.Generic;
using MainGame.Network.Util;

namespace MainGame.Network.Send
{
    public class RequestBlockInventoryProtocol
    {
        private const short ProtocolId = 9;
        private readonly ISocket _socket;

        public RequestBlockInventoryProtocol(ISocket socket)
        {
            _socket = socket;
        }

        public void Send(int x, int y)
        {
            var packet = new List<byte>();
            
            packet.AddRange(ToByteList.Convert(ProtocolId));
            packet.AddRange(ToByteList.Convert(x));
            packet.AddRange(ToByteList.Convert(y));
            
            _socket.Send(packet);
        }
    }
}
using Lagrange.Core.Common;
using Lagrange.Core.Internal.Event.Protocol;
using Lagrange.Core.Internal.Event.Protocol.Message;
using Lagrange.Core.Internal.Packets.Action;
using Lagrange.Core.Message;
using Lagrange.Core.Utility.Binary;
using ProtoBuf;

namespace Lagrange.Core.Internal.Service.Message;

[EventSubscribe(typeof(SendMessageEvent))]
[Service("MessageSvc.PbSendMsg")]
internal class SendMessageService : BaseService<SendMessageEvent>
{
    protected override bool Build(SendMessageEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        var packer = new MessagePacker(keystore.Uid ?? throw new Exception("No UID found in keystore"));
        var packet = packer.Build(input.Chain);

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, packet);
        
        output = new BinaryPacket(stream);
        extraPackets = null;

        return true;
    }

    protected override bool Parse(byte[] input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out SendMessageEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var response = Serializer.Deserialize<SendMessageResponse>(input.AsSpan());
        var result = new MessageResult
        {
            Result = (uint)response.Result,
            Sequence = response.Sequence,
            Timestamp = response.Timestamp1,
        };
        
        output = SendMessageEvent.Result(response.Result, result);
        extraEvents = null;
        return true;
    }
}
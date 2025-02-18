using Lagrange.Core.Common;
using Lagrange.Core.Internal.Event.Protocol;
using Lagrange.Core.Internal.Event.Protocol.Action;
using Lagrange.Core.Internal.Packets.Service.Oidb;
using Lagrange.Core.Internal.Packets.Service.Oidb.Request;
using Lagrange.Core.Utility.Binary;
using ProtoBuf;

namespace Lagrange.Core.Internal.Service.Action;

[EventSubscribe(typeof(GroupRemarkEvent))]
[Service("OidbSvcTrpcTcp.0xf16_1")]
internal class GroupRemarkService : BaseService<GroupRemarkEvent>
{
    protected override bool Build(GroupRemarkEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        var packet = new OidbSvcTrpcTcpBase<OidbSvcTrpcTcp0xF16_1>(new OidbSvcTrpcTcp0xF16_1
        {
            Body = new OidbSvcTrpcTcp0xF16_1Body
            {
                GroupUin = input.GroupUin,
                TargetRemark = input.TargetRemark
            }
        });

        var stream = new MemoryStream();
        Serializer.Serialize(stream, packet);
        output = new BinaryPacket(stream);

        extraPackets = null;
        return true;
    }

    protected override bool Parse(byte[] input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out GroupRemarkEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var payload = Serializer.Deserialize<OidbSvcTrpcTcpResponse<byte[]>>(input.AsSpan());
        
        output = GroupRemarkEvent.Result((int)payload.ErrorCode);
        extraEvents = null;
        return true;
    }
}
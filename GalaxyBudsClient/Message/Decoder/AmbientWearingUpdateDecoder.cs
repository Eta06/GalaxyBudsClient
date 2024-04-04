﻿using System;
using GalaxyBudsClient.Model.Constants;

namespace GalaxyBudsClient.Message.Decoder;

/*
 * Mostly unused if (versionOfMR < 2). Refer to ExtendedStatusUpdateDecoder
 */
internal class AmbientWearingUpdateDecoder : BaseMessageDecoder
{
    public override MsgIds HandledType => MsgIds.AMBIENT_WEARING_STATUS_UPDATED;

    public LegacyWearStates WearState { set; get; }
    public int LeftDetectionCount { set; get; }
    public int RightDetectionCount { set; get; }

    public override void Decode(SppMessage msg)
    {
        if (msg.Id != HandledType)
            return;

        WearState = (LegacyWearStates) msg.Payload[0];
        LeftDetectionCount = BitConverter.ToInt16(msg.Payload, 1);
        RightDetectionCount = BitConverter.ToInt16(msg.Payload, 3);
    }
}
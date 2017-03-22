using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Prototype_Assets
{
    public enum PacketType
    {
        MOVE,
        WEBCAM,
        QRCODE,
        ENDMOVE,
        PlayerTryRegister,
        PlayerTryRegisterResult,
        PlayerRegister,
        PlayerUnRegister,
        UpdateAllEscapeStatesOnClients,
        UpdateSingleEscapeStateOnClients,
        UpdateSingleEscapeStateOnServer,
        CheckEscapeState,
        CheckClientAlive,
        SHAKE,
    };
}

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
        ESCAPESTARTED,
        UpdateEscapeState,
        CheckEscapeState,
        CheckClientAlive,
        SHAKE,
    };
}

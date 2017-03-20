
using UnityEngine;

namespace Assets.Prototype_Assets
{
    public static class GlobalVariables
    {
        public static string ipAddress = PlayerPrefs.GetString("IPAddress", "127.0.0.1");
        public static int playerNumber = 0;
        public static bool mobilePlayerRegistered = false;
        public static EscapeState escapeState = EscapeState.WaitingToStart;

        public enum Direction { Forward, Backward, Left, Right, Sonar };

        public enum EscapeState { WaitingToStart, SubDescending, }
    }
}


using System.Collections.Generic;
using UnityEngine;

namespace Assets.Prototype_Assets
{
    public static class GlobalVariables
    {
        public static string ipAddress = PlayerPrefs.GetString("IPAddress", "127.0.0.1");
        public static int playerNumber = 0;
        public static bool mobilePlayerRegistered = false;
        public static bool IPRadar = false;

        public static Dictionary<EscapeState, bool> progression = new Dictionary<EscapeState, bool>();

        public enum Direction { Forward, Backward, Left, Right, Sonar };
        public enum EscapeState { EscapeStarted, SubDescended, KeypadCodeEntered, FuzesScattered, SubControlsEnabled ,}

        public static bool CheckProgression(EscapeState escapeState)
        {
            if (progression.ContainsKey(escapeState))
            {
                return progression[escapeState];
            }
            else
            {
                return false;
            }
        }

        // DO NOT CALL THIS IF YOU'RE ON THE SERVER.
        // Instead call EscapeRoomController.Instance.UpdateSingleEscapeStateOnClients().
        public static void UpdateProgression(EscapeState escapeState, bool progressed)
        {
            progression[escapeState] = progressed;
        }
    }
}

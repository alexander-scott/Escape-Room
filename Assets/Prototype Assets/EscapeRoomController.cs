using NetworkLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Assets.Prototype_Assets
{
    public class EscapeRoomController : NetworkBehaviour
    {
        private GlobalVariables.EscapeState escapeState;

        private bool player1Registered = false;
        private bool player2Registered = false;
        private bool player3Registered = false;
        private bool player4Registered = false;

        private bool gameStarted = false;

        public override void OnStartServer()
        {
            // Starts a server to listen for movement commands if this client is the host
            if (isServer)
            {
                // Starts a UDP Server from the Network Lib
                NetworkLib.Server.start(LibProtocolType.UDP);

                Server.ServerPacketObserver.AddObserver((int)PacketType.PlayerTryRegister, PlayerTryRegister);
                Server.ServerPacketObserver.AddObserver((int)PacketType.PlayerRegister, PlayerRegister);
                Server.ServerPacketObserver.AddObserver((int)PacketType.PlayerUnRegister, PlayerUnRegister);

                // THIS IS A SHIT WAY TO DO THIS - GET A REFERENCE
                SubMove submove = FindObjectOfType<SubMove>();

                Server.ServerPacketObserver.AddObserver((int)PacketType.MOVE, submove.MoveSub);
                Server.ServerPacketObserver.AddObserver((int)PacketType.ENDMOVE, submove.EndMoveSub);
            }
        }

        void OnApplicationQuit()
        {
            NetworkLib.Server.stop();
        }

        private void PlayerUnRegister(Packet p)
        {
            switch ((GlobalVariables.Direction)p.generalData[0])
            {
                case GlobalVariables.Direction.Forward:
                    player1Registered = false;
                    break;

                case GlobalVariables.Direction.Backward:
                    player2Registered = false;
                    break;

                case GlobalVariables.Direction.Left:
                    player3Registered = false;
                    break;

                case GlobalVariables.Direction.Right:
                    player4Registered = false;
                    break;
            }
        }

        private void PlayerRegister(Packet p)
        {
            switch ((GlobalVariables.Direction)p.generalData[0])
            {
                case GlobalVariables.Direction.Forward:
                    player1Registered = true;
                    Debug.Log("P1 REGISTERED");
                    break;

                case GlobalVariables.Direction.Backward:
                    player2Registered = true;
                    Debug.Log("P2 REGISTERED");
                    break;

                case GlobalVariables.Direction.Left:
                    player3Registered = true;
                    Debug.Log("P3 REGISTERED");
                    break;

                case GlobalVariables.Direction.Right:
                    player4Registered = true;
                    Debug.Log("P4 REGISTERED");
                    break;
            }
        }

        private void PlayerTryRegister(Packet p)
        {
            switch ((GlobalVariables.Direction)p.generalData[0])
            {
                case GlobalVariables.Direction.Forward:
                    if (!player1Registered)
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Forward).ToString(), true);
                    }
                    else
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Forward).ToString(), false);
                    }

                    break;

                case GlobalVariables.Direction.Backward:
                    if (!player2Registered)
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Backward).ToString(), true);
                    }
                    else
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Backward).ToString(), false);
                    }

                    break;

                case GlobalVariables.Direction.Left:
                    if (!player3Registered)
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Left).ToString(), true);
                    }
                    else
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Left).ToString(), false);
                    }

                    break;

                case GlobalVariables.Direction.Right:
                    if (!player4Registered)
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Right).ToString(), true);
                    }
                    else
                    {
                        SendRegisterResponse((GlobalVariables.Direction.Right).ToString(), false);
                    }

                    break;
            }
        }

        private void SendRegisterResponse(string contents, bool response)
        {
            Packet pack = new Packet((int)PacketType.PlayerTryRegisterResult, contents);
            pack.generalData.Add(contents);
            pack.generalData.Add(response);

            for (int i = 0; i < Server.udpClients.Count; i++)
            {
                Server.udpClients[i].SendPacket(pack);
            }
        }
    }
}

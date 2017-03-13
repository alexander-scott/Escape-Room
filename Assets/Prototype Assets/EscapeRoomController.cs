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
                Server.ServerPacketObserver.AddObserver((int)PacketType.EscapeStartRequest, CheckEscapeStart);
                Server.ServerPacketObserver.AddObserver((int)PacketType.ESCAPESTARTED, EscapeStarted);
                Server.ServerPacketObserver.AddObserver((int)PacketType.CheckEscapeStart, CheckEscapeStart);

                // THIS IS A SHIT WAY TO DO THIS - GET A REFERENCE
                SubMove submove = FindObjectOfType<SubMove>();

                Server.ServerPacketObserver.AddObserver((int)PacketType.MOVE, submove.MoveSub);
                Server.ServerPacketObserver.AddObserver((int)PacketType.ENDMOVE, submove.EndMoveSub);

                StartCoroutine(CheckClientsAlive());
            }
        }

        // Ensure we stop the server when the application ends
        void OnApplicationQuit()
        {
            NetworkLib.Server.stop();
        }

        // This is called when a client asks the server if the game has started yet. It returns true or false to all clients. 
        // PacketType = EscapeStartRequest
        private void CheckEscapeStart(Packet p)
        {
            SendCheckGameStartResponse();
        }

        // This is called when the start button is pressed on the iPad. Sets escapeStarted to true. Tells all clients the escape has started.
        // PacketType = ESCAPESTARTED
        private void EscapeStarted(Packet p)
        {
            GlobalVariables.escapeStarted = true;
            Debug.Log("GAME STARTED");

            SendGameStart();
        }

        // This is called when a client presses disconnect on their mobile phone. Sets a specific bool to false.
        // PacketType = PlayerUnRegister
        private void PlayerUnRegister(Packet p)
        {
            switch ((GlobalVariables.Direction)p.generalData[0])
            {
                case GlobalVariables.Direction.Forward:
                    player1Registered = false;
                    Debug.Log("P1 UNREGISTERED");
                    break;

                case GlobalVariables.Direction.Backward:
                    player2Registered = false;
                    Debug.Log("P2 UNREGISTERED");
                    break;

                case GlobalVariables.Direction.Left:
                    player3Registered = false;
                    Debug.Log("P3 UNREGISTERED");
                    break;

                case GlobalVariables.Direction.Right:
                    player4Registered = false;
                    Debug.Log("P4 UNREGISTERED");
                    break;
            }
        }

        // This is called when a client presses connect on their mobile phone. Sets a specific bool to true. 
        // This is only called by a client after the server has told them their player of choice is free.
        // PacketType = PlayerRegister
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

        // This is called when a client presses connect on their mobile phone. Sends a message to that client telling them whether their
        // player number of choice has been taken yet or not.
        // PacketType = PlayerTryRegister
        private void PlayerTryRegister(Packet p)
        {
            switch ((GlobalVariables.Direction)p.generalData[0])
            {
                case GlobalVariables.Direction.Forward:
                    if (!player1Registered)
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Forward).ToString(), true);
                    }
                    else
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Forward).ToString(), false);
                    }

                    break;

                case GlobalVariables.Direction.Backward:
                    if (!player2Registered)
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Backward).ToString(), true);
                    }
                    else
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Backward).ToString(), false);
                    }

                    break;

                case GlobalVariables.Direction.Left:
                    if (!player3Registered)
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Left).ToString(), true);
                    }
                    else
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Left).ToString(), false);
                    }

                    break;

                case GlobalVariables.Direction.Right:
                    if (!player4Registered)
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Right).ToString(), true);
                    }
                    else
                    {
                        SendTryRegisterResponse((GlobalVariables.Direction.Right).ToString(), false);
                    }

                    break;
            }
        }

        // Sends a message to all clients (only the one who requested it will read it) telling them if their player of choice is free or not.
        private void SendTryRegisterResponse(string contents, bool response)
        {
            Packet pack = new Packet((int)PacketType.PlayerTryRegisterResult, contents);
            pack.generalData.Add(contents);
            pack.generalData.Add(response);

            for (int i = 0; i < Server.udpClients.Count; i++)
            {
                Server.udpClients[i].SendPacket(pack);
            }
        }

        // Sends a message to all clients telling them that the escape has started
        private void SendGameStart()
        {
            Packet pack = new Packet((int)PacketType.ESCAPESTARTED, PacketType.ESCAPESTARTED.ToString());

            for (int i = 0; i < Server.udpClients.Count; i++)
            {
                Server.udpClients[i].SendPacket(pack);
            }
        }

        // Sends a message to all clients telling them if the escape has started or hasn't started.
        private void SendCheckGameStartResponse()
        {
            Packet pack = new Packet((int)PacketType.CheckEscapeStartResponse, PacketType.CheckEscapeStartResponse.ToString());
            pack.generalData.Add(GlobalVariables.escapeStarted);

            for (int i = 0; i < Server.udpClients.Count; i++)
            {
                Server.udpClients[i].SendPacket(pack);
            }
        }

        private IEnumerator CheckClientsAlive()
        {
            Packet pack = new Packet((int)PacketType.CheckClientAlive, PacketType.CheckClientAlive.ToString());

            while (true) // PERMANENTLY ACTIVE
            {
                for (int i = 0; i < Server.udpClients.Count; i++)
                {
                    Server.udpClients[i].SendPacket(pack);
                }

                yield return new WaitForSeconds(1f);
            }
        }
    }
}

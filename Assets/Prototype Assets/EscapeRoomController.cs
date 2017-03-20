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
        #region Singleton

        private static EscapeRoomController _instance;

        public static EscapeRoomController Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        #endregion

        private bool player1Registered = false;
        private bool player2Registered = false;
        private bool player3Registered = false;
        private bool player4Registered = false;

        private PlayerController playerController;

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
                Server.ServerPacketObserver.AddObserver((int)PacketType.CheckEscapeState, CheckEscapeState);
                Server.ServerPacketObserver.AddObserver((int)PacketType.ESCAPESTARTED, EscapeStarted);

                // THIS IS A SHIT WAY TO DO THIS - GET A REFERENCE
                SubMove submove = FindObjectOfType<SubMove>();

                Server.ServerPacketObserver.AddObserver((int)PacketType.MOVE, submove.MoveSub);
                Server.ServerPacketObserver.AddObserver((int)PacketType.ENDMOVE, submove.EndMoveSub);
                Server.ServerPacketObserver.AddObserver((int)PacketType.SHAKE, EnableShake);

                StartCoroutine(CheckClientsAlive());
            }
        }

        private void EnableShake(Packet p)
        {
            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
            }

            playerController.EnableShake(p);
        }

        // Ensure we stop the server when the application ends
        private void OnApplicationQuit()
        {
            if (isServer)
            {
                NetworkLib.Server.stop();
            }
            
            if (isClient)
            {
                NetworkLib.Client.stop();
            }
        }

        #region Escape state sync

        // Sends a message to all clients telling them what the new escape state is. Call this if you need to update the game state. Leave clientCalled false.
        // PacketType = UpdateEscapeState
        public void UpdateEscapeState(GlobalVariables.EscapeState escapeState, bool clientCalled = false)
        {
            Packet pack = new Packet((int)PacketType.UpdateEscapeState, PacketType.UpdateEscapeState.ToString());
            pack.generalData.Add(escapeState);
            pack.generalData.Add(clientCalled);

            for (int i = 0; i < Server.udpClients.Count; i++)
            {
                Server.udpClients[i].SendPacket(pack);
            }
        }

        // This is called when a client asks the server what the current escape state is. It returns the current escape state to all clients.
        // PacketType = EscapeStartRequest
        private void CheckEscapeState(Packet p)
        {
            UpdateEscapeState(GlobalVariables.escapeState, true);
        }

        // This is called when the start button is pressed on the iPad
        // PacketType = ESCAPESTARTED
        private void EscapeStarted(Packet p)
        {
            GlobalVariables.escapeState = GlobalVariables.EscapeState.SubDescending;

            UpdateEscapeState(GlobalVariables.escapeState);
        }

        #endregion

        #region Player registration

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

        #endregion

        // This coroutine constantly sends tiny packets to clients telling them that the server is still alive
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

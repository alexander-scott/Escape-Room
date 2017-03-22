using NetworkLib;
using System.Collections;
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

        public bool raspPiOnline = false;

        private bool player1Registered = false;
        private bool player2Registered = false;
        private bool player3Registered = false;
        private bool player4Registered = false;
        private RPI.Metalberry rpi;

        private PlayerController playerController;

        public override void OnStartServer()
        {
            rpi = new RPI.Metalberry();

            // Starts a server to listen for movement commands if this client is the host
            if (isServer)
            {
                // Starts a UDP Server from the Network Lib
                NetworkLib.Server.start(LibProtocolType.UDP);

                Server.ServerPacketObserver.AddObserver((int)PacketType.PlayerTryRegister, PlayerTryRegister);
                Server.ServerPacketObserver.AddObserver((int)PacketType.PlayerRegister, PlayerRegister);
                Server.ServerPacketObserver.AddObserver((int)PacketType.PlayerUnRegister, PlayerUnRegister);
                Server.ServerPacketObserver.AddObserver((int)PacketType.CheckEscapeState, CheckEscapeState);
                Server.ServerPacketObserver.AddObserver((int)PacketType.UpdateSingleEscapeStateOnServer, UpdateSingleEscapeStateOnServer);

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
            else
            {
                NetworkLib.Client.stop();
            }
        }

        #region Escape state sync

        // Allows you to update a single escape state on the server and clients
        // CALLED BY ANYWHERE ON THE SERVER.
        public void UpdateSingleEscapeStateOnClients(GlobalVariables.EscapeState escapeState, bool progression)
        {
            if (!GlobalVariables.CheckProgression(escapeState))
            {
                TriggerEscapeStateFirstTimeEvent(escapeState);
            }

            GlobalVariables.UpdateProgression(escapeState, progression);

            Packet pack = new Packet((int)PacketType.UpdateSingleEscapeStateOnClients, PacketType.UpdateSingleEscapeStateOnClients.ToString());
            pack.generalData.Add(escapeState);
            pack.generalData.Add(progression);

            for (int i = 0; i < Server.udpClients.Count; i++)
            {
                Server.udpClients[i].SendPacket(pack);
            }
        }

        // Allows clients to update escape states on the server and all other clients.
        // PacketType = UpdateSingleEscapeStateOnServer. CALLED BY ANY CLIENT.
        private void UpdateSingleEscapeStateOnServer(Packet p)
        {
            GlobalVariables.EscapeState escapeState = (GlobalVariables.EscapeState)Enum.Parse(typeof(GlobalVariables.EscapeState), p.generalData[0].ToString());
            bool progression = bool.Parse(p.generalData[1].ToString());

            if (!GlobalVariables.CheckProgression(escapeState))
            {
                TriggerEscapeStateFirstTimeEvent(escapeState);
            }

            GlobalVariables.UpdateProgression(escapeState, progression);

            UpdateEscapeStateOnClients();
        }

        // This is called when a client asks the server what the current escape state is. It returns the current escape state to all clients.
        // PacketType = EscapeStartRequest. CALLED BY ANY CLIENT.
        private void CheckEscapeState(Packet p)
        {
            UpdateEscapeStateOnClients();
        }

        // Sends a message to all clients telling them what the new escape state is. 
        private void UpdateEscapeStateOnClients()
        {
            Packet pack = new Packet((int)PacketType.UpdateAllEscapeStatesOnClients, PacketType.UpdateAllEscapeStatesOnClients.ToString());

            for (int i = 0; i < Enum.GetNames(typeof(GlobalVariables.EscapeState)).Length; i++)
            {
                pack.generalData.Add(GlobalVariables.CheckProgression((GlobalVariables.EscapeState)i));
            }

            for (int i = 0; i < Server.udpClients.Count; i++)
            {
                Server.udpClients[i].SendPacket(pack);
            }
        }

        // This function contains logic for what happens after a escape state gets triggered for the first time
        private void TriggerEscapeStateFirstTimeEvent(GlobalVariables.EscapeState escapeState)
        {
            Debug.Log(escapeState.ToString() + " triggered");

            switch (escapeState)
            {
                case GlobalVariables.EscapeState.EscapeStarted:
                    if (raspPiOnline)
                        rpi.Do(CMD.PLAY_COMPUTER_GREETING);
                    break;

                case GlobalVariables.EscapeState.SubControlsEnabled:
                    if (raspPiOnline)
                        rpi.Do(CMD.PLAY_S_CONTROL_BUTTON);
                    break;

                case GlobalVariables.EscapeState.FuzesScattered:
                    if (raspPiOnline)
                        rpi.Do(CMD.PLAY_FUSES_DISLODGED);
                    break;

                case GlobalVariables.EscapeState.SubDescended:
                    if (raspPiOnline)
                        rpi.Do(CMD.PLAY_OXYGEN_LVL_DECREASE);
                    break;

                case GlobalVariables.EscapeState.KeypadCodeEntered:
                    if (raspPiOnline)
                        rpi.Do(CMD.PLAY_DIAGNOSTICS_ONLINE);
                    break;
            }
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

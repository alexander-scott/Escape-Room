using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NetworkLib;
using UnityEngine.Networking;
using System;

namespace Assets.Prototype_Assets
{
    public class MenuButtons : MonoBehaviour
    {
        public Button subButton;
        public Button controlsButton;
        public Button qrScannerButton;
        public Button sonarButton;

        public Button connectButton;

        public InputField ipAddressText;
        public Dropdown dropDownList;

        public Text infoText;

        private bool awaitingResponse = false;
        private bool clientCreated = false;
        private bool updateUI = false;

        private Text connectButtonText;

        private void Start()
        {
            subButton.onClick.AddListener(SubButtonClicked);
            controlsButton.onClick.AddListener(ControlsButtonClicked);
            qrScannerButton.onClick.AddListener(QRButtonClicked);
            sonarButton.onClick.AddListener(SonarButtonClicked);

            connectButton.onClick.AddListener(ConnectButtonClicked);

            ipAddressText.text = GlobalVariables.ipAddress;
            dropDownList.value = GlobalVariables.playerNumber;

            connectButtonText = connectButton.GetComponentInChildren<Text>();

            // If we're returing to this scene from the sub controls then the play might still be registered
            if (GlobalVariables.mobilePlayerRegistered)
            {
                connectButtonText.text = "Disconnect";
                controlsButton.gameObject.GetComponent<Image>().color = Color.green;
                dropDownList.enabled = false;
            }
            else
            {
                connectButtonText.text = "Connect";
                controlsButton.gameObject.GetComponent<Image>().color = Color.gray;
                dropDownList.enabled = true;
            }
        }

        private void Update()
        {
            // Since the Server runs on a seperate thread we can't directly update this stuff. 
            // This is a hack to make modifcations to gameobjects/components such as text
            if (updateUI)
            {
                updateUI = false;

                if (GlobalVariables.mobilePlayerRegistered)
                {
                    connectButtonText.text = "Disconnect";
                    controlsButton.gameObject.GetComponent<Image>().color = Color.green;
                    dropDownList.enabled = false;
                    infoText.text = "Successfully connected!";
                }
                else
                {
                    connectButtonText.text = "Connect";
                    controlsButton.gameObject.GetComponent<Image>().color = Color.gray;
                    dropDownList.enabled = true;
                    infoText.text = "Someone else has taken this player!";
                }
            }
        }

        void OnApplicationQuit()
        {
            if (GlobalVariables.mobilePlayerRegistered)
            {
                NetworkLib.Client.stop();
            }
            else
            {
                DontDestroyOnLoad(this); // Don't end the client if we're registered. Keep it alive so the sub controls can send packets.
            }
        }

        private void ConnectButtonClicked()
        {
            if (!GlobalVariables.mobilePlayerRegistered)
            {
                if (!clientCreated)
                {
                    // This is the only place that the client gets created on the mobile app. It stays alive so other scenes can use it.
                    NetworkLib.Client.connect(GlobalVariables.ipAddress, LibProtocolType.UDP);
                    NetworkLib.Client.ClientPacketObserver.AddObserver((int)PacketType.PlayerTryRegisterResult, PlayerTryRegisterResult);
                    NetworkLib.Client.ClientPacketObserver.AddObserver((int)PacketType.ESCAPESTARTED, EscapeStarted);
                    NetworkLib.Client.ClientPacketObserver.AddObserver((int)PacketType.CheckEscapeStartResponse, CheckEscapeStartResponse);

                    clientCreated = true;
                }

                awaitingResponse = true;

                // Ask the server if we can take this player number
                Packet p = new Packet((int)PacketType.PlayerTryRegister, ((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString());
                p.generalData.Add(((GlobalVariables.Direction)GlobalVariables.playerNumber));
                Client.SendPacket(p);

                infoText.text = "Attempting to connect...";
            }
            else
            {
                // Tell the server we are giving up our player number
                Packet p = new Packet((int)PacketType.PlayerUnRegister, ((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString());
                p.generalData.Add(((GlobalVariables.Direction)GlobalVariables.playerNumber));
                Client.SendPacket(p);

                NetworkLib.Client.stop();

                clientCreated = false;
                awaitingResponse = false;

                GlobalVariables.mobilePlayerRegistered = false;
                infoText.text = "Disconnected";
                connectButtonText.text = "Connect";
                dropDownList.enabled = true;
                controlsButton.gameObject.GetComponent<Image>().color = Color.gray;
            }
        }

        private void PlayerTryRegisterResult(Packet p)
        {
            if (awaitingResponse)
            {
                awaitingResponse = false;
                updateUI = true;

                if (bool.Parse(p.generalData[1].ToString())) // This player number is free
                {
                    Packet pack = new Packet((int)PacketType.PlayerRegister, ((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString());
                    pack.generalData.Add(((GlobalVariables.Direction)GlobalVariables.playerNumber));
                    Client.SendPacket(pack);

                    GlobalVariables.mobilePlayerRegistered = true;

                    CheckEscapeStart();
                }
                else // It is already taken
                {
                    GlobalVariables.mobilePlayerRegistered = false;
                }
            }
        }

        // Asks the server if the escape has started yet
        private void CheckEscapeStart()
        {
            Packet p = new Packet((int)PacketType.CheckEscapeStart, PacketType.CheckEscapeStart.ToString());
            Client.SendPacket(p);
        }

        // This is the response from the server telling us if the game has started or not
        private void CheckEscapeStartResponse(Packet p)
        {
            if (bool.Parse(p.generalData[0].ToString())) // Game has started
            {
                GlobalVariables.escapeStarted = true;
            }
        }

        private void EscapeStarted(Packet p)
        {
            GlobalVariables.escapeStarted = true;
        }

        private void SubButtonClicked()
        {
            SceneManager.LoadScene("Test");
        }

        private void ControlsButtonClicked()
        {
            if (GlobalVariables.mobilePlayerRegistered && GlobalVariables.escapeStarted)
            {
                SceneManager.LoadScene("Controls");
            }
            else if (GlobalVariables.mobilePlayerRegistered && !GlobalVariables.escapeStarted)
            {
                infoText.text = "Please start the game first!";
            }
            else
            {
                infoText.text = "Please connect first!";
            }
        }

        private void QRButtonClicked()
        {
            SceneManager.LoadScene("QRScanner");
        }

        private void SonarButtonClicked()
        {
            SceneManager.LoadScene("Sonar");
        }

        public void IPAddressChanged(string ipaddress)
        {
            GlobalVariables.ipAddress = ipaddress;
        }

        public void PlayerSelectChanged(int val)
        {
            GlobalVariables.playerNumber = val;
        }
    }
}

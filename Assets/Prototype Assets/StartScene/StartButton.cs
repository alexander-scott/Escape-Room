using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkLib;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

namespace Assets.Prototype_Assets
{
    public class StartButton : MonoBehaviour
    {
        private Button startButton;

        // Use this for initialization
        void Start()
        {
            startButton = GetComponent<Button>();
            startButton.onClick.AddListener(StartPressed);
        }

        private void OnApplicationQuit()
        {
            NetworkLib.Client.stop();
        }

        private void StartPressed()
        {
            // TODO: IF CONNECT FAILS TRY AGAIN IN A MOMENT
            NetworkLib.Client.connect(GlobalVariables.ipAddress, LibProtocolType.UDP);

            Packet p = new Packet((int)PacketType.ESCAPESTARTED, PacketType.ESCAPESTARTED.ToString());
            Client.SendPacket(p);

            NetworkLib.Client.stop();

            SceneManager.LoadScene("Sonar");
        }
    }
}

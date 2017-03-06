using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Assets.Prototype_Assets
{
    public class SubMenuButtons : MonoBehaviour {

        public Button startSessionBtn;
        public Button joinSessionBtn;
        public Canvas canvas;
        //public Button subControls;

        private void Start()
        {
            startSessionBtn.onClick.AddListener(StartSessionBtnClicked);
            joinSessionBtn.onClick.AddListener(JoinSessionBtnClicked);

        }

        private void StartSessionBtnClicked()
        {
            NetworkManager.singleton.StartHost();
            canvas.enabled = false;
        }

        private void JoinSessionBtnClicked()
        {
            NetworkManager.singleton.networkAddress = GlobalVariables.ipAddress;
            NetworkManager.singleton.networkPort = 7777;
            NetworkManager.singleton.StartClient();
            canvas.enabled = false;
        }
    }
}

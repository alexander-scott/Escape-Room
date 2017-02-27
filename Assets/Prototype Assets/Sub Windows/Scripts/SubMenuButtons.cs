using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SubMenuButtons : MonoBehaviour {

    public Button startSessionBtn;
    //public Button subControls;

    private void Start()
    {
        startSessionBtn.onClick.AddListener(StartSessionBtnClicked);
    }

    private void StartSessionBtnClicked()
    {
        //NetworkManager.singleton.StartServer();
    }
}

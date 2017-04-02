using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using NetworkLib;
using System;

namespace Assets.Prototype_Assets
{
    public class SendDataToCentralScene : MonoBehaviour
    {




        // Use this for initialization
        void Start()
        {
            NetworkManager.singleton.networkAddress = GlobalVariables.ipAddress;
            NetworkManager.singleton.networkPort = 7777;
            NetworkManager.singleton.StartClient();
        }

        // Update is called once per frame
        void Update()
        {
            PlayerPrefs.SetFloat("PosX", transform.position.x);
            PlayerPrefs.SetFloat("PosY", transform.position.y);
            PlayerPrefs.SetFloat("PosZ", transform.position.z);
            
        }

        void OnGUI()
        {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), String.Format("{0}, {1}, {2}", PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ")));

        }
        Vector3 getCraftPos()
        {
            return transform.position;

        }

        Quaternion getCraftRot()
        {
            return transform.rotation;
        }
    }
}
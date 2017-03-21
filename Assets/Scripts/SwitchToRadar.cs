using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NetworkLib;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Prototype_Assets
{
    public class SwitchToRadar : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseUp()
        {
            if (SceneManager.GetActiveScene().name == "CodeLevel")
            {
                SceneManager.LoadScene("Test");
                GlobalVariables.playerNumber = 5;
                GlobalVariables.IPRadar = true;
            }
        }
    }
}

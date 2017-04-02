using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NetworkLib;
using System;
using System.Collections;
using System.Collections.Generic;
public class SwitchToCodeMenu : MonoBehaviour {

    private GameObject IPRadarcam;

    // Use this for initialization
    void Start()
    {
        IPRadarcam = GameObject.FindGameObjectWithTag("IPRadarCamera");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseUp()
    {
        IPRadarcam.SetActive(false);
        if (SceneManager.GetActiveScene().name == "Test")
        {
            SceneManager.LoadScene("CodeLevel");
        }
    }
}

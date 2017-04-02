using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using NetworkLib;
using System;

public class UpdateCraftPosition : MonoBehaviour {
    public GameObject Red;
    public GameObject Yellow;
    public GameObject Blue;
    public GameObject Green;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 updatePosition = new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ"));

        GameObject.Find("Craft").GetComponent<Renderer>().transform.position = updatePosition;
        //transform.position.Set(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ"));

        if (PlayerPrefs.GetInt("fusesCollected_Capsule Red") == 1)
        {
            Destroy(Red);
        }
        if (PlayerPrefs.GetInt("fusesCollected_Capsule Green") == 1)
        {
            Destroy(Green);
        }
        if (PlayerPrefs.GetInt("fusesCollected_Capsule Blue") == 1)
        {
            Destroy(Blue);
        }
        if (PlayerPrefs.GetInt("fusesCollected_Capsule Yellow") == 1)
        {
            Destroy(Yellow);
        }

    }
    
    void OnGUI()
    {
        Component rend = GameObject.Find("Craft").GetComponent<Renderer>();
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), String.Format("Craft: {0}, {1}, {2} \n Cube:{3}, {4}, {5}", PlayerPrefs.GetInt("RedFuseAvailable"), PlayerPrefs.GetInt("GreenFuseAvailable"), PlayerPrefs.GetFloat("PosZ"), rend.transform.position.x, rend.transform.position.y, rend.transform.position.z));
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.rigidbody);
        Debug.Log("OnCollisionEnter");
        if (col.gameObject.name == "Dropoff")
        {
            if (PlayerPrefs.GetInt("fuseCollected_Capsule Red") == 1)
            {
                PlayerPrefs.SetInt("RedFuseAvailable", 1);
                PlayerPrefs.SetInt("fuseCollected_Capsule Red", 0);
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), String.Format("{0}", "Red"));
            }
            if (PlayerPrefs.GetInt("fuseCollected_Capsule Green") == 1)
            {
                PlayerPrefs.SetInt("GreenFuseAvailable", 1);
                PlayerPrefs.SetInt("fuseCollected_Capsule Green", 0);
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), String.Format("{0}", "Green"));
            }
            if (PlayerPrefs.GetInt("fuseCollected_Capsule Blue") == 1)
            {
                PlayerPrefs.SetInt("BlueFuseAvailable", 1);
                PlayerPrefs.SetInt("fuseCollected_Capsule Blue", 0);
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), String.Format("{0}" ,"Blue"));
            }
            if (PlayerPrefs.GetInt("fuseCollected_Capsule Yellow") == 1)
            {
                PlayerPrefs.SetInt("YellowFuseAvailable", 1);
                PlayerPrefs.SetInt("fuseCollected_Capsule Yellow", 0);
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), String.Format("{0}", "Yellow"));
            }
            PlayerPrefs.GetInt("fusesCollected_" + gameObject.name, 1);


            Destroy(gameObject);
        }
    }
}

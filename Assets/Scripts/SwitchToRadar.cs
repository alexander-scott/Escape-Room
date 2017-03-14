using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToRadar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseUp()
    {
        if(SceneManager.GetActiveScene().name == "CodeLevel")
            SceneManager.LoadScene("RadarScreen");
        else if(SceneManager.GetActiveScene().name == "RadarScreen")
            SceneManager.LoadScene("CodeLevel");


    }
}

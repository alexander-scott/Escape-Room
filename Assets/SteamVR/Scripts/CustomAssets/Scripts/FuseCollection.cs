using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseCollection : MonoBehaviour {
    private GameObject collidingObject;
    private string objectName;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("ontriggerEnter This One");
        Debug.Log("(col.gameObject.name");

        if (col.gameObject.name == "Craft")
        {
            Debug.Log("Dropoff");
            if (PlayerPrefs.GetInt("fusesCollected_Capsule Red") == 1)
            {
                PlayerPrefs.SetInt("RedFuseAvailable", 1);
                PlayerPrefs.SetInt("fusesCollected_Capsule Red", 0);

            }
            if (PlayerPrefs.GetInt("fusesCollected_Capsule Green") == 1)
            {
                PlayerPrefs.SetInt("GreenFuseAvailable", 1);
                PlayerPrefs.SetInt("fusesCollected_Capsule Green", 0);
            }
            if (PlayerPrefs.GetInt("fusesCollected_Capsule Blue") == 1)
            {
                PlayerPrefs.SetInt("BlueFuseAvailable", 1);
                PlayerPrefs.SetInt("fusesCollected_Capsule Blue", 0);
            }
            if (PlayerPrefs.GetInt("fusesCollected_Capsule Yellow") == 1)
            {
                PlayerPrefs.SetInt("YellowFuseAvailable", 1);
                PlayerPrefs.SetInt("fusesCollected_Capsule Yellow", 0);
            }
        }
    }
}

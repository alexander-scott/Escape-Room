using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusePickup : MonoBehaviour {
    private GameObject collidingObject;
    public int fusesCollected = 0;
    // Use this for initialization
    void Start () {
        PlayerPrefs.SetInt("fusesCollected_Capsule Red", 0);
        PlayerPrefs.SetInt("fusesCollected_Capsule Blue", 0);
        PlayerPrefs.SetInt("fusesCollected_Capsule Yellow", 0);
        PlayerPrefs.SetInt("fusesCollected_Capsule Green", 0);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

     void OnCollisionEnter(Collision col)
    {
       Debug.Log("CollisionEnter Pickup: " + this.gameObject.name);

       switch (this.gameObject.name)
        {
            case "Capsule Red":
                PlayerPrefs.SetInt("fusesCollected_Capsule Red", 1);
                break;
            case "Capsule Blue":
                PlayerPrefs.SetInt("fusesCollected_Capsule Blue", 1);
                break;
            case "Capsule Yellow":
                PlayerPrefs.SetInt("fusesCollected_Capsule Yellow", 1);
                break;
            case "Capsule Green":
                PlayerPrefs.SetInt("fusesCollected_Capsule Green", 1);
                break;
            default:
                break;
        }

        if (col.gameObject.name == "Craft")
        {
            PlayerPrefs.SetInt("fusesCollected_" + gameObject.name, 1);
            Destroy(gameObject);
        }

 
    }

}


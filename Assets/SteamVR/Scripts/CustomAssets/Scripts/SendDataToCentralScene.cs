using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendDataToCentralScene : MonoBehaviour
{

    
    

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat("PosX", transform.position.x);
        PlayerPrefs.SetFloat("PosY", transform.position.y);
        PlayerPrefs.SetFloat("PosZ", transform.position.z);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRot : MonoBehaviour {
    public bool lockX;
    public bool lockY;
    public bool lockZ;

    private float lockFloat;
	// Use this for initialization
	void Start () {
        lockFloat = 0.0f;


	}
	
	// Update is called once per frame
	void Update () {

        Vector3 tempVec = transform.eulerAngles;

        if (lockX)
            tempVec = new Vector3(lockFloat, tempVec.y, tempVec.z);

        if (lockY)
            tempVec = new Vector3(tempVec.x, lockFloat, tempVec.z);

        if (lockZ)
            tempVec = new Vector3(tempVec.x, tempVec.y, tempVec.z);

        transform.eulerAngles = tempVec;


	}
}

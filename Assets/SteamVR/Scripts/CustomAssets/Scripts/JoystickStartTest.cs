using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickStartTest : MonoBehaviour {
	public GameObject theJoystick;

	private ConstrainMovementPlane scriptToAccess;
	// Use this for initialization
	void Start () {
		scriptToAccess = theJoystick.GetComponent<ConstrainMovementPlane> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 startPos = scriptToAccess.startPosOfJoystick;
		startPos.y += 0.2f;
		transform.position = startPos;
	}
}

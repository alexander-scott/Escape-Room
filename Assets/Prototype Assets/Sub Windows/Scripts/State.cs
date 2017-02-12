using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour {

    public enum cState { WindowSelection, ActiveWindow }

    public cState currentState;

	// Use this for initialization
	void Start () {
        currentState = cState.WindowSelection;
	}
	
	public void setState(cState newState)
    {
        currentState = newState;
    }

    public cState getState()
    {
        return currentState;
    }
}

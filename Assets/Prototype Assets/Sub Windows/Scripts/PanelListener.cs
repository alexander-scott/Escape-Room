using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelListener : MonoBehaviour {

    State state;
    public GameObject panel;

	// Use this for initialization
	void Start () {
        state = GetComponent<State>();
	}
	
	// Update is called once per frame
	void Update () {
        if(state.getState() == State.cState.WindowSelection)
        {
            panel.SetActive(true);
        } else
        {
            panel.SetActive(false);
        }
	}
}

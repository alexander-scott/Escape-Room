using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackSpace : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {

        TextMesh code = GameObject.Find("txtCodeField").GetComponent<TextMesh>();
        code.text = code.text.Remove(code.text.Length - 1, 1);
    }
    
}

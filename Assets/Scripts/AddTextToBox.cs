using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTextToBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        GameObject txtCodeField = GameObject.Find("txtCodeField");
        txtCodeField.GetComponent<TextMesh>().text += gameObject.GetComponent<TextMesh>().text;
        CheckCode();
    } 

    private void CheckCode()
    {
        GameObject txtCodeField = GameObject.Find("txtCodeField");
        TextMesh tm = txtCodeField.GetComponent<TextMesh>();
        TextMesh authorised = GameObject.Find("txtAuthorised").GetComponent<TextMesh>();
        if (tm.text.Length >=6 )
        {
            if(tm.text == "621437")
            {
                authorised.text = "Authorised";
                authorised.color = Color.green;
            }
            else
            {
                authorised.text = "Unauthorised";
                authorised.color = Color.red;
                tm.text = string.Empty;
            }
        }
    }
}

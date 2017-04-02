using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseColours : MonoBehaviour {

    private string objectName;
    private string setColour;


    // Use this for initialization
    void Start () {
        //objectName = gameObject.name;

        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Specular");

        
        switch (objectName)
        {
            case "Capsule Red":
               // rend.material.SetColor("_SpecColor", Color.red);
                rend.material.color = Color.green;
                setColour = "red";
                break;
            case "Capsule Blue":
                //  rend.material.SetColor("_SpecColor", Color.blue);
                rend.material.color = Color.green;
                setColour = "blue";
                break;
            case "Capsule Yellow":
                //  rend.material.SetColor("_SpecColor", Color.yellow);
                rend.material.color = Color.green;
                setColour = "yellow";
                break;
            case "Capsule Green":
                //  rend.material.SetColor("_SpecColor", Color.green);
                rend.material.color = Color.green;
                setColour = "green";
                break;
            default:
                setColour = "white";
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
    }
}

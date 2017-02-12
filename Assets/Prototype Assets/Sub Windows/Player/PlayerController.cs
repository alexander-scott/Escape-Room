using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;



public class PlayerController : NetworkBehaviour
{

    public string debugText, debugText2;
    Rect textArea = new Rect(10, 10, Screen.width, Screen.height);
    Rect textArea2 = new Rect(10, 20, Screen.width, Screen.height);
    public Camera cam;
    int activeCamera = 0;

    //how far the window is away from the sub
    const float kWindowOffset = 5.0f;

    //the sub...
    private GameObject mSubmarine;

    public void Start()
    {
        //find sub in scene
        mSubmarine = GameObject.FindGameObjectWithTag("Submarine");

        RenderSettings.fog = true;

        //init to North Camera
        this.transform.position = new Vector3(mSubmarine.transform.position.x + 5, mSubmarine.transform.position.y, mSubmarine.transform.position.z);
        //this.transform.position = new Vector3(5.0f, 0.0f, 0f);
        this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
        //this.transform.eulerAngles = new Vector3(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
        debugText = "Camera 1";

        if (isLocalPlayer) return;
        cam.enabled = false;
    }

    public void Update()
    {
        

        if (!isLocalPlayer)
        {
            return;
        }

        //let's try this
        //mSubmarine = GameObject.FindGameObjectWithTag("Submarine");


        Quaternion tempRotation = mSubmarine.transform.rotation;

        switch (activeCamera)
        {
            
            //North facing camera
            case 0:
                this.transform.position = new Vector3(mSubmarine.transform.position.x + kWindowOffset, mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                tempRotation *= Quaternion.Euler(0, 180, 0);
                this.transform.rotation = tempRotation;
                debugText = "Camera North";
                break;
            //East facing camera
            case 1:
                this.transform.position = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z - kWindowOffset);
                tempRotation *= Quaternion.Euler(0, 270, 0);
                this.transform.rotation = tempRotation;
                debugText = "Camera East";
                break;
            //South facing camera
            case 2:
                this.transform.position = new Vector3(mSubmarine.transform.position.x - (2 * kWindowOffset), mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                tempRotation *= Quaternion.Euler(0, 0, 0);
                this.transform.rotation = tempRotation;
                debugText = "Camera South";
                break;
            //West facing camera
            case 3:
                this.transform.position = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z + kWindowOffset);
                tempRotation *= Quaternion.Euler(0, 90, 0);
                this.transform.rotation = tempRotation;
                debugText = "Camera West";
                break;
        }

        SwitchCamera();

    }

    private void SwitchCamera()
    {
        if (Input.GetMouseButtonDown(0) /*|| Input.GetTouch(0).pressure > 0.5f*/)
        {
            activeCamera++;

            if (activeCamera == 4)
            {
                activeCamera = 0;
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void OnGUI()
    {
        if(isLocalPlayer)
       GUI.Label(textArea, debugText);
       GUI.Label(textArea2, debugText2);
    }

}

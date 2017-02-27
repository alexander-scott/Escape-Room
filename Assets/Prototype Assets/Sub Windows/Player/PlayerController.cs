using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Assets.Prototype_Assets
{
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

        //underwater stuff, needs extracting out
        public float underwaterLevel = 6.4f;

        //default fog settings from scene
        private bool defaultFog;
        private Color defaultFogColour;
        private float defaultFogDensity;
        private Material defaultSkybox;
        private Material noSkybox;

        public void Start()
        {
            defaultFog = RenderSettings.fog;
            defaultFogColour = RenderSettings.fogColor;
            defaultFogDensity = RenderSettings.fogDensity;
            defaultSkybox = RenderSettings.skybox;


            this.cam.backgroundColor = new Color(0, 0.4f, 0.7f, 1);

            //find sub in scene
            mSubmarine = GameObject.FindGameObjectWithTag("Submarine");

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
            if (this.transform.position.y < underwaterLevel)
            {
                //underwater
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0, 0.4f, 0.7f, 0.6f);
                RenderSettings.fogDensity = 0.04f;
                RenderSettings.skybox = noSkybox;
            }
            else
            {
                //above water
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColour;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }

            if (!isLocalPlayer)
            {
                return;
            }

            switch (GlobalVariables.playerNumber)
            {
                //North facing camera
                case 0:
                    this.transform.position = new Vector3(mSubmarine.transform.position.x + kWindowOffset, mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                    this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);


                    //this.transform.rotation = Quaternion.LookRotation(new Vector3(mSubmarine.transform.position.x + kWindowOffset, mSubmarine.transform.position.y, mSubmarine.transform.position.z));



                    //tempRotation *= Quaternion.Euler(0, 180, 0);
                    //this.transform.rotation = tempRotation;
                    debugText = "Camera North";
                    break;
                //East facing camera
                case 1:
                    this.transform.position = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z - kWindowOffset);
                    this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y - 90.0f, mSubmarine.transform.rotation.z);

                    //should be perp to front facing vector +90deg (clockwise)
                    //take cross product of the directional vector (pos) and the vector straight up (mast)
                    //Vector3 posPerpVector = Vector3.Cross(mSubmarine.transform.position, mSubmarine.transform.up);
                    //posPerpVector.Scale(new Vector3(-1.0f, -1.0f, -1.0f));
                    //this.transform.position = new Vector3(posPerpVector.x, posPerpVector.y, posPerpVector.z += kWindowOffset);
                    //this.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);

                    //this.transform.right = mSubmarine.transform.right;
                    //this.transform.rotation = Quaternion.LookRotation(this.transform.position);

                    //tempRotation *= Quaternion.Euler(0, 270, 0);
                    //this.transform.rotation = tempRotation;
                    debugText = "Camera East";
                    break;
                //South facing camera
                case 2:

                    Vector3 oppositeVector = mSubmarine.transform.position;
                    this.transform.position = new Vector3(mSubmarine.transform.position.x - (2 * kWindowOffset), mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                    this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y, mSubmarine.transform.rotation.z);

                    //this.transform.position = new Vector3(oppositeVector.x + (2 * kWindowOffset), oppositeVector.y, oppositeVector.z);
                    //this.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                    //this.transform.rotation = Quaternion.LookRotation(this.transform.position);


                    debugText = "Camera South";
                    break;
                //West facing camera
                case 3:
                    this.transform.position = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z + kWindowOffset);
                    this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 90.0f, mSubmarine.transform.rotation.z);

                    //Vector3 negPerpVector = Vector3.Cross(mSubmarine.transform.position, mSubmarine.transform.up) * -1;
                    //this.transform.position = new Vector3(negPerpVector.x, negPerpVector.y, negPerpVector.z += kWindowOffset);
                    //this.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
                    //this.transform.rotation = Quaternion.LookRotation(this.transform.position);
                    //should be perp to front facing vector -90deg (anticlockwise)

                    debugText = "Camera West";
                    break;
            }

            //SwitchCamera();

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
            if (isLocalPlayer)
                GUI.Label(textArea, debugText);
            GUI.Label(textArea2, debugText2);
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Assets.Prototype_Assets
{
    public class PlayerController : NetworkBehaviour
    {
        private bool shaking;
        private Vector3 originPos;
        private Quaternion originRot;

        //camera shake vars
        public float shakeDecay;
        public float shakeIntensity;
        public float shakeScalar = 0.01f;
        private float debugFloat = 0.0f; //used to periodically test camera shake

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
            shaking = false;
            shakeDecay = 0.02f;
            shakeIntensity = 0.2f;

            debugFloat = 0.0f;
            originRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);


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
            if (shaking)
            {
                if (shakeIntensity > 0)
                {
                    CameraShake();
                }
                else
                {
                    shaking = false;
                }
            }
            else
            {
                if (debugFloat > 300.0f)
                {
                    shaking = true;
                    debugFloat = 0.0f;
                    shakeIntensity = 3.0f;
                }
                else
                {
                    debugFloat++;
                    switch (GlobalVariables.playerNumber)
                    {
                        //North facing camera
                        case 0:
                            this.transform.position = new Vector3(mSubmarine.transform.position.x + kWindowOffset, mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                            this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
                            debugText = "Camera North";
                            break;
                        //East facing camera
                        case 1:
                            this.transform.position = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z - kWindowOffset);
                            this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y - 90.0f, mSubmarine.transform.rotation.z);
                            debugText = "Camera East";
                            break;
                        //South facing camera
                        case 2:

                            Vector3 oppositeVector = mSubmarine.transform.position;
                            this.transform.position = new Vector3(mSubmarine.transform.position.x - (2 * kWindowOffset), mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                            this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y, mSubmarine.transform.rotation.z);
                            debugText = "Camera South";
                            break;
                        //West facing camera
                        case 3:
                            this.transform.position = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z + kWindowOffset);
                            this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 90.0f, mSubmarine.transform.rotation.z);
                            debugText = "Camera West";
                            break;
                    }
                }
                //SwitchCamera();
            }
        }

        private void CameraShake()
        {
            this.transform.position = originPos + Random.insideUnitSphere * shakeIntensity;
            this.transform.rotation = new Quaternion
            (
                originRot.x + Random.Range(0, shakeIntensity) * shakeScalar,
                originRot.y + Random.Range(1, 1),
                originRot.z + Random.Range(0, shakeIntensity) * shakeScalar,
                originRot.w + Random.Range(1, 1) * shakeScalar
            );

            shakeIntensity -= shakeDecay;
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

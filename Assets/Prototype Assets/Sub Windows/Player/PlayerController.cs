using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using NetworkLib;


namespace Assets.Prototype_Assets
{
    public class PlayerController : NetworkBehaviour
    {
        private bool shaking = false;
        private bool subHasBeenHit = false;
        //camera shake vars
        public float shakeDecay;
        public float shakeIntensity;
        public float shakeScalar = 0.01f;
        private float debugFloat = 0.0f; //used to periodically test camera shake

        public string debugText, debugText2;
        Rect textArea = new Rect(10, 10, Screen.width, Screen.height);
        Rect textArea2 = new Rect(10, 20, Screen.width, Screen.height);
        public Camera cam;
        private Camera[] radarCams;
        int activeCamera = 0;
        public RadarSubV2 radv2 = new RadarSubV2();

        //how far the window is away from the sub
        const float kWindowOffset = 5.0f;

        //the sub...
        private GameObject mSubmarine;

        //That whale/shark thing
        private GameObject mWhale;

        //underwater stuff, needs extracting out
        public float underwaterLevel = 6.4f;

        private float groundLevel = -22.0f;

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
            //originRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);

            radarCams = Camera.allCameras;
            radarCams[2].gameObject.SetActive(false);
            
            defaultFog = RenderSettings.fog;
            defaultFogColour = RenderSettings.fogColor;
            defaultFogDensity = RenderSettings.fogDensity;
            defaultSkybox = RenderSettings.skybox;


            this.cam.backgroundColor = new Color(0, 0.4f, 0.7f, 1);

            //find sub in scene
            mSubmarine = GameObject.FindGameObjectWithTag("Submarine");

            //find the whale
            mWhale = GameObject.FindGameObjectWithTag("Whale");

            //init to North Camera
            this.transform.position = new Vector3(mSubmarine.transform.position.x + 5, mSubmarine.transform.position.y, mSubmarine.transform.position.z);
            //this.transform.position = new Vector3(5.0f, 0.0f, 0f);
            this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
            //this.transform.eulerAngles = new Vector3(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
            debugText = "Camera not setup";

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

            if (isServer && GlobalVariables.escapeStarted)
            {
                if (mSubmarine.transform.position.y <= groundLevel)
                {
                    //Stop decsent
                    
                    //WHALE COMING IN BRO
                    mWhale.transform.LookAt(mSubmarine.transform.position);
                    mWhale.transform.Rotate(0.0f, 180.0f, 0.0f);

                    Vector3 tar = new Vector3(mSubmarine.transform.position.x + 5.0f, mSubmarine.transform.position.y + 1.0f, mSubmarine.transform.position.z);
                    mWhale.transform.position = Vector3.MoveTowards(mWhale.transform.position, tar, 5.0f * Time.deltaTime);

                    if (mWhale.transform.position == tar)
                    {
                        //@TODO Change this to only happen once.
                        Packet p = new Packet((int)PacketType.SHAKE, "Server");

                        for (int i = 0; i < Server.udpClients.Count; i++)
                        {
                            Server.udpClients[i].SendPacket(p);
                        }
                    }
                }


                if (mSubmarine.transform.position.y > groundLevel)
                {
                    //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.01f, this.transform.position.z);

                    Vector3 groundPos = new Vector3(mSubmarine.transform.position.x, -25.0f, mSubmarine.transform.position.z);
                    mSubmarine.transform.position = Vector3.MoveTowards(mSubmarine.transform.position, groundPos, 10.0f * Time.deltaTime);

                    Debug.Log("Decending");
                }
            }


            if (!isLocalPlayer)
            {
                return;
            }

            switch (GlobalVariables.playerNumber)
            {
                //North facing camera
                case 0:
                    Quaternion northRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
                    Vector3 northPos = new Vector3(mSubmarine.transform.position.x + kWindowOffset, mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                    this.transform.position = northPos;
                    this.transform.rotation = northRot;
                    ShakeAtInterval();
                    debugText = "Camera North";
                    break;
                //East facing camera
                case 1:
                    Quaternion eastRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y - 90.0f, mSubmarine.transform.rotation.z);
                    Vector3 eastPos = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z - kWindowOffset);
                    this.transform.rotation = eastRot;
                    this.transform.position = eastPos;
                    ShakeAtInterval();
                    debugText = "Camera East";
                    break;
                //South facing camera
                case 2:
                    Quaternion southRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y, mSubmarine.transform.rotation.z);
                    Vector3 southPos = new Vector3(mSubmarine.transform.position.x - (2 * kWindowOffset), mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                    this.transform.position = southPos;
                    this.transform.rotation = southRot;
                    ShakeAtInterval();
                    debugText = "Camera South";
                    break;
                //West facing camera
                case 3:
                    Quaternion westRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 90.0f, mSubmarine.transform.rotation.z);
                    Vector3 westPos = new Vector3(mSubmarine.transform.position.x, mSubmarine.transform.position.y, mSubmarine.transform.position.z + kWindowOffset);
                    this.transform.position = westPos;
                    this.transform.rotation = westRot;
                    ShakeAtInterval();
                    debugText = "Camera West";
                    break;
                //RadarCamera
                case 4:
                    debugText = "Camera Radar";
                    radarCams[2].gameObject.SetActive(true);
                    radarCams[1].gameObject.SetActive(false);
                    //radarCams[0].gameObject.SetActive(false);
                    break;
            }
            //SwitchCamera();
        }

        private void CameraShake()
        {
            Vector3 origPos = this.transform.position;
            Quaternion origRot = this.transform.rotation;

            this.transform.position = origPos + Random.insideUnitSphere * shakeIntensity;
            this.transform.rotation = new Quaternion
            (
                origRot.x + Random.Range(0, shakeIntensity) * shakeScalar,
                origRot.y + Random.Range(1, 1),
                origRot.z + Random.Range(0, shakeIntensity) * shakeScalar,
                origRot.w + Random.Range(1, 1) * shakeScalar
            );

            shakeIntensity -= shakeDecay;
        }

        private void ShakeAtInterval()
        {
            if (shaking)
            {
                if (shakeIntensity > 0)
                {
                    CameraShake();
                }
                else
                {
                    shaking = false;
                    shakeIntensity = 0.2f;
                }
            }
        }

        public void EnableShake(Packet p)
        {
            shaking = true;
        }

        private void SwitchCamera()
        {
            if (Input.GetMouseButtonDown(0) /*|| Input.GetTouch(0).pressure > 0.5f*/)
            {
                activeCamera++;

                if (activeCamera == 5)
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

            if (!GlobalVariables.escapeStarted)
            {
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.UpperCenter;
                GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50), "Waiting for escape to start", centeredStyle);
            }

            GUI.Label(textArea2, debugText2);
        }
    }
}

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
        private GameObject Radarcam;
        private GameObject IPRadarcam;
        int activeCamera = 0;
        public Vector3 whaleEndPos;
        private bool whaleIsLeaving = false;
        private bool whaleIsApproaching = true;
        private bool descentPacketSent = false;
        private float descendSpeed = 1f;

        //how far the window is away from the sub
        const float kWindowOffset = 5.0f;

        //the sub...
        private GameObject mSubmarine;

        //That whale/shark thing
        private GameObject mWhale;

        //underwater stuff, needs extracting out
        public float underwaterLevel = 6.4f;

        private float groundLevel = -22.0f;
        private float descendWaitTimer = 5f;

        //default fog settings from scene
        private bool defaultFog;
        private Color defaultFogColour;
        private float defaultFogDensity;
        private Material defaultSkybox;
        private Material noSkybox;
        public float minWhaleSpeed;
        private float whaleSpeed;
        public float maxWhaleSpeed;

        public void Start()
        {
            shaking = false;
            shakeDecay = 0.02f;
            shakeIntensity = 0.4f;
            minWhaleSpeed = 1.0f;
            whaleSpeed = minWhaleSpeed;
            maxWhaleSpeed = 10.0f;
            whaleEndPos = new Vector3(-50.0f, -20.0f, 0.0f);


            debugFloat = 0.0f;
            //originRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);


            defaultFog = RenderSettings.fog;
            defaultFogColour = RenderSettings.fogColor;
            defaultFogDensity = RenderSettings.fogDensity;
            defaultSkybox = RenderSettings.skybox;


            this.cam.backgroundColor = new Color(0, 0.4f, 0.7f, 1);

            //find sub in scene
            mSubmarine = GameObject.FindGameObjectWithTag("Submarine");
 
            //find the whale
            mWhale = GameObject.FindGameObjectWithTag("Whale");

            //Find the radar camera
            Radarcam = GameObject.FindGameObjectWithTag("RadarCamera");

            //Find the ipad radar camera
            IPRadarcam = GameObject.FindGameObjectWithTag("IPRadarCamera");

            //init to North Camera
            this.transform.position = new Vector3(mSubmarine.transform.position.x + 5, mSubmarine.transform.position.y, mSubmarine.transform.position.z);
            //this.transform.position = new Vector3(5.0f, 0.0f, 0f);
            this.transform.rotation = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
            //this.transform.eulerAngles = new Vector3(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 180.0f, mSubmarine.transform.rotation.z);
            debugText = "Camera not setup";
            if (!isServer)
            {
                Client.connect(GlobalVariables.ipAddress, LibProtocolType.UDP);
                Client.ClientPacketObserver.AddObserver((int)PacketType.SHAKE, EnableShake);
            }

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

            if (isServer && GlobalVariables.CheckProgression(GlobalVariables.EscapeState.EscapeStarted))
            {
                if (mSubmarine.transform.position.y <= groundLevel)
                {
                    //descent has not stopped
                    HandleWhaleMovement();

                    //happens only once
                    if (isServer && !descentPacketSent)
                    {
                        descentPacketSent = true;
                        EscapeRoomController.Instance.UpdateSingleEscapeStateOnClients(GlobalVariables.EscapeState.SubDescended, true);
                    }
                }

                if (mSubmarine.transform.position.y > groundLevel && descendWaitTimer < 0.1f)
                {
                    //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.01f, this.transform.position.z);

                    if (!GlobalVariables.CheckProgression(GlobalVariables.EscapeState.SubStartDescending))
                    {
                        EscapeRoomController.Instance.UpdateSingleEscapeStateOnClients(GlobalVariables.EscapeState.SubStartDescending, true);
                    }

                    Vector3 groundPos = new Vector3(mSubmarine.transform.position.x, -25.0f, mSubmarine.transform.position.z);
                    mSubmarine.transform.position = Vector3.MoveTowards(mSubmarine.transform.position, groundPos, descendSpeed * Time.deltaTime);
                }
                else
                {
                    descendWaitTimer -= Time.deltaTime;
                }
            }


            if (!isLocalPlayer)
            {
                return;
            }

            switch (GlobalVariables.playerNumber)
            {
                case 0:
                    //North facing camera (NE)
                    IPRadarcam.SetActive(false);
                    Radarcam.SetActive(false);
                    Quaternion northRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 270.0f, mSubmarine.transform.rotation.z);
                    Vector3 northPos = new Vector3(mSubmarine.transform.position.x + (2 * kWindowOffset), mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                    this.transform.position = northPos;
                    this.transform.rotation = northRot;
                    ShakeAtInterval();
                    debugText = "Camera North";
                    GlobalVariables.IPRadar = false;
                    break;
                case 1:
                    //East facing camera (SE)
                    IPRadarcam.SetActive(false);
                    Radarcam.SetActive(false);
                    Quaternion eastRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y - 90.0f, mSubmarine.transform.rotation.z);
                    Vector3 eastPos = new Vector3(mSubmarine.transform.position.x - kWindowOffset, mSubmarine.transform.position.y, mSubmarine.transform.position.z - kWindowOffset);
                    this.transform.rotation = eastRot;
                    this.transform.position = eastPos;
                    ShakeAtInterval();
                    debugText = "Camera East";
                    GlobalVariables.IPRadar = false;
                    break;
                case 2:
                    //South facing camera (SW)
                    IPRadarcam.SetActive(false);
                    Radarcam.SetActive(false);
                    Quaternion southRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 90, mSubmarine.transform.rotation.z);
                    Vector3 southPos = new Vector3(mSubmarine.transform.position.x - (2 * kWindowOffset), mSubmarine.transform.position.y, mSubmarine.transform.position.z);
                    this.transform.position = southPos;
                    this.transform.rotation = southRot;
                    ShakeAtInterval();
                    debugText = "Camera South";
                    GlobalVariables.IPRadar = false;
                    break;
                case 3:
                    //West facing camera (NW)
                    IPRadarcam.SetActive(false);
                    Radarcam.SetActive(false);
                    Quaternion westRot = Quaternion.Euler(mSubmarine.transform.rotation.x, mSubmarine.transform.rotation.y + 90.0f, mSubmarine.transform.rotation.z);
                    Vector3 westPos = new Vector3(mSubmarine.transform.position.x + (2 * kWindowOffset), mSubmarine.transform.position.y, mSubmarine.transform.position.z + kWindowOffset);
                    this.transform.position = westPos;
                    this.transform.rotation = westRot;
                    ShakeAtInterval();
                    debugText = "Camera West";
                    GlobalVariables.IPRadar = false;
                    break;
                //RadarCamera
                case 4:
                    debugText = "Camera Radar";
                    IPRadarcam.SetActive(false);
                    Radarcam.SetActive(true);
                    this.cam.gameObject.SetActive(false);
                    GlobalVariables.IPRadar = false;
                    break;
                //IPAD Radar Camera
                case 5:
                    debugText = "Camera Radar";
                    IPRadarcam.SetActive(true);
                    this.cam.gameObject.SetActive(false);
                    Radarcam.SetActive(false);
                    break;
            }
            //SwitchCamera();
        }

        //public void OnApplicationQuit()
        //{
        //    if (!isServer)
        //    {
        //        Client.stop();
        //        NetworkManager.singleton.StopClient();
        //    }
        //}

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

        private void HandleWhaleMovement()
        {
            Vector3 tar = new Vector3(mSubmarine.transform.position.x + 5.0f, mSubmarine.transform.position.y + 1.0f, mSubmarine.transform.position.z);

            if (whaleIsApproaching)
            {
                whaleSpeed += 0.1f;
                if(whaleSpeed > maxWhaleSpeed)
                {
                    whaleSpeed = maxWhaleSpeed;
                }

                mWhale.transform.LookAt(mSubmarine.transform.position);
                mWhale.transform.Rotate(0.0f, 180.0f, 0.0f);

                mWhale.transform.position = Vector3.MoveTowards(mWhale.transform.position, tar, whaleSpeed * Time.deltaTime);
            }
            if (mWhale != null && mWhale.transform.position == tar)
            {
                if (!GlobalVariables.CheckProgression(GlobalVariables.EscapeState.FuzesScattered))
                {
                    EscapeRoomController.Instance.UpdateSingleEscapeStateOnClients(GlobalVariables.EscapeState.FuzesScattered, true);
                }

                if (!whaleIsLeaving)
                {
                    SendCameraShakePacket();
                }
                whaleIsLeaving = true;
                whaleIsApproaching = false;
                //whaleSpeed = minWhaleSpeed;
            }

            if (mWhale != null && whaleIsLeaving)
            {
                SendWhaleAway();
            }
          
        }

        private void SendCameraShakePacket()
        {
            shaking = true;

            //Packet p = new Packet((int)PacketType.SHAKE, "Server");

            //for (int i = 0; i < Server.udpClients.Count; i++)
            //{
            //    Server.udpClients[i].SendPacket(p);
            //}
        }

        private void SendWhaleAway()
        {
            mWhale.transform.position = Vector3.MoveTowards(mWhale.transform.position, whaleEndPos, whaleSpeed * Time.deltaTime);
            if (mWhale.transform.position == whaleEndPos)
            {
                Destroy(mWhale);
            }
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
                    shakeIntensity = 0.4f;
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

            //if (GlobalVariables.escapeState == GlobalVariables.EscapeState.WaitingToStart)
            //{
            //    var centeredStyle = GUI.skin.GetStyle("Label");
            //    centeredStyle.alignment = TextAnchor.UpperCenter;
            //    GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50), "Waiting for escape to start", centeredStyle);
            //}

            GUI.Label(textArea2, debugText2);
        }
    }
}

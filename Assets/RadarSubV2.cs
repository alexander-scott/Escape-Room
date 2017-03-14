using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSubV2 : MonoBehaviour {

    // Blip information
    public bool radarBlip1Active;
    public Color radarBlip1Color = new Color(0, 0, 255);
    public string radarBlip1Tag;

    public bool radarBlip2Active;
    public Color radarBlip2Color = new Color(0, 255, 0);
    public string radarBlip2Tag;

    public bool radarBlip3Active;
    public Color radarBlip3Color = new Color(255, 0, 0);
    public string radarBlip3Tag;

    public bool radarBlip4Active;
    public Color radarBlip4Color = new Color(255, 0, 255);
    public string radarBlip4Tag;

    public GameObject radarPrefab;
    public float switchDistance;

    List<GameObject> borderObjects = new List<GameObject>();
    List<GameObject> radarObjects = new List<GameObject>();
    List<GameObject> borderObjectsupd = new List<GameObject>();
    List<GameObject> radarObjectsupd = new List<GameObject>();

    public AudioClip radarPing;
    private AudioSource source;

    public Transform helpTransform;

    private GameObject[] trackedObjects;
    private Vector3[] lastPosition;

    public GameObject RadarCam;

    // Use this for initialization
    void Start () {

        //RadarCam.SetActive(false);

        //// Draw blips
        //if (radarBlip1Active)
        //{
        //    // Find all game objects
        //    trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip1Tag);

        //    // Iterate through them and call drawBlip function
        //    foreach (GameObject go in trackedObjects)
        //    {
        //        createRadarObjects(go);
        //    }
        //}
        //if (radarBlip2Active)
        //{
        //    trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip2Tag);

        //    foreach (GameObject go in trackedObjects)
        //    {
        //        createRadarObjects(go);
        //    }
        //}
        //if (radarBlip3Active)
        //{
        //    trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip3Tag);

        //    foreach (GameObject go in trackedObjects)
        //    {
        //        createRadarObjects(go);
        //    }
        //}
        //if (radarBlip4Active)
        //{
        //    trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip4Tag);

        //    foreach (GameObject go in trackedObjects)
        //    {
        //        createRadarObjects(go);
        //    }
        //}

        source = GetComponent<AudioSource>();
        InvokeRepeating("PlayClipAndChange", 0.01f, 5.0f);
    }
	
	// Update is called once per frame
	void Update () {

        if (radarBlip1Active)
        {
            // Find all game objects
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip1Tag);
            
            // Iterate through them and call drawBlip function
            foreach (GameObject go in trackedObjects)
            {
                updateRadarObjects(go);
            }
        }
        if (radarBlip2Active)
        {
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip2Tag);

            foreach (GameObject go in trackedObjects)
            {
                updateRadarObjects(go);
            }
        }
        if (radarBlip3Active)
        {
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip3Tag);

            foreach (GameObject go in trackedObjects)
            {
                updateRadarObjects(go);
            }
        }
        if (radarBlip4Active)
        {
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip4Tag);

            foreach (GameObject go in trackedObjects)
            {
                updateRadarObjects(go);
            }
        }
    }

    void createRadarObjects(GameObject go)
    {
        
        //radarObjects = new List<GameObject>();
       // borderObjects = new List<GameObject>();

        GameObject k = Instantiate(radarPrefab, go.transform.position, Quaternion.identity) as GameObject;
        k.transform.SetParent(go.transform);
        GameObject j = Instantiate(radarPrefab, go.transform.position, Quaternion.identity) as GameObject;
        j.transform.SetParent(go.transform);
        drawBlips(k, j);

    }

    void updateRadarObjects(GameObject go)
    {
        clearBlips(go);
        GameObject k = Instantiate(radarPrefab, go.transform.position, Quaternion.identity) as GameObject;
        GameObject j = Instantiate(radarPrefab, go.transform.position, Quaternion.identity) as GameObject;
        k.transform.SetParent(go.transform);
        j.transform.SetParent(go.transform);
        drawBlips(k , j);
    }

    void drawBlips(GameObject k, GameObject j)
    {
       
            if (Vector3.Distance(k.transform.position, transform.position) > switchDistance)
            {
                helpTransform.LookAt(k.transform);
                //j.transform.position = transform.position + switchDistance * helpTransform.forward;
                j.layer = LayerMask.NameToLayer("Radar");
                k.layer = LayerMask.NameToLayer("Invisible");
            }
            else
            {
                j.layer = LayerMask.NameToLayer("Invisible");
                k.layer = LayerMask.NameToLayer("Radar");
            }
        
    }

    void clearBlips(GameObject go)
    {
        int children = go.transform.childCount;

        for (int i = 0; i < children; i++)
        {
            Transform po = go.transform.GetChild(i);
            Destroy(po.gameObject);
        }
    }

    void PlayClipAndChange()
    {
        source.PlayOneShot(radarPing);
    }

    public void changeCamera()
    {
        RadarCam.SetActive(true);
    }
}

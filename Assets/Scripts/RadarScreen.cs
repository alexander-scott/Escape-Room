using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RadarScreen : MonoBehaviour {

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

    public AudioClip radarPing;
    private AudioSource source;

    public Transform helpTransform;

    private GameObject[] trackedObjects;
    private Vector3[] lastPosition;

    // Use this for initialization
    void Start ()
    {
        //GameObject thePlayer = GameObject.FindGameObjectWithTag("Submarine");
        //RadarSubV2 playerScript = thePlayer.GetComponent<RadarSubV2>();
        //bool radarBlip1Active = playerScript.radarBlip1Active;

        GameObject[] goArray = SceneManager.GetSceneByName("Test").GetRootGameObjects();
        if (goArray.Length > 0)
        {
            GameObject rootGo = goArray[0];
            // Do something with rootGo here...
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
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
        drawBlips(k, j);
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

    //void PlayClipAndChange()
    //{
    //    source.PlayOneShot(radarPing);
    //}
}

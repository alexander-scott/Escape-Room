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
    List<GameObject> borderObjects;
    List<GameObject> radarObjects;

    // Use this for initialization
    void Start () {
        //createRadarObjects();
	}
	
	// Update is called once per frame
	void Update () {

        GameObject[] trackedObjects;

        // Draw blips
        if (radarBlip1Active)
        {
            // Find all game objects
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip1Tag);

            // Iterate through them and call drawBlip function
            foreach (GameObject go in trackedObjects)
            {
                createRadarObjects(go);
            }
        }
        if (radarBlip2Active)
        {
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip2Tag);

            foreach (GameObject go in trackedObjects)
            {
                createRadarObjects(go);
            }
        }
        if (radarBlip3Active)
        {
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip3Tag);

            foreach (GameObject go in trackedObjects)
            {
                createRadarObjects(go);
            }
        }
        if (radarBlip4Active)
        {
            trackedObjects = GameObject.FindGameObjectsWithTag(radarBlip4Tag);

            foreach (GameObject go in trackedObjects)
            {
                createRadarObjects(go);
            }
        }

        for (int i = 0; i < radarObjects.Count; i++)
        {
            //Destroy(radarObjects[i]);
            //radarObjects[i];
            DestroyObject(radarObjects[i], .5f);
        }
    }

    void createRadarObjects(GameObject go)
    {
        
        radarObjects = new List<GameObject>();

        GameObject k = Instantiate(radarPrefab, go.transform.position, Quaternion.identity) as GameObject;
        radarObjects.Add(k);
        //GameObject j = Instantiate(radarPrefab, go.transform.position, Quaternion.identity) as GameObject;
        //borderObjects.Add(j);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamDisplay : MonoBehaviour {

    public MeshRenderer[] UseWebcamTexture;
    private WebCamTexture webcamTexture;
    WebCamDevice[] devices;
    public Text debugText;
    private Color32[] data;  

    void Start()
    {
        devices = WebCamTexture.devices;

        debugText.text = "Webcams: " + devices.Length;

        webcamTexture = new WebCamTexture(devices[1].name);

        data = new Color32[webcamTexture.width * webcamTexture.height];

        foreach (MeshRenderer r in UseWebcamTexture)
        {
            r.material.mainTexture = webcamTexture;
        }

        webcamTexture.Play();

    }

    void Update()
    {
        webcamTexture.GetPixels32(data);
    }
    
}

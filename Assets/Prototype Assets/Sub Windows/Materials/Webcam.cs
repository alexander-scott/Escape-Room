using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using NetworkLib;

namespace Assets.Prototype_Assets
{

    public class Webcam : NetworkBehaviour
    {

        void Start()
        {
            WebCamTexture webcamTexture = new WebCamTexture();
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
    }
}

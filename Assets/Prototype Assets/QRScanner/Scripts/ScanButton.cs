using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

namespace Assets.Prototype_Assets
{
    public class ScanButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public CameraController cameraController;

        public void OnPointerDown(PointerEventData eventData)
        {
            cameraController.StartScanning();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            cameraController.EndScanning();
        }
    }
}

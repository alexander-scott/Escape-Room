using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class ScanButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CameraController cameraController;

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown(PointerEventData eventData)
    {
        cameraController.StartScanning();
    }

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerUp(PointerEventData eventData)
    {
        cameraController.EndScanning();
    }
}

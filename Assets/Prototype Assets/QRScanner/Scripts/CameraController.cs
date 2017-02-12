using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public RawImage image;
    public RectTransform imageParent;
    public AspectRatioFitter imageFitter;
    public Button scanButton;
    public Text infoText;
    public Button resumeButton;

    public float scanFrequency = 0.3f;

    // Device cameras
    WebCamDevice frontCameraDevice;
    WebCamDevice backCameraDevice;
    WebCamDevice activeCameraDevice;

    WebCamTexture frontCameraTexture;
    WebCamTexture backCameraTexture;
    WebCamTexture activeCameraTexture;

    // Image rotation
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);

    // Image uvRect
    Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

    // Image Parent's scale
    Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    Vector3 fixedScale = new Vector3(-1f, 1f, 1f);

    CurrentState currentState = CurrentState.Idle;

    enum CurrentState
    {
        Idle,
        Scanning,
        ResultFound,
    }

    void Start()
    {
        // Check for device cameras
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("No devices cameras found");
            return;
        }

        // Get the device's cameras and create WebCamTextures with them
        frontCameraDevice = WebCamTexture.devices.Last();
        backCameraDevice = WebCamTexture.devices.First();

        frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
        backCameraTexture = new WebCamTexture(backCameraDevice.name);

        // Set camera filter modes for a smoother looking image
        frontCameraTexture.filterMode = FilterMode.Trilinear;
        backCameraTexture.filterMode = FilterMode.Trilinear;

        // Set the camera to use by default
        SetActiveCamera(frontCameraTexture);

        //scanButton.OnPointerDown += StartScanning;
        //scanButton.OnPointerUp += EndScanning;
    }

    // Set the device camera to use and start it
    public void SetActiveCamera(WebCamTexture cameraToUse)
    {
        if (activeCameraTexture != null)
        {
            activeCameraTexture.Stop();
        }

        activeCameraTexture = cameraToUse;
        activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
            device.name == cameraToUse.deviceName);

        image.texture = activeCameraTexture;
        image.material.mainTexture = activeCameraTexture;

        activeCameraTexture.Play();
    }

    // Make adjustments to image every frame to be safe, since Unity isn't 
    // guaranteed to report correct data as soon as device camera is started
    void Update()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            return;
        }

        // Skip making adjustment for incorrect camera data
        if (activeCameraTexture.width < 100)
        {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // Rotate image to show correct orientation 
        rotationVector.z = -activeCameraTexture.videoRotationAngle;
        image.rectTransform.localEulerAngles = rotationVector;

        // Set AspectRatioFitter's ratio
        float videoRatio =
            (float)activeCameraTexture.width / (float)activeCameraTexture.height;
        imageFitter.aspectRatio = videoRatio;

        // Unflip if vertically flipped
        image.uvRect =
            activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        imageParent.localScale =
            activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;
    }


    // Switch between the device's front and back camera
    public void SwitchCamera()
    {
        SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ?
            backCameraTexture : frontCameraTexture);
    }

    // Pause the camera feed, leaving it as a still image
    public void PauseCameraFeed()
    {
        if (activeCameraTexture.isPlaying)
        {
            activeCameraTexture.Stop();
        }
    }

    // Resume the camera feed
    public void ResumeCameraFeed()
    {
        if (!activeCameraTexture.isPlaying)
        {
            activeCameraTexture.Play();
            infoText.text = "Awaiting input...";
            resumeButton.gameObject.SetActive(false);
        }
    }

    // Starts the coroutine that scans the video feed
    public void StartScanning()
    {
        currentState = CurrentState.Scanning;

        StartCoroutine(Scanning());

        infoText.text = "Scanning...";
    }

    // Stops scanning the video feed
    public void EndScanning()
    {
        if (currentState == CurrentState.Scanning)
        {
            currentState = CurrentState.Idle;

            StopCoroutine(Scanning());

            infoText.text = "Nothing found";
        }
    }

    // The coroutine which will scan the video feed every 'scanFrequency' secs
    private IEnumerator Scanning()
    {
        while (currentState == CurrentState.Scanning)
        {
            string output = ReadBarcodeFromFile.ReadTexture(activeCameraTexture);

            if (output != null) // If a QR Code was found
            {
                infoText.text = output;
                currentState = CurrentState.ResultFound;
                resumeButton.gameObject.SetActive(true);

                PauseCameraFeed();
            }

            yield return new WaitForSeconds(scanFrequency);
        }
    }
}
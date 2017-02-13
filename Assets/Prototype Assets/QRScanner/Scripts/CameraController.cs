using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using NetworkLib;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Prototype_Assets
{
    public class CameraController : MonoBehaviour
    {
        public RawImage image;
        public RectTransform imageParent;
        public AspectRatioFitter imageFitter;
        public GameObject scanButton;
        public Text infoText;
        public Sprite resumeSprite;
        public Sprite scanSprite;

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

        CurrentScanState currentState = CurrentScanState.Idle;

        private bool iconZooming = false;

        enum CurrentScanState
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

        public void ScanSuccess(string result)
        {
            infoText.text = result;
            currentState = CurrentScanState.ResultFound;
            scanButton.GetComponent<Image>().sprite = resumeSprite;

            /*********************************************************/
            //Packet p = new Packet((int)PacketType.QRCODE, result);
            //p.generalData.Add(result);
            //Client.SendPacket(p, LibProtocolType.UDP);
            /*********************************************************/

            PauseCameraFeed();
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

        // Switch between the device's front and back camera
        public void SwitchCamera()
        {
            if (activeCameraTexture.isPlaying)
            {
                SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ?
                backCameraTexture : frontCameraTexture);
            } 
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
                infoText.text = "";
            }
        }

        // Starts the coroutine that scans the video feed
        public void StartScanning()
        {
            // If we a result has been found then don't immediately go back to scanning.
            if (currentState == CurrentScanState.ResultFound)
            {
                scanButton.GetComponent<Image>().sprite = scanSprite;
                currentState = CurrentScanState.Idle;
                ResumeCameraFeed();
                return;
            }

            currentState = CurrentScanState.Scanning;

            StartCoroutine(Scanning());

            if (!iconZooming)
            {
                StartCoroutine(EnlargeIcon(1.1f, 0.3f));
            }

            StartCoroutine(RotateIcon(0.3f));
            
            infoText.text = "";
        }

        // Stops scanning the video feed
        public void EndScanning()
        {
            if (currentState == CurrentScanState.Scanning)
            {
                currentState = CurrentScanState.Idle;

                StopCoroutine(Scanning());
                StartCoroutine(ShrinkIcon(0.3f));

                //infoText.text = "Nothing found";
            }
        }

        // The coroutine which will scan the video feed every 'scanFrequency' secs
        private IEnumerator Scanning()
        {
            while (currentState == CurrentScanState.Scanning)
            {
                string output = ReadBarcodeFromFile.ReadTexture(activeCameraTexture);

                if (output != null) // If a QR Code was found
                {
                    ScanSuccess(output);
                }

                yield return new WaitForSeconds(scanFrequency);
            }
        }

        // Slightly enlarges the scan icon. Currently used when it is tapped.
        private IEnumerator EnlargeIcon(float increaseBy, float duration)
        {
            iconZooming = true;

            float smoothness = 0.02f;
            float progress = 0; // This float will serve as the 3rd parameter of the lerp function.
            float increment = smoothness / duration; // The amount of change to apply.

            RectTransform rt = scanButton.GetComponent<RectTransform>();
            float endScale = rt.localScale.x * increaseBy;

            while (progress < 1)
            {
                rt.localScale = new Vector3(Mathf.Lerp(rt.localScale.x, endScale,progress), 
                Mathf.Lerp(rt.localScale.y, endScale, progress));

                progress += increment;
                yield return new WaitForSeconds(smoothness);
            }
        }

        // Shrinks the scan icon back to 1f
        private IEnumerator ShrinkIcon(float duration)
        {
            float smoothness = 0.02f;
            float progress = 0; // This float will serve as the 3rd parameter of the lerp function.
            float increment = smoothness / duration; // The amount of change to apply.

            RectTransform rt = scanButton.GetComponent<RectTransform>();

            while (progress < 1)
            {
                rt.localScale = new Vector3(Mathf.Lerp(rt.localScale.x, 1f ,progress), 
                Mathf.Lerp(rt.localScale.y, 1f, progress));

                progress += increment;
                yield return new WaitForSeconds(smoothness);
            }

            iconZooming = false;
        }

        // Rotates the scan icon elliptically until the user stops scanning
        private IEnumerator RotateIcon(float revLength)
        {
            Image img = scanButton.GetComponent<Image>();

            float smoothness = 0.02f;
            float progress = 0; // This float will serve as the 3rd parameter of the lerp function.
            float increment = smoothness / revLength; // The amount of change to apply.

            float target = 0;

            // While we're in scanning mode, rotate the black circle to show the user that we are scanning.
            while (currentState == CurrentScanState.Scanning)
            {
                progress = 0;

                if (img.fillClockwise)
                {
                    target = 1;
                }
                else 
                {
                    target = 0;
                }

                img.fillClockwise = !img.fillClockwise;

                while (progress < 1) // Progress from one end of the rev to the other
                {
                    img.fillAmount = Mathf.Lerp(img.fillAmount, target, progress);

                    progress += increment;
                    yield return new WaitForSeconds(smoothness);
                }
            }

            // Make sure the icon is still visible. If the icon is currently hidden add an additional loop to make it visible again
            if (target == 0f)
            {
                progress = 0;

                if (img.fillClockwise)
                {
                    target = 1;
                }
                else 
                {
                    target = 0;
                }

                img.fillClockwise = !img.fillClockwise;

                while (progress < 1)
                {
                    img.fillAmount = Mathf.Lerp(img.fillAmount, target ,progress);

                    progress += increment;
                    yield return new WaitForSeconds(smoothness);
                }
            }
        }

        public void ReturnToHome()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
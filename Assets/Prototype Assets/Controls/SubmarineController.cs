using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NetworkLib;

namespace Assets.Prototype_Assets
{
    public class SubmarineController : MonoBehaviour
    {
        public Button moveButton;
        public Text moveText;

        private bool moving = false;

        void Start()
        {
            // MOVED TO MENUBUTTONS.CS
            //NetworkLib.Client.connect(GlobalVariables.ipAddress, LibProtocolType.UDP);
            
            moveText.text = ((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString();

            switch ((GlobalVariables.Direction)GlobalVariables.playerNumber)
            {
                case GlobalVariables.Direction.Forward:
                    moveButton.GetComponent<Image>().rectTransform.eulerAngles = new Vector3(0f, 0f, 0f);
                    break;

                case GlobalVariables.Direction.Backward:
                    moveButton.GetComponent<Image>().rectTransform.eulerAngles = new Vector3(0f, 0f, 180f);
                    break;

                case GlobalVariables.Direction.Left:
                    moveButton.GetComponent<Image>().rectTransform.eulerAngles = new Vector3(0f, 0f, 90f);
                    break;

                case GlobalVariables.Direction.Right:
                    moveButton.GetComponent<Image>().rectTransform.eulerAngles = new Vector3(0f, 0f, 270f);
                    break;
            }
        }

		public void StartMoving()
        {
            moving = true;

            Packet p = new Packet((int)PacketType.MOVE, ((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString());
            p.generalData.Add(((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString());
            Client.SendPacket(p);
        }

        public void EndMoving()
        {
            moving = false;

            Packet p = new Packet((int)PacketType.ENDMOVE, ((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString());
            p.generalData.Add(((GlobalVariables.Direction)GlobalVariables.playerNumber).ToString());
            Client.SendPacket(p);
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
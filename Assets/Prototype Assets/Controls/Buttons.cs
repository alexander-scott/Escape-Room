using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkLib;

namespace Assets.Prototype_Assets
{
    public class Buttons : MonoBehaviour
    { 
        public Button forwardButton;
        public Button backwardButton;
        public Button leftButton;
        public Button rightButton;

        // Use this for initialization
        void Start()
        {
            //NetworkLib.Client.connect("10.4.118.175", LibProtocolType.UDP);
            forwardButton.onClick.AddListener(forwardButtonPressed);
            backwardButton.onClick.AddListener(backwardButtonPressed);
            leftButton.onClick.AddListener(leftButtonPressed);
            rightButton.onClick.AddListener(rightButtonPressed);

        }

        void forwardButtonPressed()
        {
            //Construct a packet with a unique packet ID for the event eg. 1 = Movement command packet
            //Add enum to PacketType enum and cast back to an int for ease of reading
            //second arguement is SenderID 
            //(not currently used for movement but can be helpful for identifying the sending client on the Server end)
            Packet p = new Packet((int)PacketType.MOVE, "Forward");
            p.generalData.Add("Forward");
            Client.SendPacket(p, LibProtocolType.UDP);
        }

        void backwardButtonPressed()
        {
            Packet p = new Packet((int)PacketType.MOVE, "Backward");
            p.generalData.Add("Backward");
            Client.SendPacket(p, LibProtocolType.UDP);
        }

        void leftButtonPressed()
        {
            Packet p = new Packet((int)PacketType.MOVE, "Left");
            p.generalData.Add("Left");
            Client.SendPacket(p, LibProtocolType.UDP);
        }

        void rightButtonPressed()
        {
            Packet p = new Packet((int)PacketType.MOVE, "Right");
            p.generalData.Add("Right");
            Client.SendPacket(p, LibProtocolType.UDP);
        }
    }
}

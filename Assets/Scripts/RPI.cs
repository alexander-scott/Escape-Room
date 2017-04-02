using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;



public class RPI : MonoBehaviour {

    public enum CMD
    {
        PLAY_SUB_CONTROL_TUT,
        PLAY_ACCES_CHAMBER_PASS,
        PLAY_FUSES_COLLECTED,
        PLAY_OXYGEN_LVL_DECREASE,
        PLAY_SYSTEM_MALFUNC,
        PLAY_COMPUTER_GREETING,
        PLAY_FUSES_DISLODGED,
        PLAY_DIAGNOSTICS_ONLINE,
        PLAY_SUB_START_DESCEND,
        DO_OPEN_BOX,
        DO_LED_ON,
        DO_LED_OFF
    };

    public class Metalberry
    {
        private Dictionary<int, string> commands;

        string ip = "169.254.15.108";
        string port = "5000";

        public Metalberry()
        {
            commands = new Dictionary<int, string>();

            // Sounds
            commands.Add((int)CMD.PLAY_SUB_CONTROL_TUT, "Submarine_Controls_tut");
            commands.Add((int)CMD.PLAY_ACCES_CHAMBER_PASS, "Access_Chamber_pass");
            commands.Add((int)CMD.PLAY_FUSES_COLLECTED, "Fuses_collected");
            commands.Add((int)CMD.PLAY_OXYGEN_LVL_DECREASE, "Oxygen_decrease");
            commands.Add((int)CMD.PLAY_SYSTEM_MALFUNC, "System_malfunction");
            commands.Add((int)CMD.PLAY_COMPUTER_GREETING, "Computer_greeting");
            commands.Add((int)CMD.PLAY_FUSES_DISLODGED, "Fuses_dislodged");
            commands.Add((int)CMD.PLAY_DIAGNOSTICS_ONLINE, "Diagnostics_online");
            commands.Add((int)CMD.PLAY_SUB_START_DESCEND, "Submarine_descend");

            commands.Add((int)CMD.DO_OPEN_BOX, "Open_Box");

            commands.Add((int)CMD.DO_LED_ON, "LED_ON");
            commands.Add((int)CMD.DO_LED_OFF, "LED_OFF");
        }
        private string ConstructRequest(int command)
        {
            string cmd = commands[command];
            string request = "http://" + ip + ":" + port + "/" + cmd;
            return request;
        }
        public void Do(CMD command)
        {
            string requestString = ConstructRequest((int)command);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            Debug.Log("request " + requestString);
            
            WebResponse response = request.GetResponse();
            response.Close();

            //StreamReader sr = new StreamReader(response.GetResponseStream());
        }

        private IEnumerator SendResponse(string requestString)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            WebResponse response = request.GetResponse();

            yield return null;
        }
    }

    public Metalberry metalBerry;
    private void Start()
    {
        metalBerry = new Metalberry();
    }
    //public void TaskOnClick()
    //{
    //    ResponseTxt.text = metalBerry.Do(CMD.LED_OFF);
    //    //Debug.Log("Response: " + metalBerry.Do(CMD.BEEP));
    //}
}

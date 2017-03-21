using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public enum CMD
{
    PLAY_S_CONTROL_BUTTON,
    PLAY_ACCES_CHAMBER_PASS,
    PLAY_FUSES_COLLECTED,
    PLAY_OXYGEN_LVL_DECREASE,
    PLAY_SYSTEM_MALFUNC,
    PLAY_COMPUTER_GREETING,
    PLAY_FUSES_DISLODGED,
    PLAY_DIAGNOSTICS_ONLINE,
    DO_OPEN_BOX,
    DO_LED_ON,
    DO_LED_OFF
};

public class RPI : MonoBehaviour {
    
    public class Metalberry
    {
        private Dictionary<int, string> commands;

        string ip = "192.168.42.1";
        string port = "5000";

        public Metalberry()
        {
            commands = new Dictionary<int, string>();

            // Sounds
            commands.Add((int)CMD.PLAY_S_CONTROL_BUTTON, "S_Control_button");
            commands.Add((int)CMD.PLAY_ACCES_CHAMBER_PASS, "Access_Chamber_pass");
            commands.Add((int)CMD.PLAY_FUSES_COLLECTED, "Fuses_collected");
            commands.Add((int)CMD.PLAY_OXYGEN_LVL_DECREASE, "Oxygen_Level_decreasing");
            commands.Add((int)CMD.PLAY_SYSTEM_MALFUNC, "System_malfunction");
            commands.Add((int)CMD.PLAY_COMPUTER_GREETING, "Computer_greeting");
            commands.Add((int)CMD.PLAY_FUSES_DISLODGED, "Fuses_dislodged");
            commands.Add((int)CMD.PLAY_DIAGNOSTICS_ONLINE, "Diagnostics_online");

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
        public string Do(CMD command)
        {
            string requestString = ConstructRequest((int)command);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            Debug.Log("request " + requestString);


            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());

            return sr.ReadToEnd();
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

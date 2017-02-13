using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using NetworkLib;


public class SubMove : NetworkBehaviour
{

    bool forward = true;
    float kTranslateMultiplier = 3.0f;
    float kRotateMultiplier = 150.0f;
    float x, y, z;


    // Use this for initialization
    void Start()
    {

        //transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        //transform.Rotate(0.0f, 0.0f, 0.0f);

        //Starts a server to listen for movement commands if this client is the host
        if (isServer)
        {
            //Starts a UDP Server from the Network Lib
            NetworkLib.Server.start(LibProtocolType.UDP);
            //Create an Observer to listen for the Packet Type '1' is used for movement commands
            //Calls the method 'MoveSub' when recieved to handle the packet.
            Server.ServerPacketObserver.AddObserver(0, MoveSub);
        }

    }

    void Update()
    {
        //transform.Translate(x, y, z);

        //debugging movement with cameras
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * kRotateMultiplier;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * kTranslateMultiplier;


        float y = 0.0f;

        //dive to submerge
        if (Input.GetKey("m"))
        {
            y = 1.0f * Time.deltaTime * kTranslateMultiplier;
        }
        //rise back to surface
        else if (Input.GetKey("n"))
        {
            y = -1.0f * Time.deltaTime * kTranslateMultiplier;
        }

        transform.Rotate(0, x, 0);
        transform.Translate(z, y, 0);
    }

    void MoveSub(Packet p)
    {
        //If the data in the packet contains the forward command, move the Sub along
        //the relevant axis

        //Since the Server runs on a seperate thread, unable to directly update the translations here
        //since Unity is a fussy bitch and doesn't let you touch translations when not on main thread.
        if (p.generalData[0] == "Forward")
        {
            x += 0.1f;
        }

        if (p.generalData[0] == "Backward")
        {
            x -= 0.1f;
        }


    }
}

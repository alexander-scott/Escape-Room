using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControlFromJoystick : MonoBehaviour
{
    public GameObject theJoystickObject;
    public GameObject theRotatorObject;

    enum Direction
    {
        forwards = 0,
        left,
        right,
        backwards,
        stop,
    };

    private Rigidbody rb;
    public float rotSpeed;
    private Direction directionToGo;
    private int directionToGoInt;
    public float moveSpeed;

    public float maxSpeed;

    private GameObject closestSensorToJS;

    //private ConstrainMovementPlane scriptToAccess;

    //private Vector3 joystickCurrentPos;
    //private Vector3 joystickStartPos;

    //private float moveX;
    //private float moveY;
    //private float moveZ;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //scriptToAccess = theJoystickObject.GetComponent<ConstrainMovementPlane> ();

    }

    // Update is called once per frame
    void Update()
    {
        //joystickCurrentPos = scriptToAccess.currentPos;


        //Debug.Log ("Move X Recieve" + moveX);
        //Debug.Log ("Move Z Recieve" + moveZ);
        //moveX = (joystickStartPos.x - joystickCurrentPos.x) * -1;
        //moveY = (joystickStartY - joystickCurrentPos.y) * -1;
        //moveZ = (joystickStartPos.z - joystickCurrentPos.z) * -1;

        //if (moveX >= maxSpeed)
        //	moveX = maxSpeed;
        //else if (moveX <= maxSpeed * -1)
        //	moveX = maxSpeed * -1;

        //if (moveZ >= maxSpeed)
        //	moveZ = maxSpeed;
        //else if (moveZ <= maxSpeed * -1)
        //	moveZ = maxSpeed * -1;


        //transform.position = new Vector3(transform.position.x + moveX, transform.position.y, transform.position.z + moveZ);

        //Debug.Log ("New pos " + transform.position);

        CalculateMovement();

    }

    void CalculateMovement()
    {
        directionToGoInt = (int)theJoystickObject.GetComponent<ConstrainMovementPlane>().directionToGo;
        directionToGo = (Direction)directionToGoInt;

        Debug.Log("DirectionToGo " + directionToGo);
        if (directionToGo == Direction.forwards)
        {
            rb.velocity += (transform.forward * moveSpeed);
        }
        else if (directionToGo == Direction.backwards)
            rb.velocity += (transform.forward * moveSpeed) * -1;
        else if (directionToGo == Direction.left)
        {
            //rb.velocity += (transform.right * moveSpeed) * -1;
            transform.Rotate((Vector3.up * Time.deltaTime) * rotSpeed);
        }
        else if (directionToGo == Direction.right)
        {
            //rb.velocity += transform.right * moveSpeed;
            transform.Rotate(((Vector3.up * -1) * Time.deltaTime) * rotSpeed);
        }

            if (rb.velocity.magnitude > maxSpeed)
            {
                Vector3 tempVec3 = rb.velocity;
                tempVec3.Normalize();
                rb.velocity = tempVec3 * maxSpeed;
            }

        if (directionToGo == Direction.stop)
            rb.velocity = rb.velocity * 0.9f;
    }
}


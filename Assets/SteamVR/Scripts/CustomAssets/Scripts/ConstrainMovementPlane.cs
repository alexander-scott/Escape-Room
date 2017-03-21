using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstrainMovementPlane : MonoBehaviour
{
    public GameObject theSubCraft;
    public GameObject theHand1;
    public GameObject theHand2;
    public GameObject theJoystickStartPoint;
    private ControllerGrabObject controller1Script;
    private ControllerGrabObject controller2Script;
    private JoystickStartTest joystickStartPointScript;
    public float tolerance;

    

    public enum Direction
    {
        forwards = 0,
        left,
        right,
        backwards,
        stop,
    };

    public GameObject frontSensor;
    public GameObject rearSensor;
    public GameObject leftSensor;
    public GameObject rightSensor;
    public GameObject neutralSensor;

    public float joystickNullZone;      //The distance from center at which the joystick responds
    private GameObject closestSensor;
    public Direction directionToGo;

    public Vector3 startPosOfJoystick;
    private Vector3 subCraftStartPos;
    private Vector3 subCraftPosition;
    private Vector3 posToSubCraft;      //Position relative to subcraft
    private Vector3 posToStart;         //Position relative to start position. Important when following craft
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;
    public Vector3 StartMin;
    public Vector3 StartMax;
    public Vector3 currentPos;
    public Rigidbody rb;
    public float maxSpeed;
    public bool isGrabbed;     //Check if this object is grabbed. Needed for joystick snap-back
                               //Variables dictating how much the controlled object moves
    public float moveX;
    public float moveZ;
    // Use this for initialization
    void Start()
    {

        isGrabbed = false;

        controller1Script = theHand1.GetComponent<ControllerGrabObject>();
        controller2Script = theHand2.GetComponent<ControllerGrabObject>();
        joystickStartPointScript = theJoystickStartPoint.GetComponent<JoystickStartTest>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startPosOfJoystick = transform.position;
        subCraftPosition = theSubCraft.transform.position;


        minY = startPosOfJoystick.y;
        maxY = startPosOfJoystick.y;



        StartMin = new Vector3(minX, minY, minZ);
        StartMax = new Vector3(maxX, maxY, maxZ);

        minX = startPosOfJoystick.x - tolerance;
        maxX = startPosOfJoystick.x + tolerance;

        minZ = startPosOfJoystick.z - tolerance;
        maxZ = startPosOfJoystick.z + tolerance;

        currentPos = transform.position;

        subCraftStartPos = theSubCraft.transform.position;

        //Position relative to sub craft
        posToSubCraft = new Vector3(startPosOfJoystick.x - subCraftPosition.x, startPosOfJoystick.y - subCraftPosition.y, startPosOfJoystick.z - subCraftPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        //rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 neutralSensorPos = neutralSensor.GetComponent<Transform>().position;
        minX = neutralSensorPos.x - tolerance;
        maxX = neutralSensorPos.x + tolerance;
        minZ = neutralSensorPos.z - tolerance;
        maxZ = neutralSensorPos.z + tolerance;
        //startPosOfJoystick = transform.TransformPoint(neutralSensor.GetComponent<Transform>().position);
        //minX = startPosOfJoystick.x - tolerance;
        //maxX = startPosOfJoystick.x + tolerance;

        //minZ = startPosOfJoystick.z - tolerance;
        //maxZ = startPosOfJoystick.z + tolerance;
        this.GetComponent<Transform>().position = this.GetComponent<Rigidbody>().position;
        CalculateMovement();
        TranslateToFollowCraft(posToStart);
        currentPos = transform.TransformPoint(transform.position);
        Vector3 tempPos = transform.TransformPoint(transform.position);
        if (currentPos.y > maxY)
        {
            tempPos = new Vector3(tempPos.x, maxY, tempPos.z);
            
            //Debug.Log ("Hit Max Y");
        }
        else if (currentPos.y < minY)
        {
            tempPos = new Vector3(tempPos.x, minY, tempPos.z);
            //Debug.Log ("Hit Min Y");
        }

        if (currentPos.z > maxZ)
        {
            tempPos = new Vector3(tempPos.x, tempPos.y, maxZ);
            //Debug.Log ("Hit Max Z");
        }
        else if (currentPos.z < minZ)
        {
            tempPos = new Vector3(tempPos.x, tempPos.y, minZ);
            //Debug.Log ("Hit Min Z");
        }

        if (currentPos.x > maxX)
        {
            tempPos = new Vector3(maxX, tempPos.y, tempPos.z);
            //Debug.Log ("Hit Max X");
        }
        else if (currentPos.x < minX)
        {
            tempPos = new Vector3(minX, tempPos.y, tempPos.z);
            //Debug.Log ("Hit Min X"); 
        }

        //Debug.Log (minX);
        //Debug.Log (minZ);
        transform.TransformPoint(tempPos);
        transform.position = tempPos;
        
        //posToStart = new Vector3 (transform.position.x - startPosOfJoystick.x, transform.position.y - startPosOfJoystick.y, transform.position.z - startPosOfJoystick.z);

        //Debug.Log("Controller 1 " + controller1Script.isGrabbing);
        //Debug.Log("Controller 2 " + controller2Script.isGrabbing);

        if(!isGrabbed)
        {

            Vector3 tempPos2 = neutralSensor.GetComponent<Transform>().position;
            tempPos = new Vector3(tempPos2.x, tempPos2.y, tempPos2.z);
            transform.TransformPoint(tempPos);
            transform.position = tempPos;
        }




        //Debug.Log ("joystick" + startX);
    }

    public Vector3 GetPosition()
    {
        return currentPos;
    }

    void CalculateMovement()
    {
        //moveX = (startPosOfJoystick.x - transform.position.x) * -1;
        ////Debug.Log ("Move X Send " + moveX);
        //moveZ = (startPosOfJoystick.z - transform.position.z) * -1;
        ////Debug.Log ("Move Z Send " + moveZ);

        closestSensor = neutralSensor;



        float distFront = Vector3.Distance(transform.TransformPoint(frontSensor.GetComponent<Transform>().position), (transform.TransformPoint(this.GetComponent<Transform>().position)));
        float distLeft = Vector3.Distance(transform.TransformPoint(leftSensor.GetComponent<Transform>().position), (transform.TransformPoint(this.GetComponent<Transform>().position)));
        float distRight = Vector3.Distance(transform.TransformPoint(rightSensor.GetComponent<Transform>().position), (transform.TransformPoint(this.GetComponent<Transform>().position)));
        float distRear = Vector3.Distance(transform.TransformPoint(rearSensor.GetComponent<Transform>().position), (transform.TransformPoint(this.GetComponent<Transform>().position)));
        float distNeutral = Vector3.Distance(transform.TransformPoint(neutralSensor.GetComponent<Transform>().position), (transform.TransformPoint(this.GetComponent<Transform>().position)));

        //Debug.Log("Transform " + neutralSensor.GetComponent<Transform>().position);
        //Debug.Log("TransformPoint " + transform.TransformPoint(neutralSensor.GetComponent<Transform>().position));
        Debug.Log("front" + distFront);
        Debug.Log("rear" + distRear);

        if (distNeutral > joystickNullZone)
        {
            if (distFront < distLeft && distFront < distRight && distFront < distRear)
            {
                closestSensor = frontSensor;
                directionToGo = Direction.forwards;
            }
            else if (distLeft < distFront && distLeft < distRight && distLeft < distRear)
            {
                closestSensor = leftSensor;
                directionToGo = Direction.left;
            }
            else if (distRight < distFront && distRight < distLeft && distRight < distRear)
            {
                closestSensor = rightSensor;
                directionToGo = Direction.right;
            }
            else if (distRear < distFront && distRear < distLeft && distRear < distRight)
            {
                closestSensor = rearSensor;
                directionToGo = Direction.backwards;
            }
        }
        else
        {
            closestSensor = neutralSensor;
            directionToGo = Direction.stop;
        }

        //Debug.Log(directionToGo);





    }

    void TranslateToFollowCraft(Vector3 posToStart)
    {
        //if (moveX >= maxSpeed)
        //	moveX = maxSpeed;
        //else if (moveX <= maxSpeed * -1)
        //	moveX = maxSpeed * -1;

        //if (moveZ >= maxSpeed)
        //	moveZ = maxSpeed;
        //else if (moveZ <= maxSpeed * -1)
        //	moveZ = maxSpeed * -1;

        ////subCraftPosition = theSubCraft.transform.position;
        ////Transform joystick along with craft, giving new centre point and position of stick
        //Vector3 tempVec3 = new Vector3 (startPosOfJoystick.x + moveX, startPosOfJoystick.y, startPosOfJoystick.z + moveZ);
        //startPosOfJoystick = tempVec3;
        //tempVec3 = transform.position;
        ////Debug.Log (tempVec3);
        //transform.position = new Vector3 (transform.position.x + moveX, transform.position.y, transform.position.z + moveZ);
        //Debug.Log ("Hello World!");
        //maxX += moveX;
        //minX += moveX;
        //maxZ += moveZ;
        //minZ += moveZ;
    }






}

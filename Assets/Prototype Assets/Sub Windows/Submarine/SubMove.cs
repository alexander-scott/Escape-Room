using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using NetworkLib;
using System;

namespace Assets.Prototype_Assets
{
	public class SubMove : NetworkBehaviour
	{
        public float maximumMovementSpeed = 5f;
        public float maximumRotationSpeed = 5f;
        public float divingSpeed = 3f;

        public float movementAcceleration = 0.5f;
        public float rotationAcceleration = 2f;

        private float currentMovementSpeed = 0f;
        private float currentRotationSpeed = 0f;

        private bool forward = false;
        private bool backward = false;
        private bool left = false;
        private bool right = false;

		void Start()
		{
			// Starts a server to listen for movement commands if this client is the host
			if (isServer)
			{
				// Starts a UDP Server from the Network Lib
				NetworkLib.Server.start(LibProtocolType.UDP);

				// Create an Observer to listen for the various pack types that are used to move the sub
				Server.ServerPacketObserver.AddObserver((int)PacketType.MOVE, MoveSub);
                Server.ServerPacketObserver.AddObserver((int)PacketType.ENDMOVE, EndMoveSub);
            }
		}

        void OnApplicationQuit()
		{
			NetworkLib.Server.stop();
		}

		void Update()
		{
			float y = 0.0f;

			if (Input.GetKey("m")) // Dive to submerge
            {
				y = 1.0f * Time.deltaTime * divingSpeed;
			}
			else if (Input.GetKey("n")) // Rise back to surface
            {
				y = -1.0f * Time.deltaTime * divingSpeed;
			}

            Vector3 movementVector = transform.right;

            // Sub movement - Depending on the button pressed move in a specific direction. If both are pressed do not move
            if (forward && !backward)
            {
                if (currentMovementSpeed < maximumMovementSpeed)
                {
                    currentMovementSpeed += movementAcceleration * Time.deltaTime;
                }
            }
            else if (backward && !forward)
            {
                if (currentMovementSpeed > -maximumMovementSpeed)
                {
                    currentMovementSpeed -= movementAcceleration * Time.deltaTime;
                }
            }
            else
            {
                if (currentMovementSpeed > 0f)
                {
                    currentMovementSpeed -= movementAcceleration * Time.deltaTime;
                }
                else
                {
                    currentMovementSpeed += movementAcceleration * Time.deltaTime;
                }
            }

            // Sub rotation - Same as above.
            if (left && !right)
            {
                if (currentRotationSpeed > -maximumRotationSpeed)
                {
                    currentRotationSpeed -= rotationAcceleration * Time.deltaTime;
                }
            }
            else if (right && !left)
            {
                if (currentRotationSpeed < maximumRotationSpeed)
                {
                    currentRotationSpeed += rotationAcceleration * Time.deltaTime;
                }
            }
            else
            {
                if (currentRotationSpeed > 0f)
                {
                    currentRotationSpeed -= rotationAcceleration * Time.deltaTime;
                }
                else
                {
                    currentRotationSpeed += rotationAcceleration * Time.deltaTime;
                }
            }

			transform.Rotate(0, Time.deltaTime * currentRotationSpeed, 0);
			transform.Translate(movementVector * Time.deltaTime * currentMovementSpeed, Space.World);
		}

		private void MoveSub(Packet p)
		{
			if ((string)p.generalData[0] == "Forward")
			{
                forward = true;
			}

			if ((string)p.generalData[0] == "Backward")
			{
                backward = true;
			}

            if ((string)p.generalData[0] == "Left")
            {
                left = true;
            }

            if ((string)p.generalData[0] == "Right")
            {
                right = true;
            }
        }

        private void EndMoveSub(Packet p)
        {
            if ((string)p.generalData[0] == "Forward")
            {
                forward = false;
            }

            if ((string)p.generalData[0] == "Backward")
            {
                backward = false;
            }

            if ((string)p.generalData[0] == "Left")
            {
                left = false;
            }

            if ((string)p.generalData[0] == "Right")
            {
                right = false;
            }
        }
    }
}

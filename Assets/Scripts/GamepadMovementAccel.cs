using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class GamepadMovementAccel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public Camera C;
    private float acceleration = 2.5f;
    private float speed = 0;
    private Vector3 inputVector;
    private Vector3 movementVector;
    private float movementSpeed = 5.0f;

    void Start()
    {
        //player = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        inputVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        inputVector.z = inputVector.y;
        inputVector.y = 0;
        if ((inputVector.x > 0.5 || inputVector.x < -0.5 && (inputVector.y > -0.5 && inputVector.z < 0.5)) || (inputVector.z > 0.5 || (inputVector.z < -0.5 && (inputVector.x > -0.5 && inputVector.x < 0.5))))
            movementVector += inputVector;
        if (movementVector.x > 5)
            movementVector.x = 5;
        if (movementVector.x < -5)
            movementVector.x = -5;
        if (movementVector.z > 5)
            movementVector.z = 5;
        if (movementVector.z < -5)
            movementVector.z = -5;
        //movementVector += inputVector
        else
        {
            if (movementVector.x < 1 && movementVector.x > -1) {
                if (movementVector.x > 0)
                    movementVector.x -= 0.2f;
                if (movementVector.x < 0)
                    movementVector.x += 0.2f;
            }
            if (movementVector.z < 1 && movementVector.z > -1)
                if (movementVector.z > 0)
                    movementVector.z -= 0.2f;
                if (movementVector.z < 0)
                    movementVector.z += 0.2f;
        }
        player.transform.rotation = C.transform.localRotation;
        player.transform.Translate(movementVector * movementSpeed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class GamepadMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    private float acceleration = 2.5f;
    private float speed = 0;
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
        movementVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);


	if(movementVector.x > 0){
	    if(speed + acceleration < 50)
		speed += acceleration;
	    else
		speed = 50;
	}
	else{
	    if(speed + acceleration > -50)
		speed += acceleration;
	    else{
		speed = -50
	    }
	}
	player.transform.Translate(movementVector * Time.deltaTime);
    }
}

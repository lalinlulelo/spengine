using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class GamepadMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
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
        movementVector.x = movementVector.x * movementSpeed;
        movementVector.y = movementVector.y * movementSpeed;

        player.transform.Translate(movementVector * Time.deltaTime);



        //player.transform.Translate(Vector3.right * movementVector.x * Time.deltaTime);

        /*
        if (Input.GetKey(KeyCode.UpArrow)) {
            player.transform.Translate(Vector3.forward * 2 * Time.deltaTime);
            Debug.Log("Going forward");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            player.transform.Translate(Vector3.forward * -2 * Time.deltaTime);
            Debug.Log("Going backward");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            player.transform.Translate(Vector3.right * 2 * Time.deltaTime);
            Debug.Log("Going right");
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            player.transform.Translate(Vector3.right * -2 * Time.deltaTime);
           Debug.Log("Going left");
        }
        */


    }
}

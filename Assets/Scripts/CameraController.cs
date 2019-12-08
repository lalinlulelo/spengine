using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float speed = 75.0f;
    public GameObject target;
    // Update is called once per frame
    void Update()
    {
        target.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * speed * Time.deltaTime);
        if (Input.GetKeyDown("space"))
        {
            target.transform.position = new Vector3(0, 0, target.transform.position.z);
        }
    }
}

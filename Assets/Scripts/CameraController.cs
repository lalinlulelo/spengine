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
        target.transform.Rotate(new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0) * speed * Time.deltaTime);
    }
}

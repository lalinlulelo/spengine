using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPointer : MonoBehaviour
{
    public GameObject target;
    public float distance;
    public float centreOffset;
    public MeshRenderer arrowRenderer;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.transform.position);
        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
        {
            arrowRenderer.enabled = false;
        } else
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
            transform.position += Camera.main.transform.up * centreOffset;
            transform.LookAt(target.transform);
            arrowRenderer.enabled = true;
        }
        
    }
}

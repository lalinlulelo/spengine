using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPointer : MonoBehaviour
{
    public GameObject target;
    public float distance;
    public float centreOffset;
    public MeshRenderer arrowRenderer;
    public GravityManager gravManager;
    private Transform tempTarget;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            tempTarget = gravManager.TargetBody;
        } else
        {
            tempTarget = target.transform;
        }
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(tempTarget.position);
        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
        {
            arrowRenderer.enabled = false;
        } else
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
            transform.position += Camera.main.transform.up * centreOffset;
            transform.LookAt(tempTarget);
            arrowRenderer.enabled = true;
        }
        
    }
}

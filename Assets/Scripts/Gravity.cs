using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public Rigidbody rb;
    public Gravity body;
    public float initialVelocity;
    public GravityManager gravManager;

    const float gravityConstant = 1;

    private Vector3 gravity;
    private Vector3 velocity;

    void OnEnable()
    {
        gravManager.AddBody(this, rb.mass);
    }

    void OnDisable()
    {
        gravManager.RemoveBody(this);
    }

    void Start()
    {
        rb.velocity = new Vector3(initialVelocity, 0, 0);
    }

    void FixedUpdate()
    {
        gravity = body.GetGravity(rb.worldCenterOfMass);
        velocity = rb.velocity;
        velocity += gravity * Time.fixedDeltaTime;
        rb.velocity = velocity;
    }

    public Vector3 GetGravity(Vector3 attract)
    {
        Vector2 delta = transform.position - attract;
        return delta.normalized * ((rb.mass * gravityConstant) / delta.sqrMagnitude);
    }
}

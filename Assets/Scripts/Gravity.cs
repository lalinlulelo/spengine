using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public Rigidbody rb;
    public Gravity Body;
    public Transform tf;
    public Vector3 initialVelocity;
    public GravityManager gravManager;

    private Vector3 gravity;
    public Vector3 Velocity { get; set; }
    public float SemiMajor { get; set; }

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
        //rb.velocity = new Vector3(initialVelocity, 0, 0);
        rb.velocity = initialVelocity;
        SemiMajor = 0;
    }

    void FixedUpdate()
    {
        if (Body == null)
        {
            return;
        }
        UpdateGravity(Body);
        if (!Body.Body.Equals(this))
        {
            UpdateGravity(Body.Body);
        }
    }

    private void UpdateGravity(Gravity target)
    {
        gravity = target.GetGravity(rb.worldCenterOfMass);
        Velocity = rb.velocity;
        Velocity += gravity * Time.fixedDeltaTime;
        rb.velocity = Velocity;
    }

    public Vector3 GetGravity(Vector3 attract)
    {
        Vector3 delta = transform.position - attract;
        return delta.normalized * ((rb.mass * Constants.gravityConstant) / delta.sqrMagnitude);
    }
}

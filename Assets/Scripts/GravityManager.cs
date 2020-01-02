using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    private static List<Gravity> bodies;
    private static bool needsUpdate = false;
    public static bool targetUpdate = false;
    public Transform LargestBody { get; private set; }
    public float gravityConstant = 1;
    public Transform TargetBody { get; private set; }

    private void SetLargest()
    {
        LargestBody = bodies.First().transform;
    }

    public void AddBody(Gravity body, float mass)
    {
        if (bodies == null)
        {
            bodies = new List<Gravity>();
        }
        bodies.Add(body);
        needsUpdate = true;
    }

    public void DestroyScene()
    {
        foreach (Gravity body in bodies)
        {
            Destroy(body);
        }
    }

    public void RemoveBody(Gravity body)
    {
        bodies.Remove(body);
        needsUpdate = true;
    }

    private void SortBodies()
    {
        bodies = bodies.OrderByDescending(key => key.rb.mass).ToList();
    }

    void FixedUpdate()
    {
        if (needsUpdate)
        {
            SortBodies();
            InitOrbits();
            CheckHillSpheres();
            SetLargest();
            needsUpdate = false;
        }
        if (targetUpdate)
        {
            FindTarget();
            targetUpdate = false;
        }
    }

    private void FindTarget()
    {
        bool foundBody = false;
        foreach (Gravity body in bodies)
        {
            if (body.IsTarget)
            {
                TargetBody = body.transform;
                foundBody = true;
                break;
            }
        }
        if (!foundBody)
        {
            TargetBody = LargestBody;
        }
    }

    private void InitOrbits()
    {
        Gravity star = bodies.First();
        foreach (Gravity body in bodies)
        {
            if (body.Equals(star))
            {
                if (bodies.Count < 2)
                {
                    Debug.LogError("At least 2 bodies are required to perform simulations");
                    return;
                }
                else
                {
                    body.Body = bodies.ElementAt(1);
                }
            }
            else
            {
                body.Body = star;
            }
            body.SemiMajor = CalculateSemiMajor(body, gravityConstant);
        }
    }

    private void CheckHillSpheres()
    {
        foreach (Gravity body1 in bodies)
        {
            if (body1.rb.mass < 0.05f)
            {
                body1.Body = bodies.First();
                body1.SemiMajor = CalculateSemiMajor(body1, gravityConstant);
            }
            else
            {
                foreach (Gravity body2 in bodies)
                {
                    if (!body1.Equals(body2))
                    {
                        if (body2.rb.mass < 0.05f)
                        {
                            //skip
                        }
                        else if (IsInHillSphere(body1, body2))
                        {
                            body1.Body = body2;
                            body1.SemiMajor = CalculateSemiMajor(body1, gravityConstant);
                        }
                    }
                }
            }
        }
    }

    public static bool IsInHillSphere(Gravity body1, Gravity body2)
    {
        float semiMajorAxis = body1.SemiMajor;
        float smallMass;
        float largeMass;
        largeMass = body1.rb.mass;
        smallMass = body2.rb.mass;
        
        float hillRadiusCubed = Mathf.Pow(semiMajorAxis, 3) * (smallMass / (3 * largeMass));
        float distance = Vector3.Distance(body1.transform.position, body2.transform.position);
        if (Mathf.Pow(distance, 3) < hillRadiusCubed)
        {
            return true;
        }
        return false;
    }

    public static float CalculateSemiMajor(Gravity body1, float gravityConstant)
    {
        Gravity body2 = body1.Body;
        float orbitDistance = Vector3.Distance(body1.transform.position, body2.transform.position);
        float semiMajor = body1.rb.velocity.sqrMagnitude / (2 * gravityConstant * body2.rb.mass) + 1 / orbitDistance;
        semiMajor = (1 / semiMajor) / 2;
        return semiMajor;
    }
}

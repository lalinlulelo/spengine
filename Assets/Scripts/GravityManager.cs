using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    private static Dictionary<Gravity, float> bodies;
    private static bool needsUpdate = false;

    public void AddBody(Gravity body, float mass)
    {
        if (bodies == null)
        {
            bodies = new Dictionary<Gravity, float>();
        }
        bodies.Add(body, mass);
        needsUpdate = true;
    }

    public void RemoveBody(Gravity body)
    {
        bodies.Remove(body);
        needsUpdate = true;
    }

    private void SortBodies()
    {
        foreach (var item in bodies.OrderByDescending(key => key.Value))
        {
        }
    }

    void FixedUpdate()
    {
        if (needsUpdate)
        {
            SortBodies();
        }
    }

}

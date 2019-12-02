using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    private struct CelestialState
    {
        public double[] u;
        public double[] pos1;
        public double[] pos2;
        public double mass1;
        public double mass2;
        public double massRatio;
        public double massSum;
        public float eccentricity;
    }
    private CelestialState celestialState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitState()
    {
        celestialState = new CelestialState
        {
            u = new double[] { 0, 0, 0, 0 },
            pos1 = new double[] { 0, 0 },
            pos2 = new double[] { 0, 0 },
            mass1 = 1,
            mass2 = 0,
            massRatio = celestialState.mass2 / celestialState.mass1,
            massSum = celestialState.mass1 + celestialState.mass2,
            eccentricity = 0
        };

    }

    /**
     * Calculates derivatives of the system of ODEs that describe the equation of motion of two bodies
     * returns array of [xpos, ypos, xvelocity, yvelocity]
     **/
    private double[] Derivative()
    {
        double[] du = new double[celestialState.u.Length];
        //Get x and y coordinates
        double[] r = new double[2];
        Array.Copy(celestialState.u, r, r.Length);
        //Get distance between bodies
        double rr = Math.Sqrt(Math.Pow(r[0], 2) + Math.Pow(r[1], 2));

        for (int i=0; i<2; i++)
        {
            du[i] = celestialState.u[i + 2];
            du[i + 2] = -(1 + celestialState.massRatio) * r[i] / Math.Pow(rr, 3);
        }
        return du;
    }

    private void RungeKutta(float timeStep, double[] u)
    {
        float[] a = new float[] { timeStep / 2, timeStep / 2, timeStep, 0 };
        float[] b = new float[] { timeStep / 6, timeStep / 3, timeStep / 3, timeStep / 6 };
//celestialState
    }
}

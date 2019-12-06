using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialManager : MonoBehaviour
{
    public GameObject basePlanet;
    public GameObject baseStar;
    public GravityManager gravityManager;
    public Material[] materials;

    enum BodyType {Star, Planet};

    // Start is called before the first frame update
    void Start()
    {
        //Sun
        GenerateBody(BodyType.Star, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 100, new Vector3(2, 2, 2), materials[3]);
        //Earth
        GenerateBody(BodyType.Planet, new Vector3(0, 10, 0), new Vector3(-0.75f, 0, 0), 1, new Vector3(1, 1, 1));
        //Moon
        GenerateBody(BodyType.Planet, new Vector3(1.5f, 10, 0), new Vector3(-0.75f, -0.125f, 0), 0.1f, new Vector3(0.25f, 0.25f, 0.25f), materials[2]);
        //Mars
        GenerateBody(BodyType.Planet, new Vector3(0, 20, 0), new Vector3(0.5f, 0, 0), 1.5f, new Vector3(1, 1, 1), materials[1]);
    }

    private void GenerateBody(BodyType type, Vector3 position, Vector3 initialVelocity, float mass, Vector3 scale, Material material = null)
    {
        GameObject basePrefab;
        switch (type)
        {
            case BodyType.Star:
                basePrefab = baseStar;
                break;
            default:
                basePrefab = basePlanet;
                break;
        }
        GameObject newPrefab = Instantiate(basePrefab, position, Quaternion.identity);
        newPrefab.transform.localScale = scale;
        Gravity body = newPrefab.GetComponent<Gravity>();
        body.Velocity = initialVelocity;
        body.gravManager = gravityManager;
        body.initialVelocity = initialVelocity;
        Rigidbody rb = newPrefab.GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.velocity = initialVelocity;
        if (material != null)
        {
            MeshRenderer renderer = newPrefab.GetComponent<MeshRenderer>();
            renderer.material = material;
        }
        newPrefab.SetActive(true);
    }
}

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
        GenerateBody(BodyType.Star, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 100, new Vector3(5, 5, 5), materials[9]);
        //Mercury
        GenerateBody(BodyType.Planet, new Vector3(0, 5, 0), new Vector3(-1, 0, 0), 0.15f, new Vector3(0.3f, 0.3f, 0.3f), materials[0]);
        //Venus
        GenerateBody(BodyType.Planet, new Vector3(0, 10, 0), new Vector3(0.75f, 0, 0), 0.6f, new Vector3(0.6f, 0.6f, 0.6f), materials[1]);
        //Earth
        GenerateBody(BodyType.Planet, new Vector3(0, 15, 0), new Vector3(0.6f, 0, 0), 1, new Vector3(1, 1, 1), materials[2]);
        //Moon
        GenerateBody(BodyType.Planet, new Vector3(1.5f, 15, 0), new Vector3(0.6f, -0.15f, 0), 0.05f, new Vector3(0.25f, 0.25f, 0.25f), materials[0]);
        //Mars
        GenerateBody(BodyType.Planet, new Vector3(0, 20, 0), new Vector3(0.5f, 0, 0), 1, new Vector3(1, 1, 1), materials[3]);
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

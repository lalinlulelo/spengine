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
        //GenerateSetScene();
        GenerateRandomScene(200, 200, 300);
        GravityManager.targetUpdate = true;
    }

    private void GenerateSetScene()
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
        GenerateBody(BodyType.Planet, new Vector3(1.5f, 15, 0), new Vector3(0.6f, -0.155f, 0), 0.05f, new Vector3(0.25f, 0.25f, 0.25f), materials[0]);
        //Mars
        GenerateBody(BodyType.Planet, new Vector3(0, 20, 0), new Vector3(-0.5f, 0, 0), 0.9f, new Vector3(1, 1, 1), materials[3]);
        //Asteroids
        //GenerateBody(BodyType.Planet, new Vector3(0, 26, 0), new Vector3(0.3f, 0, 0), 0.1f, new Vector3(1, 1, 1), materials[17]);
        GenerateAsteroidBelt(26, 1, 2, 0.45f, 200, 0.04f, new Vector3(0.2f, 0.2f, 0.2f), 2);
        //Jupiter
        GenerateBody(BodyType.Planet, new Vector3(0, 40, 0), new Vector3(0.2f, 0, 0), 20, new Vector3(3, 3, 3), materials[4]);
        //Moons
        GenerateBody(BodyType.Planet, new Vector3(2.5f, 40, 0), new Vector3(0.2f, -0.65f, 0), 0.2f, new Vector3(0.2f, 0.2f, 0.2f), materials[11]);
        GenerateBody(BodyType.Planet, new Vector3(-3f, 40, 0), new Vector3(0.2f, -0.6f, 0), 0.2f, new Vector3(0.2f, 0.2f, 0.2f), materials[12]);
        GenerateBody(BodyType.Planet, new Vector3(3.5f, 40, 0), new Vector3(0.2f, 0.55f, 0), 0.2f, new Vector3(0.2f, 0.2f, 0.2f), materials[13]);
        GenerateBody(BodyType.Planet, new Vector3(-4f, 40, 0), new Vector3(0.2f, -0.5f, 0), 0.2f, new Vector3(0.2f, 0.2f, 0.2f), materials[14]);
        //Saturn
        GenerateBody(BodyType.Planet, new Vector3(0, 65, 0), new Vector3(-0.15f, 0, 0), 15, new Vector3(2.5f, 2.5f, 2.5f), materials[5]);
        //Moons
        GenerateBody(BodyType.Planet, new Vector3(4f, 65, 0), new Vector3(-0.15f, -0.4f, 0), 0.12f, new Vector3(0.3f, 0.3f, 0.3f), materials[15]);
        GenerateBody(BodyType.Planet, new Vector3(-5f, 65, 0), new Vector3(-0.15f, 0.3f, 0), 0.18f, new Vector3(0.45f, 0.45f, 0.45f), materials[16]);
        //Uranus
        GenerateBody(BodyType.Planet, new Vector3(0, -80, 0), new Vector3(-0.1f, 0, 0), 8, new Vector3(1.5f, 1.5f, 1.5f), materials[6]);
        //Neptune
        GenerateBody(BodyType.Planet, new Vector3(90, 0, 0), new Vector3(0, -0.08f, 0), 10, new Vector3(2, 2, 2), materials[7]);
        //Pluto
        GenerateBody(BodyType.Planet, new Vector3(-105, 0, 0), new Vector3(0, 0.06f, 0), 0.2f, new Vector3(0.3f, 0.3f, 0.3f), materials[8], true);
        //Charon
        GenerateBody(BodyType.Planet, new Vector3(-105, 1, 0), new Vector3(0.1f, 0.06f, 0), 0.18f, new Vector3(0.25f, 0.25f, 0.25f), materials[10]);
    }

    private void GenerateRandomScene(float radius, float bodies, float starMass)
    {
        //Make a star
        GenerateBody(BodyType.Star, new Vector3(0, 0, 0), new Vector3(0, 0, 0), starMass, new Vector3(5, 5, 5), materials[9]);
        //Generate remaining bodies randomly
        int chosenType;
        int materialChoice;
        float scale;
        float chosenMass;
        for (int i=0; i<bodies; i++)
        {
            chosenType = Random.Range(0, 100);
            if (chosenType == 0)
            {
                chosenMass = RandomDouble(0.005f, 0.1f);
                scale = (chosenMass / 0.1f) * 0.5f;
                GenerateAsteroidBelt(RandomDouble(5, radius), RandomDouble(1, 5), RandomDouble(1, 3),
                    RandomDouble(0.1f, 1), Random.Range(30, 200), chosenMass, new Vector3(scale, scale, scale), Random.Range(1, 4));
            }
            else
            {
                chosenMass = RandomDouble(0.1f, starMass / 4);
                scale = chosenMass / (starMass / 4) * 3;
                materialChoice = Random.Range(0, materials.Length);
                GenerateBody(BodyType.Planet, RandomVector3(-radius, radius), RandomVector3(-1, 1), chosenMass, new Vector3(scale, scale, scale), materials[materialChoice]);
            }
        }
    }

    private float RandomDouble(float min, float max)
    {
        return Random.Range(min, max);
    }

    private Vector3 RandomVector3(float min, float max)
    {
        return new Vector3(RandomDouble(min, max), RandomDouble(min, max), RandomDouble(min, max));
    }

    private void GenerateAsteroidBelt(float radius, float radiusVariance, float angleVariance, float initialVelocity, float maxAsteroids, float mass, Vector3 scale, int layers)
    {
        System.Random random = new System.Random();
        float angle = 0;
        float angleChange = 360 / (maxAsteroids / layers);
        float radiusChoice;
        float angleChoice;
        int materialOffset = 17;
        int materialChoice;
        Vector3 position;
        Vector3 initVelocity;
        for (int i=0; i<maxAsteroids; i++)
        {
            radiusChoice = (float)random.NextDouble() * ((radius + radiusVariance / 2) - (radius - radiusVariance / 2)) + (radius - radiusVariance / 2);
            angleChoice = (float)random.NextDouble() * ((angle + angleVariance / 2) - (angle - angleVariance / 2)) + (angle - angleVariance / 2);
            position = new Vector3(radiusChoice * Mathf.Cos(Mathf.Deg2Rad * angleChoice), radiusChoice * Mathf.Sin(Mathf.Deg2Rad * angleChoice), 0);
            initVelocity = new Vector3(initialVelocity * Mathf.Cos(Mathf.Deg2Rad * (angleChoice+90)), initialVelocity * Mathf.Sin(Mathf.Deg2Rad * (angleChoice+90)), 0);
            materialChoice = random.Next(2) + materialOffset;
            GenerateBody(BodyType.Planet, position, initVelocity, mass, scale, materials[materialChoice]);
            angle += angleChange;
        }
    }

    private void GenerateBody(BodyType type, Vector3 position, Vector3 initialVelocity, float mass, Vector3 scale, Material material = null, bool isTarget = false)
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
        newPrefab.transform.rotation *= Quaternion.Euler(90, 0, 0);
        newPrefab.transform.localScale = scale;
        Gravity body = newPrefab.GetComponent<Gravity>();
        body.Velocity = initialVelocity;
        body.gravManager = gravityManager;
        body.initialVelocity = initialVelocity;
        body.IsTarget = isTarget;
        if (isTarget)
        {
            GravityManager.targetUpdate = true;
        }
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

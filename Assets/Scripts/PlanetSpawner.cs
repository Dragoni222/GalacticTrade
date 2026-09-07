using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{

    public int numOfPlanets;
    public GameObject planetPrefab;
    void Start()
    {
        for (int i = 0; i < numOfPlanets; i++)
        {
            Vector3 planetStartingPosition = new Vector3(Random.Range(2, 15), 0, Random.Range(2, 15));
            Instantiate(planetPrefab, planetStartingPosition, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        
    }
}

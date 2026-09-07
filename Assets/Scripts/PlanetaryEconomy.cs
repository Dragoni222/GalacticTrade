using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlanetaryEconomy : MonoBehaviour
{

    public SystemEconomy systemEconomy;
    public GameObject starshipPrefab;
    public int planetID;

    public PlanetaryEconomy TEMPtargetPlanet;


    void Start()
    {
        planetID = systemEconomy.InsertPlanet(0, gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (systemEconomy.time % 500 == 0)
        {
            int shipID = systemEconomy.InsertShip(planetID);
            SendStarship(shipID, new List<int>(), TEMPtargetPlanet.planetID);
        }
    }

    void SendStarship(int shipID, List<int> resourceIDs, int targetPlanetID)
    {
        GameObject ship = Instantiate(starshipPrefab);
        ship.transform.position = transform.position + new Vector3(0.5f, 0, 0.5f);
        Starship starshipScript = ship.GetComponent<Starship>();
        starshipScript.shipID = shipID;
        starshipScript.target = systemEconomy.GetPlanetFromID(targetPlanetID).transform;
        starshipScript.star = systemEconomy.transform;

        foreach (int id in resourceIDs)
        {
            systemEconomy.MoveResourceToShip(id, shipID);
        }

        systemEconomy.UndockShip(shipID, targetPlanetID);


    }
}




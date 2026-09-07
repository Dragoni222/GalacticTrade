using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using System.Numerics;
public class SystemEconomy : MonoBehaviour
{
    // Start is called before the first frame update

    private SQLiteConnection _db;
    public int time = 0;
    private Dictionary<int, GameObject> _planetIDs = new Dictionary<int, GameObject>();
    void Awake()
    {
        _db = new SQLiteConnection($"{Application.persistentDataPath}/SystemEconomyDB.db3", SQLiteOpenFlags.Create |
                                         SQLiteOpenFlags.FullMutex |
                                         SQLiteOpenFlags.ReadWrite);
        _db.CreateTable<Planet>();
        _db.CreateTable<Ship>();
        _db.CreateTable<Resource>();
        _db.CreateTable<ProductionSite>();
        _db.CreateTable<PlanetHasResource>();
        _db.CreateTable<ShipHasResource>();
    }

    public int InsertPlanet(int victoryPoints, GameObject planetGameObject)
    {
        Planet planet = new Planet { VictoryPoints = victoryPoints };
        _db.Insert(planet);
        _planetIDs.Add(planet.PlanetID, planetGameObject);

        return planet.PlanetID;
    }

    public int InsertShip(int planetID)
    {
        Ship ship = new Ship { PlanetID = planetID, DestinationID = planetID, Docked = true, TimeOfDeparture = time };
        _db.Insert(ship);
        return ship.ShipID;
    }

    public int InsertResource(string resourceType)
    {
        Resource resource = new Resource { ResourceType = resourceType };
        _db.Insert(resource);
        return resource.ResourceID;
    }

    public int InsertResourceOnPlanet(string resourceType, int planetID)
    {
        int resourceID = InsertResource(resourceType);
        MoveResourceToPlanet(resourceID, planetID);
        return resourceID;
    }

    public int InsertResourceOnShip(string resourceType, int shipID)
    {
        int resourceID = InsertResource(resourceType);
        MoveResourceToShip(resourceID, shipID);
        return resourceID;
    }

    public int InsertProductionSite(int planetID, float productivity, string resourceType)
    {
        ProductionSite productionSite = new ProductionSite { PlanetID = planetID, Productivity = productivity, ResourceType = resourceType };
        _db.Insert(productionSite);
        return productionSite.SiteID;
    }

    public void MoveResourceToShip(int resourceID, int shipID)
    {
        var planetHasResourceRecord = _db.Table<PlanetHasResource>().Where(p => p.ResourceID == resourceID).ToList();
        if (planetHasResourceRecord.Count > 0)
        {
            _db.Delete<PlanetHasResource>(resourceID);
        }


        ShipHasResource shipHasResource = new ShipHasResource { ResourceID = resourceID, ShipID = shipID };
        _db.Insert(shipHasResource);
    }

    public void MoveResourceToPlanet(int resourceID, int planetID)
    {
        var shipHasResourceRecord = _db.Table<ShipHasResource>().Where(p => p.ResourceID == resourceID).ToList();
        if (shipHasResourceRecord.Count > 0)
        {
            _db.Delete<ShipHasResource>(resourceID);
        }


        PlanetHasResource planetHasResource = new PlanetHasResource { ResourceID = resourceID, PlanetID = planetID };
        _db.Insert(planetHasResource);
    }

    public void UndockShip(int shipID, int destinationID)
    {
        string query = $"UPDATE Ships SET Docked = false, DestinationID = {destinationID}, TimeOfDeparture = {time} WHERE ShipID = {shipID}";
        _db.Query<Ship>(query);
    }

    public GameObject GetPlanetFromID(int planetID)
    {
        return _planetIDs[planetID];
    }

    private void OnApplicationQuit()
    {
        _db.Close();
    }

    private void FixedUpdate()
    {
        time += 1;
    }


}





[Table("Planets")]
public class Planet
{
    [PrimaryKey, AutoIncrement]
    [Column("PlanetID")]
    public int PlanetID { get; set; }
    [Column("VictoryPoints")]
    public int VictoryPoints { get; set; }

}
[Table("Ships")]
public class Ship
{
    [PrimaryKey, AutoIncrement]
    [Column("ShipID")]
    public int ShipID { get; set; }
    [Column("PlanetID")]
    public int PlanetID { get; set; }
    [Column("DestinationID")]
    public int DestinationID { get; set; }
    [Column("TimeOfDeparture")]
    public int TimeOfDeparture { get; set; }
    [Column("Docked")]
    public bool Docked { get; set; }
}
[Table("Resources")]
public class Resource
{
    [PrimaryKey, AutoIncrement]
    [Column("ResourceID")]
    public int ResourceID { get; set; }
    [Column("ResourceType")]
    public string ResourceType { get; set; }

}

[Table("ProductionSites")]
public class ProductionSite
{
    [PrimaryKey, AutoIncrement]
    [Column("SiteID")]
    public int SiteID { get; set; }

    [Column("PlanetID")]
    public int PlanetID { get; set; }

    [Column("Productivity")]
    public float Productivity { get; set; }

    [Column("ResourceType")]
    public string ResourceType { get; set; }

}

[Table("PlanetHasResource")]
public class PlanetHasResource
{
    [PrimaryKey]
    [Column("ResourceID")]
    public int ResourceID { get; set; }

    [Column("PlanetID")]
    public int PlanetID { get; set; }


}

[Table("ShipHasResource")]
public class ShipHasResource
{
    [PrimaryKey]
    [Column("ResourceID")]
    public int ResourceID { get; set; }

    [Column("ShipID")]
    public int ShipID { get; set; }


}


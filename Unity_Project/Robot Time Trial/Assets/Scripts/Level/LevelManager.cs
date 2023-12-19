using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject m_BatteryPrefab;
    GameObject[] m_BatteryLocations;


    // Start is called before the first frame update
    void Start()
    {
        // This should save a reference to all consumable locations, so they can be respawned on failure

        m_BatteryLocations = GameObject.FindGameObjectsWithTag("BatterySpawn");

        SpawnBatteries();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        IRespawn[] respawns = FindObjectsOfType<MonoBehaviour>(true).OfType<IRespawn>().ToArray();

        foreach (var respawn in respawns)
        {
            respawn.Respawn();
        }

        SpawnBatteries();
    }

    public void SpawnBatteries()
    {
        foreach(GameObject obj in m_BatteryLocations)
        {
            BatterySpawnIndicator indicator = obj.GetComponent<BatterySpawnIndicator>();

            if (indicator != null)
            {
                indicator.SpawnBattery(m_BatteryPrefab);
            }
        }
    }
}

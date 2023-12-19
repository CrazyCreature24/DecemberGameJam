using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float m_LevelTime = 30.0f;
    public float m_CurrentTime = 30.0f;

    public GameObject m_BatteryPrefab;
    GameObject[] m_BatteryLocations;


    // Start is called before the first frame update
    void Start()
    {
        // This should save a reference to all consumable locations, so they can be respawned on failure

        m_BatteryLocations = GameObject.FindGameObjectsWithTag("BatterySpawn");

        SpawnBatteries();

        m_CurrentTime = m_LevelTime;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleLevelTimer();
    }

    public void Respawn()
    {
        // This calls respawn on all object with the interface. This helps centralize everything into one place. Player Movement shows an example.
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

    public void HandleLevelTimer()
    {
        if (m_CurrentTime > 0.0f)
        {
            m_CurrentTime -= Time.fixedDeltaTime;
        }
        else if (m_CurrentTime <= 0.0f)
        {
            m_CurrentTime = 0;
        }

    }

    public void ResetTime()
    {
        m_CurrentTime = m_LevelTime;
    }
}

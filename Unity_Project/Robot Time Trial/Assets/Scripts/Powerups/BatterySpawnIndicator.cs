using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySpawnIndicator : MonoBehaviour
{
    bool m_IsActivePowerup = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBattery(GameObject prefab)
    {
        if (m_IsActivePowerup == false)
        {
            GameObject newGO = PoolManager.Get(prefab);
            newGO.transform.position = gameObject.transform.position;
            newGO.GetComponent<BatteryPowerup>().m_SpawnIndicator = this;
            m_IsActivePowerup = true;
        }
    }


    public bool IsActivePowerup() 
    { 
        return m_IsActivePowerup; 
    }

    public void SetIsActivePowerup(bool val)
    {
        m_IsActivePowerup = val;
    }

}

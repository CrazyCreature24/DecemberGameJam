using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    public bool CleanUpStaleObjects = false;
    public int SecondsStaleBeforeCleanup = 500;
    void Start()
    {
        //m_SecondsPoolRefreshRate = 15;

        if (CleanUpStaleObjects)
            StartCoroutine(CleanPools());
    }

    private static ObjectPool CreateNewPool(GameObject prefab)
    {
        ObjectPool pool = new ObjectPool();
        pool.PoolType = typeof(GameObject);
        pool.ObjectName = prefab.name;
        m_Pools.Add(pool);
        return pool;
    }

    public static ObjectPool GetPool(string name, Type type)
    {
        var matchingPools = m_Pools.Where(x => x.ObjectName == name && x.PoolType == type);
        if (matchingPools.Any())
        {
            return matchingPools.First();
        }
        return null;
    }

    public static GameObject Get(GameObject prefab)
    {
        ObjectPool objPool = GetPool(prefab.name, typeof(GameObject));
        if (objPool == null)
        {
            objPool = CreateNewPool(prefab);
        }
        GameObject newObject = objPool.GetObject(prefab);
        newObject.SetActive(true);
        return newObject;
    }

    public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        GameObject newObject = Get(prefab);
        newObject.transform.position = pos;
        newObject.transform.rotation = rot;
        return newObject;
    }

    public static GameObject ObjectCreator(GameObject prefab)
    {
        GameObject newObject = (GameObject)Instantiate(prefab);
        return newObject;
    }


    public static IEnumerator CleanPools()
    {
        while (true)
        {
            List<ObjectPool> poolsToDelete = new List<ObjectPool>();

            foreach (ObjectPool pool in m_Pools)
            {
                if (pool.lastUsed + 0.000000001f < Time.timeSinceLevelLoad)
                {
                    pool.RemoveDisabledObjects();
                    if (pool.ObjectCount < 1)
                    {
                        poolsToDelete.Add(pool);
                    }
                }
            }
            poolsToDelete.ForEach(x => m_Pools.Remove(x));

            yield return new WaitForSeconds(0.00001f);
        }
    }

    //float m_SecondsPoolRefreshRate;
    static List<ObjectPool> m_Pools = new List<ObjectPool>();
}
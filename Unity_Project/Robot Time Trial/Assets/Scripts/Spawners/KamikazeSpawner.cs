using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeSpawner : MonoBehaviour, IRespawn
{
    [SerializeField]
    GameObject SpawnLocation;

    [Header("Spawner Stats")]
    public GameObject ObjectToSpawn;
    [SerializeField]
    [Tooltip("Check this to activate the spawner when the player gets in the range.")]
    bool ActivateWhenPlayerInRange = true;
    [SerializeField]
    float ActivationRange = 50.0f;
    [SerializeField]
    [Tooltip("Starting delay in seconds if Activate When Player In Range is true.")]
    float StartDelayAfterActivation = 0.0f;
    [SerializeField]
    [Tooltip("Starting delay in seconds if Activate When Player In Range is false.")]
    float StartDelay = 0.0f;
    [SerializeField]
    [Tooltip("Total number of Objects to spawn.")]
    int NumOfObjects = 2;
    [SerializeField]
    [Tooltip("Delay in seconds between each spawn.")]
    float DelayBetweenSpawn = 7.0f;



    float ElapsedTime = 0.0f;
    bool IsActivated = false;
    bool StartTimerReached = false;
    SphereCollider sphereCollider = null;
    int numSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = ActivationRange;
    }

    // Update is called once per frame
    void Update()
    {
        if(ActivateWhenPlayerInRange)
            UpdateAfterActivation();
        else
            UpdateWithoutActivation();
    }

    private void UpdateAfterActivation()
    {
        if (numSpawned < NumOfObjects)
        {
            if (IsActivated)
            {
                ElapsedTime += Time.deltaTime;
                if (ElapsedTime >= StartDelayAfterActivation && !StartTimerReached)
                {
                    StartTimerReached = true;
                    ElapsedTime = 0.0f;
                    SpawnObject();
                }

                if (StartTimerReached)
                {
                    if (ElapsedTime >= DelayBetweenSpawn)
                    {
                        SpawnObject();
                        ElapsedTime = 0.0f;
                    }
                }
            }
        }
    }

    private void UpdateWithoutActivation()
    {
        if (numSpawned < NumOfObjects)
        {
            ElapsedTime += Time.deltaTime;
            if (ElapsedTime >= StartDelay && !StartTimerReached)
            {
                StartTimerReached = true;
                ElapsedTime = 0.0f;
                SpawnObject();
            }

            if(StartTimerReached)
            {
                if(ElapsedTime >= DelayBetweenSpawn)
                {
                    SpawnObject();
                    ElapsedTime = 0.0f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ActivateWhenPlayerInRange)
            return;
        if (other.gameObject.CompareTag("Player"))
            IsActivated = true;
    }

    private void SpawnObject()
    {
        GameObject obj = PoolManager.Get(ObjectToSpawn.gameObject, SpawnLocation.transform.position, SpawnLocation.transform.rotation);
        obj.GetComponent<KamikazeDrone>().Init();
        numSpawned++;
    }

    public void Respawn()
    {
        numSpawned = 0;
        ElapsedTime = 0.0f;
        IsActivated = false;
        StartTimerReached = false;
        ElapsedTime = 0.0f;
    }
}

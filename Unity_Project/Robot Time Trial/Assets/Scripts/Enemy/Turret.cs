using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject Projectile;
    public float FireRate = 10.0f;
    public float rotationSpeed = 1.0f;
    public float ActivationRange = 40.0f;

    float TimeAfterLastFire = 0.0f;
    GameObject Player;
    [SerializeField]
    GameObject Muzzle;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if (!Player)
            return;

        Vector3 playerPos = Player.transform.position;
        Vector3 directionTowardsPlayer = playerPos - transform.position;
        float distanceFromPlayer = directionTowardsPlayer.magnitude;

        if (distanceFromPlayer < ActivationRange)
        {
            Quaternion rot = Quaternion.LookRotation(directionTowardsPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);

            if(TimeAfterLastFire >= FireRate)
            {
                GameObject projectile = PoolManager.Get(Projectile.gameObject, Muzzle.transform.position, Quaternion.identity);
                Vector3 direction = Muzzle.transform.forward;
                if (projectile)
                {
                    projectile.GetComponent<TurretOrb>().Init(Muzzle.transform.position, direction);
                    TimeAfterLastFire = 0.0f;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        TimeAfterLastFire += Time.deltaTime;
    }
}

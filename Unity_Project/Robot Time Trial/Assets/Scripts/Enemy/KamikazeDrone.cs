using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KamikazeDrone : MonoBehaviour, IRespawn
{
    [Header("Stats")]
    [SerializeField]
    float moveSpeed = 5.0f;
    [SerializeField]
    float RotationSpeed = 4.0f;
    [SerializeField]
    float ExplosionTimer = 1.0f;
    [SerializeField]
    ParticleSystem ExplosionParticleEffect = null;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    float DamageRadius = 5.0f;
    [SerializeField]
    [Tooltip("Percentage of battery damage done to the player")]
    [Range(0.0f, 1.0f)]
    float DamagePercentage = 0.4f;

    private Rigidbody rigidBody;
    GameObject Player;

    enum State
    {
        Moving = 0,
        Exploding = 1,
        Spawning = 2,
    };
    State state = State.Spawning;
    float ElapsedTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        state = State.Spawning;
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player");
        ElapsedTime = 0.0f;
    }

    public void Init()
    {
        state = State.Spawning;
        ElapsedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Spawning)
        {
            if (HasLanded())
                state = State.Moving;
            return;
        }

        if (state == State.Moving)
        {
            Vector3 direction = Player.transform.position - transform.position;
            direction.Normalize();
            direction.y = 0.0f;
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
            rigidBody.velocity = transform.forward * moveSpeed;
        }

        if(state == State.Exploding)
        {
            rigidBody.velocity = Vector3.zero;
            ElapsedTime += Time.deltaTime;
            if(ElapsedTime >= ExplosionTimer)
            {
                Explosion();
            }
        }
    }

    bool HasLanded()
    {
        //BoxCollider collider = GetComponent<BoxCollider>();
        //Vector3 rayStart = collider.bounds.center;
        ////float rayDistance = GetComponent<CapsuleCollider>().height * 0.5f;
        //float rayDistance = collider.bounds.extents.y;

        ////int layerMask = ~LayerMask.GetMask("Player", "Ignore Raycast", "Enemy");
        //RaycastHit hit;
        ////Physics.SphereCast(rayStart, collider.bounds.extents.z, Vector3.down, out hit, rayDistance);
        //bool isHit = Physics.Raycast(rayStart, Vector3.down, out hit, rayDistance);
        //if (isHit)
        //{
        //    int flag = 1;
        //}
        //return isHit;

        Vector3 rayStart = transform.position;
        float rayDistance = (GetComponent<CapsuleCollider>().height * 0.5f) - GetComponent<CapsuleCollider>().radius + 0.01f;

        int layerMask = ~LayerMask.GetMask("Player", "Ignore Raycast", "Enemy");
        RaycastHit hit;
        bool isHit = Physics.SphereCast(rayStart, GetComponent<CapsuleCollider>().radius, Vector3.down, out hit, rayDistance, layerMask);

        return isHit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            state = State.Exploding;
        }
    }

    private void Explosion()
    {
        GameObject vfx = PoolManager.Get(ExplosionParticleEffect.gameObject, transform.position, transform.rotation);
        vfx.GetComponent<SimpleVFX>().Init();

        Collider[] colliders = Physics.OverlapSphere(transform.position, DamageRadius);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.CompareTag("Player"))
            {
                Vector3 directionTowardsPlayer = this.GetComponent<CapsuleCollider>().bounds.center - collider.gameObject.transform.position;
                float distanceFromPlayer = directionTowardsPlayer.magnitude;
                float inverseScale = distanceFromPlayer / DamageRadius;
                float damageScale = 1.0f - inverseScale;
                float damageToApply = DamagePercentage * damageScale;
                collider.gameObject.GetComponent<PlayerPowerManager>().TakePowerDamage(damageToApply);
                break;
            }
        }
        state = State.Spawning;
        gameObject.SetActive(false);
    }


    public void Respawn()
    {
        state = State.Spawning;
        ElapsedTime = 0.0f;
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KamikazeDrone : MonoBehaviour
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
                GameObject vfx = PoolManager.Get(ExplosionParticleEffect.gameObject, transform.position, transform.rotation);
                vfx.GetComponent<SimpleVFX>().Init();
                gameObject.SetActive(false);
            }
        }
    }

    bool HasLanded()
    {
        Vector3 rayStart = transform.position;
        float rayDistance = GetComponent<CapsuleCollider>().height * 0.5f;

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
}

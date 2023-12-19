
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroMagnet : MonoBehaviour
{

    GameObject Player;

    LineRenderer LR1;
    LineRenderer LR2;
    bool bIsAttacking = false;
    float MagnetForceScaling = 0.0f;

    [Header("Base setup")]
    [SerializeField]
    GameObject LRObject1;
    [SerializeField]
    GameObject LRObject2;

    [Header("Stats")]
    [Range(5.0f, 20.0f)]
    public float ActivationRange = 10.0f;
    [Range(1.0f, 20.0f)]
    public float MagnetForce = 5.0f;

    [Header("Lightning Effect 1")]
    [Range(0.01f, 5.0f)]
    public float LR1PointDistance = 0.5f;
    [Range(0.01f, 2.0f)]
    public float LR1Width = 0.5f;
    [Range(1, 30)]
    public int LR1Segments = 20;
    [SerializeField]
    Material LR1Material;

    [Header("Lightning Effect 2")]
    [Range(0.01f, 5.0f)]
    public float LR2PointDistance = 0.5f;
    [Range(0.01f, 2.0f)]
    public float LR2Width = 0.5f;
    [Range(1, 30)]
    public int LR2Segments = 20;
    [SerializeField]
    Material LR2Material;
    

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        LR1 = LRObject1.GetComponent<LineRenderer>();
        LR2 = LRObject2.GetComponent<LineRenderer>();
        LR1.material = LR1Material;
        LR2.material = LR2Material;
        this.GetComponent<SphereCollider>().radius = ActivationRange;
    }

    private void FixedUpdate()
    {
        Vector3 direction = Player.transform.position - this.transform.position;
        float distanceFromPlayer = direction.magnitude;

        if (bIsAttacking)
        {
            //
            // LR1
            //
            {
                //
                // Experimental
                //
                //LR1.positionCount = (int)(LR1Segments * MagnetForceScaling);
                //
                // Experimental end
                //
                LR1.positionCount = LR1Segments;
                Vector3 LR1Direction = direction / LR1Segments;
                for (int i = 0; i < LR1Segments; i++)
                {
                    Vector3 pos = this.transform.position + LR1Direction * i + Random.insideUnitSphere * LR1PointDistance;
                    LR1.SetPosition(i, pos);
                }
                LR1.startWidth = LR1Width;
                LR1.endWidth = LR1Width;
            }

            //
            // LR2
            //
            {
                //
                // Experimental
                //
                //LR2.positionCount = (int)(LR2Segments * MagnetForceScaling);
                //
                // Experimental end
                //
                LR2.positionCount = LR2Segments;
                Vector3 LR2Direction = direction / LR2Segments;
                for (int i = 0; i < LR2Segments; i++)
                {
                    Vector3 pos = this.transform.position + LR2Direction * i + Random.insideUnitSphere * LR2PointDistance;
                    LR2.SetPosition(i, pos);
                }
                LR2.startWidth = LR2Width;
                LR2.endWidth = LR2Width;
            }
        }
        else
        {
            LR1.positionCount = 0;
            LR2.positionCount = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(bIsAttacking)
        {
            Vector3 directionTowardsEnemy = this.transform.position - Player.transform.position;
            float distanceFromPlayer = directionTowardsEnemy.magnitude;
            directionTowardsEnemy.Normalize();
            // distperc = x/100 * activationrange;  x = distperc/activationrange * 100
            float inverseScale = distanceFromPlayer / ActivationRange;
            MagnetForceScaling = 1 - inverseScale;
            //Mathf.Clamp(MagnetForceScaling, 0.0f, 1.0f);
            float forceToApply = MagnetForce * MagnetForceScaling;
            Player.transform.position += directionTowardsEnemy * forceToApply * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            bIsAttacking = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            bIsAttacking = false;
    }
}

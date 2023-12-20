using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeDrone : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 5.0f;
    public float maxDistFromWall = 7.0f;
    public LayerMask doNotCollide;


    bool bSpawnComplete;
    private Rigidbody rigidBody;
    GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        bSpawnComplete = false;
        rigidBody = GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!bSpawnComplete)
        {
            if (HasLanded())
                bSpawnComplete = true;
            return;
        }

        Vector3 direction = Player.transform.position - transform.position;
        direction.Normalize();
        direction.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(direction);
        rigidBody.velocity = direction * moveSpeed;
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
}

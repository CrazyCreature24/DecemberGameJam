using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretOrb : MonoBehaviour
{
    public float LifeTime = 10.0f;
    public float Speed = 20.0f;

    Vector3 Velocity = Vector3.zero;
    float currentLifeTime;

    public void Init(Vector3 position, Vector3 velocity)
    {
        transform.position = position;
        Velocity = velocity;
        currentLifeTime = LifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Velocity * Speed * Time.deltaTime;
        currentLifeTime -= Time.deltaTime;

        if (currentLifeTime <= 0) 
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject)
        {
            this.gameObject.SetActive(false);
            Debug.Log("Collision");
        }
    }
}

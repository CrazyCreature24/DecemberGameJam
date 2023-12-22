using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretOrb : MonoBehaviour
{
    public float LifeTime = 10.0f;
    public float Speed = 20.0f;
    public float Damage = 0.1f;

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
            if(collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerPowerManager>().TakePowerDamage(Damage);
            }

            if (collision.gameObject.CompareTag("Wall"))
            {
                gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerPowerManager>().TakePowerDamage(Damage);
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerup : MonoBehaviour
{
    float m_TimeElapsed = 0;
    public bool m_CanMove = true;
    public float m_MoveSpeed = 0.01f;
    public float m_MoveTime = 1.0f;
    public float m_RotationSpeed = 50.0f;

    public ShieldSpawnIndicator m_SpawnIndicator;

    [Header("Shield Stats")]
    [SerializeField]
    float ShieldDuration = 4.0f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float DamageReduction = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        SineMovement();
    }

    void SineMovement()
    {
        if (m_CanMove)
        {
            m_TimeElapsed += Time.fixedDeltaTime;

            if (m_TimeElapsed >= m_MoveTime)
            {
                m_MoveSpeed *= -1.0f;
                m_TimeElapsed = 0.0f;
            }
            transform.Rotate(Vector3.up, m_RotationSpeed * Time.fixedDeltaTime);
            Vector3 newPos = transform.position;
            newPos += new Vector3(0, m_MoveSpeed, 0);
            transform.position = newPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerPowerManager>().ActivateShield(ShieldDuration, DamageReduction);
            gameObject.SetActive(false);

            m_SpawnIndicator.SetIsActivePowerup(false);
            m_SpawnIndicator = null;
        }
    }
}

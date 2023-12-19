using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPowerup : MonoBehaviour
{
    float m_TimeElapsed = 0;

    public bool m_CanMove = true;
    public float m_MoveSpeed = 0.01f;
    public float m_MoveTime = 1.0f;

    public float m_PickupPowerIncrease = 0.3f;

    public BatterySpawnIndicator m_SpawnIndicator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SineMovement();
    }

    public void SineMovement()
    {
        if (m_CanMove)
        {
            m_TimeElapsed += Time.fixedDeltaTime;

            if (m_TimeElapsed >= m_MoveTime)
            {
                m_MoveSpeed *= -1.0f;
                m_TimeElapsed = 0.0f;
            }

            Vector3 newPos = transform.position;
            newPos += new Vector3(0, m_MoveSpeed, 0);
            transform.position = newPos;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerPowerManager powerManager = collision.gameObject.GetComponent<PlayerPowerManager>();

                powerManager.m_CurrentPower += m_PickupPowerIncrease;

                gameObject.SetActive(false);

                m_SpawnIndicator.SetIsActivePowerup(false);
                m_SpawnIndicator = null;
            }
        }
    }

}

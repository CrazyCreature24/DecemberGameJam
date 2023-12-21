using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowerManager : MonoBehaviour
{
    public float m_MaxPower = 1.0f;
    public float m_CurrentPower = 1.0f;
    public bool m_IsMoving = false;
    public float m_PowerDepletionMultiplier = 0.1f;
    public float m_PowerIncreaseMultiplier = 0.2f;
    public ParticleSystem ShieldFX;

    PlayerMovement m_MovementReference;
    public GameObject m_LevelManager;

    bool bIsShieldActive = false;
    float ShieldTimer = 0.0f;
    float DamageReduction = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_MovementReference = GetComponent<PlayerMovement>();

        m_CurrentPower = m_MaxPower;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovementPower();

        if (bIsShieldActive)
        {
            if(!ShieldFX.isPlaying)
                ShieldFX.Play();
            ShieldTimer -= Time.fixedDeltaTime;
            if (ShieldTimer <= 0.0f)
            {
                ShieldTimer = 0.0f;
                bIsShieldActive = false;
            }
        }
        else
            ShieldFX.Stop(true);
    }

    private void UpdateMovementPower()
    {
        m_IsMoving = CheckIsMoving();

        if (m_IsMoving == true && m_CurrentPower > 0.0f)
        {
            m_CurrentPower -= Time.fixedDeltaTime * m_PowerDepletionMultiplier;
        }
        else if (m_IsMoving == false && m_CurrentPower < m_MaxPower)
        {
            m_CurrentPower += Time.fixedDeltaTime * m_PowerIncreaseMultiplier;
        }

        if (m_CurrentPower <= 0.0f)
        {
            m_CurrentPower = 0.0f;

            LevelManager levelManager = m_LevelManager.GetComponent<LevelManager>();

            if (levelManager != null)
            {
                levelManager.Respawn();
            }
        }

        if (m_CurrentPower > m_MaxPower)
        {
            m_CurrentPower = m_MaxPower;
        }
    }

    public bool CheckIsMoving()
    {
        bool isMoving = false;

        float playerSqrMagnitude = m_MovementReference.m_Velocity.sqrMagnitude;

        if(playerSqrMagnitude > MathUtils.CompareEpsilon)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        return isMoving;
    }

    public void ActivateShield(float shieldTimer, float damageReduction)
    {
        bIsShieldActive = true;
        ShieldTimer = shieldTimer;
        DamageReduction = damageReduction;
    }

    public void TakePowerDamage(float damage)
    {
        if(bIsShieldActive)
        {
            damage -= damage * DamageReduction;
        }
        m_CurrentPower -= damage;
    }
}

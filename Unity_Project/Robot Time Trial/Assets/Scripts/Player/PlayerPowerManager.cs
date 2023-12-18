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

    PlayerMovement m_MovementReference;

    public Scrollbar m_ScrollBar;
    public TMP_Text m_BarText;


    // Start is called before the first frame update
    void Start()
    {
        m_MovementReference = GetComponent<PlayerMovement>();

        m_CurrentPower = m_MaxPower;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_IsMoving = CheckIsMoving();

        if (m_IsMoving == true && m_CurrentPower > 0.0f)
        {
            m_CurrentPower -= Time.fixedDeltaTime * m_PowerDepletionMultiplier;

            if  (m_CurrentPower <= 0.0f)
            {
                m_CurrentPower = 0.0f;
                m_MovementReference.Respawn();
            }

        }
        else if (m_IsMoving == false && m_CurrentPower < m_MaxPower)
        {
            m_CurrentPower += Time.fixedDeltaTime * m_PowerIncreaseMultiplier;

            if (m_CurrentPower > m_MaxPower)
            {
                m_CurrentPower = m_MaxPower;
            }
        }



        m_ScrollBar.size = m_CurrentPower;
        m_BarText.text = (m_CurrentPower * 100).ToString() + "%";

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
}

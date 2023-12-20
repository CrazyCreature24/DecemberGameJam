using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Move : MonoBehaviour
{
    float m_TimeElapsed = 0;

    public bool m_CanMove = true;
    public float m_MoveSpeed = 0.01f;
    public float m_MoveTime = 1.0f;

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
}

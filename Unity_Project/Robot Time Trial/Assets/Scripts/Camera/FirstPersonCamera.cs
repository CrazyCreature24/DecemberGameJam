using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    Vector3 m_LookInput = Vector3.zero;
    PlayerMovement m_Player;

    public float m_RotationSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    public void UpdateCamera()
    {

        m_LookInput += m_Player.m_Controller.GetLookInput();

        // X is y and Y is x
        transform.rotation = Quaternion.Euler(-m_LookInput.x, m_LookInput.y, 0);

    }

}

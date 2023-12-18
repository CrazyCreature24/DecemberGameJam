using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetLookInput()
    {
        //Don't allow looking around if mouse isn't enabled
        if (!m_EnableMouseControl)
        {
            return Vector3.zero;
        }

        return new Vector3(Input.GetAxis("Mouse Y") * 2.0f, Input.GetAxis("Mouse X") * 2.0f, 0);
    }

    public Vector3 GetMoveInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
    }

    public bool IsJumping()
    {
        return Input.GetButton("Jump");
    }

    public void UpdateCanMouseControl()
    {
        if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        m_EnableMouseControl = (Cursor.lockState == CursorLockMode.Locked) ? true : false;

        Cursor.visible = !m_EnableMouseControl;
    }





    bool m_EnableMouseControl;
}

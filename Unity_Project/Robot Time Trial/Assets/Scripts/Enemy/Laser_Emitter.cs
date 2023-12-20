using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Emitter : MonoBehaviour
{
    LevelManager m_LevelManager;

    // Start is called before the first frame update
    void Start()
    {
        m_LevelManager = FindAnyObjectByType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        m_LevelManager.Respawn();
    }
}

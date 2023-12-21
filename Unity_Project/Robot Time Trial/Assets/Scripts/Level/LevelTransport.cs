using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransport : MonoBehaviour
{
    [SerializeField]
    public string m_NewScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Level Travel triggered successfully");

            if (m_NewScene != null)
            {
                PoolManager.CleanPools();
                SceneManager.LoadScene(m_NewScene, LoadSceneMode.Single);
            }
        }
    }
}

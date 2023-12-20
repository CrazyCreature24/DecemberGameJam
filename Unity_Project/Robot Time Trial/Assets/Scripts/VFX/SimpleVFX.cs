using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVFX : MonoBehaviour
{
    public float Lifetime = 2.0f;

    float Elapsed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        Elapsed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Elapsed += Time.deltaTime;
        if(Elapsed >= Lifetime)
        {
            gameObject.SetActive(false);
        }
    }
}

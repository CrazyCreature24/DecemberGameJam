using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public Button m_PlayButton;
    public Button m_ExitButton;


    // Start is called before the first frame update
    void Start()
    {
        if (m_PlayButton != null)
        {
            m_PlayButton.onClick.AddListener(Play);
        }

        if (m_ExitButton != null)
        {
            m_ExitButton.onClick.AddListener(Exit);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Play()
    {
        SceneManager.LoadScene("Test Level",LoadSceneMode.Single);
    }

    private void Exit()
    {
        if (UnityEditor.EditorApplication.isPlaying == true)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

}

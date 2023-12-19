using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIHud : MonoBehaviour
{
    LevelManager m_LevelManager;
    PlayerPowerManager m_PlayerPowerManager;

    public TMP_Text m_LevelTimerText;

    public Scrollbar m_PowerLevelBar;
    public TMP_Text m_PowerLevelText;

    // Start is called before the first frame update
    void Start()
    {
        m_LevelManager = FindAnyObjectByType<LevelManager>();
        m_PlayerPowerManager = FindAnyObjectByType<PlayerPowerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the level timer on the screen
        m_LevelTimerText.text = m_LevelManager.m_CurrentTime.ToString("0.00") + " Time Remaining";

        if (m_LevelManager.m_CurrentTime <= 0)
        {
            YouFail();
        }


        // Displays the Powerlevel on the screen
        m_PowerLevelBar.size = m_PlayerPowerManager.m_CurrentPower;
        m_PowerLevelText.text = (m_PlayerPowerManager.m_CurrentPower * 100).ToString() + "%";
    }

    void YouFail()
    {
        //Will want a "You failed" screen to pop up, but it currently just restarts the player.

        m_LevelManager.Respawn();
        m_LevelManager.ResetTime();
    }
}

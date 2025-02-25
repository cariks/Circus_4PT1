using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigation : MonoBehaviour
{

    public GameObject SettingsMenu;
    public GameObject MainMenu;
    public GameObject Leaderboard;


    public void OpenSettings()
    {
        if(SettingsMenu != null)
        {
            SettingsMenu.SetActive(true);
            MainMenu.SetActive(false);
        }
    }

    public void CloseSettings()
    {
        if (SettingsMenu != null)
        {
            SettingsMenu.SetActive(false);
            MainMenu.SetActive(true);
        }
    }

    public void OpenLeaderboard()
    {
        if (Leaderboard != null)
        {
            Leaderboard.SetActive(true);
            MainMenu.SetActive(false);
        }
    }

    public void CloseLeaderboard()
    {
        if (Leaderboard != null)
        {
            Leaderboard.SetActive(false);
            MainMenu.SetActive(true);
        }
    }
}

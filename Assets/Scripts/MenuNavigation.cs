using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigation : MonoBehaviour
{

    public GameObject SettingsMenu;
    public GameObject MainMenu;
    
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
}

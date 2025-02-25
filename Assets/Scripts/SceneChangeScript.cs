using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{
    public FadeScript fadeScript;
    public SaveLoadScript saveLoadScript;

    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, ""));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(Delay("menu", -1, ""));
    }

    // Новый метод для загрузки MainMenu без fadeScript
    public void LoadMainMenuWithoutFade()
    {
        SceneManager.LoadScene("MainMenue", LoadSceneMode.Single);
    }

    public IEnumerator Delay(string command, int characterIndex, string name)
    {
        yield return fadeScript.FadeIn(0.1f);

        if (string.Equals(command, "quit", StringComparison.OrdinalIgnoreCase))
        {
            PlayerPrefs.DeleteAll();
            if (UnityEditor.EditorApplication.isPlaying)
                UnityEditor.EditorApplication.isPlaying = false;
            else
                Application.Quit();
        }
        else if (string.Equals(command, "play", StringComparison.OrdinalIgnoreCase))
        {
            saveLoadScript.SaveGame(characterIndex, name);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else if (string.Equals(command, "menu", StringComparison.OrdinalIgnoreCase))
        {
            SceneManager.LoadScene("MainMenue", LoadSceneMode.Single);
        }
    }
}

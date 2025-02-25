using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject pausePanel; // Окно паузы
    public GameObject settingPanel;
    public GameManager gameManager; // Ссылка на GameManager

    private bool isPaused = false;

    void Update()
    {

    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Останавливаем время
            pausePanel.SetActive(true); // Показываем окно паузы
        }
        else
        {
            Time.timeScale = 1f; // Продолжаем игру
            pausePanel.SetActive(false); // Скрываем окно паузы
            settingPanel.SetActive(false);
        }
    }
}

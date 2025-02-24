using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject pausePanel; // Окно паузы
    public GameObject settingPanel;

    private bool isPaused = false;

    void Update()
    {
        // Если нажата клавиша ESC, включаем/выключаем паузу
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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
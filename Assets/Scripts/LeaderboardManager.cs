using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro; // Добавляем TextMeshPro
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject leaderboardPanel; // Панель лидерборда
    public Transform content; // Контейнер для строк таблицы
    public GameObject leaderboardEntryPrefab; // Префаб строки

    public Sprite goldMedal;  // Золотая медаль
    public Sprite silverMedal; // Серебряная медаль
    public Sprite bronzeMedal; // Бронзовая медаль

    private string leaderboardFilePath;

    private void Start()
    {
        leaderboardFilePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        leaderboardPanel.SetActive(false); // Скрываем панель при старте
    }

    public void ShowLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        LoadLeaderboardData();
    }

    public void HideLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    private void LoadLeaderboardData()
    {
        // Удаляем старые записи, чтобы не дублировать
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        if (!File.Exists(leaderboardFilePath))
        {
            Debug.LogWarning("Файл leaderboard.json не найден!");
            return;
        }

        string json = File.ReadAllText(leaderboardFilePath);
        LeaderboardList leaderboardData = JsonUtility.FromJson<LeaderboardList>(json);

        // Подсчитываем количество игр каждого игрока
        Dictionary<string, (int totalScore, int gamesPlayed)> playerStats = new Dictionary<string, (int, int)>();

        foreach (var entry in leaderboardData.entries)
        {
            if (playerStats.ContainsKey(entry.playerName))
            {
                playerStats[entry.playerName] = (
                    playerStats[entry.playerName].totalScore + entry.score,
                    playerStats[entry.playerName].gamesPlayed + 1
                );
            }
            else
            {
                playerStats[entry.playerName] = (entry.score, 1);
            }
        }

        // Сортируем по общему количеству очков (по убыванию)
        var sortedPlayers = playerStats.OrderByDescending(p => p.Value.totalScore).ToList();

        // Создаем строки таблицы
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            var player = sortedPlayers[i];

            GameObject entryObject = Instantiate(leaderboardEntryPrefab, content);
            TMP_Text[] texts = entryObject.GetComponentsInChildren<TMP_Text>();
            Image medalImage = entryObject.transform.Find("MedalImage").GetComponent<Image>(); // Ищем объект Image для медали

            texts[0].text = player.Key; // Имя игрока
            texts[2].text = player.Value.totalScore.ToString(); // Очки
            texts[1].text = $"Games played: {player.Value.gamesPlayed}"; // Количество игр

            // Назначаем медали
            if (i == 0)
                medalImage.sprite = goldMedal;
            else if (i == 1)
                medalImage.sprite = silverMedal;
            else if (i == 2)
                medalImage.sprite = bronzeMedal;
            else
                medalImage.gameObject.SetActive(false); // Прячем медаль у остальных
        }
    }

    [System.Serializable]
    private class LeaderboardList
    {
        public List<LeaderboardEntry> entries;
    }

    [System.Serializable]
    private class LeaderboardEntry
    {
        public string playerName;
        public int score;
    }
}

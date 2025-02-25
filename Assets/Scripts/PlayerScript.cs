using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject spawnPoint;
    int[] otherPlayers;
    int index;

    private const string textFileName = "playerNames";

    public static event Action<List<GameObject>> OnPlayersReady;
    public static event Action<GameObject> OnPlayerReady;

    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex],
            spawnPoint.transform.position, Quaternion.identity);

        string mainPlayerName = PlayerPrefs.GetString("PlayerName");
        mainCharacter.GetComponent<NameScript>().SetPlayerName(mainPlayerName);
        mainCharacter.GetComponent<PlayerController>().SetPlayerName(mainPlayerName);

        OnPlayerReady?.Invoke(mainCharacter);

        int playerCount = PlayerPrefs.GetInt("PlayerCount");
        otherPlayers = new int[playerCount - 1];

        List<string> availableNames = new List<string>(ReadLinesFromFile(textFileName)); // Загружаем имена

        List<GameObject> players = new List<GameObject> { mainCharacter };

        for (int i = 0; i < otherPlayers.Length; i++)
        {
            spawnPoint.transform.position += new Vector3(0.4f, 0, 0.08f);
            index = Random.Range(0, playerPrefabs.Length);
            GameObject character = Instantiate(playerPrefabs[index], spawnPoint.transform.position, Quaternion.identity);

            string botName;
            if (availableNames.Count > 0)
            {
                int nameIndex = Random.Range(0, availableNames.Count);
                botName = availableNames[nameIndex];
                availableNames.RemoveAt(nameIndex); // Удаляем имя, чтобы оно не повторялось
            }
            else
            {
                botName = $"Bot_{i + 1}"; // Если имен не хватает, используем шаблонное имя
            }

            character.GetComponent<NameScript>().SetPlayerName(botName);
            character.GetComponent<PlayerController>().SetPlayerName(botName);
            players.Add(character);
            character.GetComponent<PlayerController>().isBot = true;
        }

        OnPlayersReady?.Invoke(players);
    }

    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset != null)
            return textAsset.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Debug.LogError("File not found: " + fileName);
        return new string[0];
    }
}

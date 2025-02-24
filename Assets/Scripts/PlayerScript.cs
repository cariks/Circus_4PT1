using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
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

    public static event Action<GameObject> OnPlayerReady; // Создаём событие для главного игрока


    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex],
            spawnPoint.transform.position, Quaternion.identity);

        mainCharacter.GetComponent<NameScript>().SetPlayerName(PlayerPrefs.GetString("PlayerName"));
        OnPlayerReady?.Invoke(mainCharacter);

        // Получаем количество других игроков
        int playerCount = PlayerPrefs.GetInt("PlayerCount");

        // Создаем массив otherPlayers для ботов
        otherPlayers = new int[playerCount - 1]; // -1 потому что главный игрок не учитывается
        string[] nameArray = ReadLinesFromFile(textFileName);

        List<GameObject> players = new List<GameObject> { mainCharacter };

        for (int i = 0; i < otherPlayers.Length; i++) // Здесь мы создаем только других игроков
        {
            spawnPoint.transform.position += new Vector3(0.4f, 0, 0.08f);
            index = Random.Range(0, playerPrefabs.Length);
            GameObject character = Instantiate(playerPrefabs[index], spawnPoint.transform.position, Quaternion.identity);
            character.GetComponent<NameScript>().SetPlayerName(nameArray[Random.Range(0, nameArray.Length)]);
            players.Add(character);

            // Присваиваем флаг bot для всех, кроме главного игрока
            character.GetComponent<PlayerController>().isBot = true;
        }

        // Извлекаем событие, когда все игроки готовы
        OnPlayersReady?.Invoke(players);
    }


    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
            return textAsset.text.Split(new[] { '\r', '\n' },
                System.StringSplitOptions.RemoveEmptyEntries);
        else
            Debug.LogError("File not found: " + fileName); return new string[0];
    }
}
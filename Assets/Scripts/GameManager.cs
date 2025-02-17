using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> players = new List<GameObject>(); //player list
    private int currentPlayerIndex = 0; // current player index
    public CinemachineVirtualCamera cam; // link uz virtualo kameru

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        PlayerScript.OnPlayersReady += InitializePlayers;

    }

    void Start()
    {
        if (players.Count > 0)
        {
            SetActivePlayer();
        }
    }

    void InitializePlayers(List<GameObject> loadedPlayers)
    {
        players = loadedPlayers;

        SetActivePlayer();
    }

    void SetActivePlayer()
    {
        if (players.Count == 0) return;
        cam.Follow = players[currentPlayerIndex].transform;
        cam.LookAt = players[currentPlayerIndex].transform;
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 1f, cam.transform.position.z);

    }

    public void NextTurn()
    {
        if (players.Count == 0) return;

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        cam.Follow = players[currentPlayerIndex].transform;
        cam.LookAt = players[currentPlayerIndex].transform;
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 0.2f, cam.transform.position.z);
        Vector3 targetPosition = players[currentPlayerIndex].transform.position + new Vector3(0, 2, 0);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * 1f);

        string currentPlayerName = players[currentPlayerIndex].GetComponent<NameScript>().GetPlayerName();

        Debug.Log("Current player: " + currentPlayerName);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextTurn();
        }
    }

    private void OnDestroy()
    {
        PlayerScript.OnPlayersReady -= InitializePlayers;
    }
}
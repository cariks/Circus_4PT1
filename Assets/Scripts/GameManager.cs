using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> players = new List<GameObject>(); // Список игроков
    private int currentPlayerIndex = 0; // Текущий игрок

    public CinemachineVirtualCamera cam; // Cinemachine-камера
    public DiceRollScript dice; // Ссылка на скрипт кубика

    private Vector3 startCameraPosition = new Vector3(9, 25, -7f); // Позиция камеры при старте
    private Quaternion startCameraRotation = Quaternion.Euler(60, 0, 0); // Стартовый угол камеры

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        PlayerScript.OnPlayersReady += InitializePlayers;
    }

    private void Start()
    {
        cam.transform.position = startCameraPosition;
        cam.transform.rotation = startCameraRotation;

        if (dice != null)
        {
            dice.gameObject.SetActive(false);
        }

        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        dice.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f); // Ждём 2 секунды, показывая поле

        if (players.Count > 0)
        {
            GameObject targetPlayer = players[currentPlayerIndex];

            StartCoroutine(SmoothCameraAim(targetPlayer.transform, 1f)); // Камера начинает наводиться
            yield return StartCoroutine(SmoothCameraTransition(targetPlayer.transform, 1f)); // Камера движется к игроку
        }

        yield return new WaitForSeconds(0.5f); // Короткая пауза перед броском

        if (dice != null)
        {
            yield return new WaitForSeconds(0.3f);
            dice.RollDice(); // Автоматически бросаем кубик
            dice.gameObject.SetActive(true);
        }
    }

    private IEnumerator SmoothCameraTransition(Transform target, float duration)
    {
        Vector3 startPosition = cam.transform.position;
        Quaternion startRotation = cam.transform.rotation;

        Vector3 targetPosition = target.position + new Vector3(0, 2, -3);
        Quaternion targetRotation = Quaternion.LookRotation(target.position - cam.transform.position);

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            cam.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            cam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = targetPosition;
        cam.transform.rotation = targetRotation;

        cam.Follow = target;
        cam.LookAt = target;
    }

    private IEnumerator SmoothCameraAim(Transform target, float duration)
    {
        Quaternion startRotation = cam.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - cam.transform.position);

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            cam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.rotation = targetRotation;
    }

    private void InitializePlayers(List<GameObject> loadedPlayers)
    {
        players = loadedPlayers;
    }

    private void OnDestroy()
    {
        PlayerScript.OnPlayersReady -= InitializePlayers;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> players = new List<GameObject>(); // Список игроков
    public int currentPlayerIndex = 0; // Текущий игрок

    public CinemachineVirtualCamera cam; // Cinemachine-камера
    public DiceRollScript dice; // Ссылка на скрипт кубика

    private Vector3 startCameraPosition = new Vector3(9, 25, -7f); // Позиция камеры при старте
    private Quaternion startCameraRotation = Quaternion.Euler(60, 0, 0); // Стартовый угол камеры

    public bool isPlayerMoving = false; // Флаг, проверяющий, движется ли игрок

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        PlayerScript.OnPlayersReady += InitializePlayers;
        FindAllTiles();
    }

    private void Start()
    {
        cam.transform.position = startCameraPosition;
        cam.transform.rotation = startCameraRotation;

        if (dice != null)
        {
            dice.gameObject.SetActive(false);
        }

        for (int i = 0; i < players.Count; i++)
        {
            PlayerController playerController = players[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.playerIndex = i; // Устанавливаем уникальный индекс для каждого игрока
            }
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

            StartCoroutine(SmoothCameraAim(targetPlayer.transform, 0.5f)); // Камера начинает наводиться
            yield return StartCoroutine(SmoothCameraTransition(targetPlayer.transform, 1f)); // Камера движется к игроку
        }

        yield return new WaitForSeconds(0.5f); // Короткая пауза перед броском

        if (dice != null)
        {
            yield return new WaitForSeconds(0.3f);
            dice.StartInitialRoll(); // Автоматически бросаем кубик
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

 


    public List<Tile> tiles = new List<Tile>();

    private void FindAllTiles()
    {
        tiles.Clear(); // Очищаем список на случай повторного вызова

        GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tileObj in tileObjects)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }

        // Сортируем клетки по их индексу, чтобы они шли по порядку
        tiles.Sort((a, b) => a.index.CompareTo(b.index));

        // Выводим информацию о каждой клетке в консоль
        foreach (var tile in tiles)
        {
            Debug.Log($"Tile Index: {tile.index}, Is Bad: {tile.isBadTile}, Penalty: {tile.penaltySteps}");
        }
    }




    public IEnumerator MovePlayer(GameObject player, int steps)
    {
        if (isPlayerMoving) yield break;

        yield return new WaitUntil(() => dice.IsStopped());
        yield return new WaitUntil(() => dice.difeFaceNum != "?");

        isPlayerMoving = true;

        PlayerController playerController = player.GetComponent<PlayerController>();

        while (steps != 0)
        {
            int currentTileIndex = playerController.currentTileIndex;
            int lastTileIndex = tiles.Count - 1;
            int targetTileIndex = currentTileIndex + steps;

            Debug.Log($"[MovePlayer] Player {playerController.playerIndex} ({player.name}) starts at {currentTileIndex}, moving {steps} steps.");

            bool movingForward = steps > 0;

            if (targetTileIndex > lastTileIndex)
            {
                int overshoot = targetTileIndex - lastTileIndex;
                Debug.Log($"[MovePlayer] Player reached the finish at {lastTileIndex}, needs to go back {overshoot} steps.");

                // Доходим до финиша
                yield return StartCoroutine(MoveStepByStep(player, currentTileIndex, lastTileIndex, 1));

                // Начинаем движение назад сразу же, не телепортируем
                targetTileIndex = lastTileIndex - overshoot;
                steps = -overshoot; // Обновляем шаги на отрицательные
                movingForward = false;
            }

            // Обработка движения назад
            if (steps < 0)
            {
                // Начинаем движение назад, двигаемся шаг за шагом
                yield return StartCoroutine(MoveStepByStep(player, playerController.currentTileIndex, targetTileIndex, -1));
            }
            else
            {
                // Двигаемся вперед, как обычно
                yield return StartCoroutine(MoveStepByStep(player, playerController.currentTileIndex, targetTileIndex, 1));
            }

            // Обновляем позицию игрока
            Tile targetTile = tiles[targetTileIndex];
            playerController.currentTileIndex = targetTileIndex;
            Debug.Log($"[MovePlayer] Player {playerController.playerIndex} ({player.name}) landed on tile {targetTileIndex} (Bad: {targetTile.isBadTile})");

            // Проверка на плохую клетку
            if (targetTile.isBadTile)
            {
                int penalty = targetTile.penaltySteps;
                int newTargetIndex = targetTileIndex - penalty;
                if (newTargetIndex < 0) newTargetIndex = 0;

                Debug.Log($"[MovePlayer] Player got a penalty: -{penalty} steps (New target index: {newTargetIndex}).");

                steps = newTargetIndex - targetTileIndex;
            }
            else
            {
                steps = 0;
            }
        }

        yield return new WaitForSeconds(0.5f);
        isPlayerMoving = false;
        NextTurn();
    }






    private IEnumerator MoveStepByStep(GameObject player, int startIdx, int endIdx, int step)
    {
        // Предотвращаем выход за границы списка
        if (endIdx >= tiles.Count) endIdx = tiles.Count - 1;
        if (endIdx < 0) endIdx = 0;

        for (int i = startIdx + step; step > 0 ? i <= endIdx : i >= endIdx; i += step)
        {
            if (i < 0 || i >= tiles.Count) break; // Проверка границ списка

            float randomOffsetX = Random.Range(-0.2f, 0.2f);
            float randomOffsetZ = Random.Range(-0.2f, 0.2f);

            Vector3 startPosition = player.transform.position;
            Vector3 targetPosition = new Vector3(tiles[i].transform.position.x + randomOffsetX, 0.3f, tiles[i].transform.position.z + randomOffsetZ);

            float moveDuration = 0.6f;
            float elapsedTime = 0f;
            float jumpHeight = 0.5f;

            while (elapsedTime < moveDuration)
            {
                float t = elapsedTime / moveDuration;
                player.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                float jumpT = Mathf.Sin(t * Mathf.PI);
                player.transform.position = new Vector3(
                    player.transform.position.x,
                    Mathf.Lerp(0.3f, 0.3f + jumpHeight, jumpT),
                    player.transform.position.z
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            player.transform.position = targetPosition;
            yield return new WaitForSeconds(0.05f);
        }
    }












    public void RollDice()
    {
        if (isPlayerMoving) return;  // Блокируем подбрасывание кубика, если игрок двигается
        Debug.Log("Rolling dice...");
        dice.RollDice(); // Просто просим кубик броситься
    }

    public void OnDiceRolled(int diceValue)
    {
        Debug.Log($"[OnDiceRolled] Dice rolled: {diceValue}");
        StartCoroutine(MovePlayer(players[currentPlayerIndex], diceValue));
    }

    public void NextTurn()
    {
        if (isPlayerMoving) return; // Если игрок еще двигается, не начинаем следующий ход

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        Debug.Log($"Switching to player {currentPlayerIndex}, {players[currentPlayerIndex].name}.");

        // Переводим камеру на нового игрока
        StartCoroutine(SmoothCameraTransition(players[currentPlayerIndex].transform, 1f));
    }




    private void OnDestroy()
    {
        PlayerScript.OnPlayersReady -= InitializePlayers;
    }
}
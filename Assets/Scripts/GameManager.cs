using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> players = new List<GameObject>(); // Список игроков
    public int currentPlayerIndex = 0; // Текущий игрок

    public CinemachineVirtualCamera cam; // Cinemachine-камера
    public Transform diceTransform; // Убедись, что сюда назначен сам кубик
    public Transform TileStart;

    public DiceRollScript dice; // Ссылка на скрипт кубика

    private Vector3 startCameraPosition = new Vector3(9, 25, -7f); // Позиция камеры при старте
    private Quaternion startCameraRotation = Quaternion.Euler(60, 0, 0); // Стартовый угол камеры

    public bool isPlayerMoving = false; // Флаг, проверяющий, движется ли игрок

    [SerializeField] public Button rollButton1;

    public PostProcessVolume postProcessVolume; // Ссылка на Post-process Volume
    private DepthOfField dof; // Переменная для хранения эффекта глубины резкости


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
            DiceRollScript.OnDiceRolled += SwitchCameraToDice;

        }

        for (int i = 0; i < players.Count; i++)
        {
            PlayerController playerController = players[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.playerIndex = i; // Устанавливаем уникальный индекс для каждого игрока
            }
        }

        // Получаем компонент DepthOfField из PostProcessVolume
        if (postProcessVolume.profile.TryGetSettings(out dof))
        {
            dof.focusDistance.value = 8f; // Устанавливаем начальное значение
        }

        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        dice.gameObject.SetActive(false);

        if (dof != null)
        {
            dof.focusDistance.value = 8f;
        }

        yield return new WaitForSeconds(2f); // Ждём 2 секунды, показывая поле

        StartCoroutine(SmoothFocusDistance(8f, 5.6f, 3f));

        if (players.Count > 0)
        {
            GameObject targetPlayer = players[0];


            StartCoroutine(SmoothCameraAim(players[0].transform, 0.01f)); // Камера начинает наводиться
            yield return StartCoroutine(SmoothCameraTransition(targetPlayer.transform, 1.5f)); // Камера движется к игроку
        }


        yield return new WaitForSeconds(0.5f); // Короткая пауза перед броском

        if (dice != null)
        {
            yield return new WaitForSeconds(0.3f);
            dice.StartInitialRoll(); // Автоматически бросаем кубик
            dice.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2f);




        //cam.LookAt = diceTransform;

    }

    private IEnumerator SmoothFocusDistance(float start, float end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            dof.focusDistance.value = Mathf.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        dof.focusDistance.value = end;
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
            float t = elapsedTime / duration;
            cam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

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

        cam.LookAt = player.transform;



        yield return new WaitUntil(() => dice.IsStopped());
        yield return new WaitUntil(() => dice.difeFaceNum != "?");

        isPlayerMoving = true;



        StartCoroutine(SmoothCameraTransition(players[currentPlayerIndex].transform, 0.1f));

        PlayerController playerController = player.GetComponent<PlayerController>();
        int lastTileIndex = tiles.Count - 1;

        while (steps != 0)
        {
            int currentTileIndex = playerController.currentTileIndex;
            int targetTileIndex = currentTileIndex + steps;

            Debug.Log($"[MovePlayer] Player {playerController.playerIndex} ({player.name}) starts at {currentTileIndex}, moving {steps} steps.");

            yield return new WaitForSeconds(0.5f);
            // 1️ Сначала проверяем, попал ли игрок ровно на финиш
            if (targetTileIndex == lastTileIndex)
            {
                yield return StartCoroutine(MoveStepByStep(player, currentTileIndex, targetTileIndex, 1));
                playerController.currentTileIndex = lastTileIndex;

                Debug.Log($"[MovePlayer] 🎉 Player {playerController.playerIndex} ({player.name}) reached the finish exactly! 🎉");

                steps = 0; // Победа, больше ходов не делаем
                break;
            }

            // 2️ Затем проверяем, перескакивает ли он финиш
            if (targetTileIndex > lastTileIndex)
            {
                int overshoot = targetTileIndex - lastTileIndex;
                targetTileIndex = lastTileIndex; // Доходим до финиша

                yield return StartCoroutine(MoveStepByStep(player, currentTileIndex, targetTileIndex, 1));
                playerController.currentTileIndex = lastTileIndex;

                Debug.Log($"[MovePlayer] Player reached the finish at {lastTileIndex}, moving back {overshoot} steps.");

                // Теперь устанавливаем штрафные очки
                tiles[lastTileIndex].isBadTile = true;
                tiles[lastTileIndex].penaltySteps = overshoot;

                steps = -overshoot;
                continue;
            }

            // 3️⃣ Обычное перемещение
            yield return StartCoroutine(MoveStepByStep(player, currentTileIndex, targetTileIndex, steps > 0 ? 1 : -1));
            playerController.currentTileIndex = targetTileIndex;
            Tile targetTile = tiles[targetTileIndex];

            Debug.Log($"[MovePlayer] Player {playerController.playerIndex} ({player.name}) landed on tile {targetTileIndex} (Bad: {targetTile.isBadTile})");

            // Проверяем, "плохая" ли клетка
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









    private void SwitchCameraToDice()
    {
        // Здесь мы только настраиваем камеру, чтобы она смотрела на кубик, не двигаясь.
        cam.LookAt = diceTransform;
        cam.Follow = TileStart; // Камера не следует за кубиком, только смотрит на него


    }


    public void OnDiceRolled(int diceValue)
    {
        Debug.Log($"[OnDiceRolled] Dice rolled: {diceValue}");

        StartCoroutine(MovePlayer(players[currentPlayerIndex], diceValue));
    }

    public void NextTurn()
    {
        rollButton1.interactable = false;  // Делаем кнопку неактивной

        if (isPlayerMoving) return; // Если игрок еще двигается, не начинаем следующий ход

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        Debug.Log($"Switching to player {currentPlayerIndex}, {players[currentPlayerIndex].name}.");

        // Переводим камеру на игрока после завершения хода
        cam.LookAt = players[currentPlayerIndex].transform; // Камера будет смотреть на нового игрока
        cam.Follow = players[currentPlayerIndex].transform; // Камера будет следовать за новым игроком

        // Проверяем, является ли игрок ботом и если да, подбрасываем кубик
        StartCoroutine(HandleBotTurn());
    }

    private IEnumerator HandleBotTurn()
    {
        // Проверяем, является ли текущий игрок ботом
        if (players[currentPlayerIndex].GetComponent<PlayerController>().isBot)
        {
            rollButton1.interactable = false;  // Делаем кнопку неактивной

            SwitchCameraToDice();

            yield return new WaitForSeconds(1f); // Задержка перед ходом бота

            
            // Бросаем кубик для бота
            DiceRollScript diceRollScript = dice.GetComponent<DiceRollScript>();

            if (diceRollScript != null)
            {
                // Вызываем метод для подбрасывания кубика
                diceRollScript.RollDice();
            }
        }
        else
        {
            rollButton1.interactable = true;
        }
    }






    private void OnDestroy()
    {
        PlayerScript.OnPlayersReady -= InitializePlayers;

        // Отписываемся от события при уничтожении объекта
        if (dice != null)
        {
            DiceRollScript.OnDiceRolled -= SwitchCameraToDice;
        }
    }
}
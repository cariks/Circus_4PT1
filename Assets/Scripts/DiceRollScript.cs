using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollScript : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Vector3 startPosition;

    [SerializeField] private float minThrowForce = 200f, maxThrowForce = 600f;
    [SerializeField] private float minTorque = 1000f, maxTorque = 3000f;

    public string difeFaceNum = "?"; // По умолчанию отображаем "?"
    public bool firstThrow = false;
    public bool isLanded = false;
    public bool initialThrowDone = false;

    [SerializeField] public Button rollButton;

    [SerializeField] private Camera cam; // Ссылка на камеру
    [SerializeField] private Transform player; // Ссылка на игрока для возврата камеры

    private void OnEnable()
    {
        PlayerScript.OnPlayerReady += SetPlayer; // Подписываемся на событие
    }

    private void OnDisable()
    {
        PlayerScript.OnPlayerReady -= SetPlayer; // Отписываемся от события
    }

    private void SetPlayer(GameObject playerObject)
    {
        player = playerObject.transform; // Устанавливаем ссылку на игрока
    }

    private void Awake()
    {
        Initialize();
        rollButton.onClick.AddListener(RollDice);
        isLanded = false;

        rollButton.gameObject.SetActive(false); // Скрываем кнопку

    }
    private void Update()
    {
        if (GameManager.Instance.isPlayerMoving)
        {
            rollButton.interactable = false;  // Делаем кнопку неактивной
        }
        else
        {
            rollButton.interactable = true;  // Включаем кнопку
        }
    }

    private void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        startPosition = transform.position;
        RandomizeRotation();
        isLanded = false;
    }

    private void RandomizeRotation()
    {
        transform.rotation = Random.rotation;
    }

    public void StartInitialRoll()
    {
        if (initialThrowDone) return;

        transform.position = startPosition + new Vector3(0, 5, 0);
        rigidbody.isKinematic = false; // Разрешаем физику

        float throwForce = Random.Range(minThrowForce, maxThrowForce);
        rigidbody.AddForce(Vector3.down * throwForce);

        float torqueX = Random.Range(minTorque, maxTorque);
        float torqueY = Random.Range(minTorque, maxTorque);
        float torqueZ = Random.Range(minTorque, maxTorque);
        rigidbody.AddTorque(new Vector3(torqueX, torqueY, torqueZ));

        initialThrowDone = true;
        rollButton.gameObject.SetActive(true);

    }

    public void RollDice()
    {
        if (!isLanded || GameManager.Instance.isPlayerMoving) return;

        Debug.Log("[RollDice] Rolling the dice...");

        difeFaceNum = "?"; // Сбрасываем значение перед броском
        rigidbody.isKinematic = false;

        float throwForce = Random.Range(minThrowForce, maxThrowForce);
        rigidbody.AddForce(Vector3.up * throwForce);

        float torqueX = Random.Range(minTorque, maxTorque);
        float torqueY = Random.Range(minTorque, maxTorque);
        float torqueZ = Random.Range(minTorque, maxTorque);
        rigidbody.AddTorque(new Vector3(torqueX, torqueY, torqueZ));

        isLanded = false;
        firstThrow = true;

        Debug.Log("[RollDice] Dice thrown, starting WaitForDiceStop...");
        StartCoroutine(WaitForDiceStop());

    }

    private IEnumerator WaitForDiceStop()
    {
        Debug.Log("[WaitForDiceStop] Started. Camera following the dice.");
        cam.transform.parent = transform;
        cam.transform.position = transform.position + new Vector3(0, 1, -3);

        yield return new WaitUntil(() =>
        {
            Debug.Log($"[WaitForDiceStop] Checking if dice stopped. Velocity: {rigidbody.velocity.magnitude}, Angular: {rigidbody.angularVelocity.magnitude}");
            return IsStopped();
        });

        Debug.Log("[WaitForDiceStop] Dice stopped.");

        // Теперь ждем, пока `difeFaceNum` обновится (то есть станет не "?")
        yield return new WaitUntil(() => difeFaceNum != "?");

        Debug.Log($"[WaitForDiceStop] Face number confirmed: {difeFaceNum}");

        // Камера возвращается к игроку
        ReturnCameraToPlayer();

        if (int.TryParse(difeFaceNum, out int result))
        {
            Debug.Log($"[WaitForDiceStop] Dice roll result: {result}. Sending to GameManager.");
            GameManager.Instance.OnDiceRolled(result);
        }
        else
        {
            Debug.LogError("[WaitForDiceStop] ERROR: Failed to parse dice face number!");
        }

        yield return new WaitForSeconds(1f);
        GameManager.Instance.NextTurn();
    }



    // Проверяем, остановился ли кубик
    public bool IsStopped()
    {
        bool stopped = rigidbody.velocity.magnitude < 0.1f && Mathf.Abs(rigidbody.angularVelocity.magnitude) < 0.1f;
        Debug.Log($"[IsStopped] {stopped} (Velocity: {rigidbody.velocity.magnitude}, Angular: {rigidbody.angularVelocity.magnitude})");
        return stopped;
    }

    private void ReturnCameraToPlayer()
    {
        Debug.Log("[ReturnCameraToPlayer] Returning camera to player.");
        cam.transform.parent = player;
        cam.transform.position = player.position + new Vector3(0, 5, -10);
        cam.transform.LookAt(player.position);
    }

    public void ResetDice()
    {
        rigidbody.isKinematic = true;
        firstThrow = false;
        isLanded = false;
        difeFaceNum = "?"; // Сброс значения
        transform.position = startPosition;
        RandomizeRotation();
    }
}

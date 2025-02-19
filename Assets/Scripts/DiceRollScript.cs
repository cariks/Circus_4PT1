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

    [SerializeField] private Button rollButton;

    private void Awake()
    {
        Initialize();
        rollButton.onClick.AddListener(RollDice);
        isLanded = false;
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
    }

    public void RollDice()
    {
        if (!isLanded) return;

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

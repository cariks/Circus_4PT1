using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DiceCameraFollow : MonoBehaviour
{
    public Transform diceTransform; // Ссылка на кубик
    public Transform defaultCameraPosition; // Стандартное положение камеры
    public float followSpeed = 5f;
    public float returnSpeed = 3f;

    private bool isFollowingDice = false;
    private Vector3 initialOffset;

    private void Start()
    {
        if (diceTransform != null)
        {
            initialOffset = transform.position - diceTransform.position;
        }
    }

    private void LateUpdate()
    {
        if (isFollowingDice && diceTransform != null)
        {
            // Следуем за кубиком
            Vector3 targetPosition = diceTransform.position + initialOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void StartFollowingDice()
    {
        isFollowingDice = true;
    }

    public void StopFollowingDice()
    {
        StartCoroutine(ReturnToDefaultPosition());
    }

    private IEnumerator ReturnToDefaultPosition()
    {
        isFollowingDice = false;
        while (Vector3.Distance(transform.position, defaultCameraPosition.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, defaultCameraPosition.position, returnSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

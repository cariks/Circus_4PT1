using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    public string walkingAnimationName;

    public bool isWalking = false; // Добавим переменную, чтобы контролировать анимацию ходьбы

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Используем переменную isWalking для управления анимацией
        animator.SetBool(walkingAnimationName, isWalking);
    }
}

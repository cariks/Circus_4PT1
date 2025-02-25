using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int currentTileIndex = 0; // Индекс текущей клетки
    public int playerIndex; // Индекс игрока
    public bool isBot = false; // Флаг, является ли игрок ботом
    public string playerName; // Имя игрока
    public int diceRolls = 0; // Количество бросков кубика

    public bool isWalking = false; // Для анимации ходьбы

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
}

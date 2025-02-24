using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int currentTileIndex = 0; // Индекс текущей клетки
    public int playerIndex; // Индекс игрока
    public bool isBot = false; // Флаг, который будет указывать, является ли игрок ботом
}
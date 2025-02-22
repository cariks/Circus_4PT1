using UnityEngine;

public class Tile : MonoBehaviour
{
    public int index; // Номер клетки
    public bool isBadTile; // Является ли клетка плохой
    public int penaltySteps; // Если плохая, сколько шагов назад отправляет
}
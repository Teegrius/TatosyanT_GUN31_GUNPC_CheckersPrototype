using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float MoveAnimationDuration = 0.5f;
    public float UnitHeight = 0.5f;
    public Color HighlightColor = Color.yellow;
    public Color AvailableMoveColor = Color.blue;
    public Color AttackMoveColor = Color.red;
    public float CellSpacing = 1.1f;
    public int BoardSize = 8;
    public float RestartHoldDuration = 2f;
}
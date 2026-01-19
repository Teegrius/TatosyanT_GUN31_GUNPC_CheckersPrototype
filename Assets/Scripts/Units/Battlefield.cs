using UnityEngine;
using System.Collections.Generic;

public class Battlefield : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _whiteUnitPrefab;
    [SerializeField] private GameObject _blackUnitPrefab;
    [SerializeField] private float _cellSpacing = 1.1f;
    [SerializeField] private Transform _boardParent;

    private const int BOARD_SIZE = 8;
    private Cell[,] _cells;
    private List<Unit> _units = new List<Unit>();

    void Start()
    {
        GenerateBoard();
        SetupCheckersPieces();
    }

    private void GenerateBoard()
    {
        _cells = new Cell[BOARD_SIZE, BOARD_SIZE];

        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                Vector3 position = new Vector3(x * _cellSpacing, 0, y * _cellSpacing);
                GameObject cellObj = Instantiate(_cellPrefab, position, Quaternion.identity, _boardParent);

                Cell cell = cellObj.GetComponent<Cell>();
                bool isBlackCell = (x + y) % 2 == 1; // Переименовано
                cell.Initialize(new Vector2Int(x, y), isBlackCell);

                _cells[x, y] = cell;
            }
        }
    }

    private void SetupCheckersPieces()
    {
        // Белые шашки (нижние 3 ряда, y = 0-2)
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                if (_cells[x, y].IsBlack)
                {
                    CreateChecker(x, y, Team.White);
                }
            }
        }

        // Черные шашки (верхние 3 ряда, y = 5-7)
        for (int y = 5; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                if (_cells[x, y].IsBlack)
                {
                    CreateChecker(x, y, Team.Black);
                }
            }
        }
    }

    private void CreateChecker(int x, int y, Team team)
    {
        GameObject prefab = team == Team.White ? _whiteUnitPrefab : _blackUnitPrefab;

        // Получаем клетку
        Cell targetCell = _cells[x, y];

        // Позиция клетки
        Vector3 cellPosition = targetCell.transform.position;

        // Шашка должна быть над центром клетки
        // Если клетка в позиции (x * spacing, 0, y * spacing)
        // То шашка должна быть в той же X,Z позиции, но с высотой
        Vector3 unitPosition = new Vector3(
            cellPosition.x,
            0.5f,  // Высота над клеткой
            cellPosition.z
        );

        Debug.Log($"Creating {team} checker at cell [{x},{y}] " +
                  $"Cell pos: {cellPosition}, Unit pos: {unitPosition}");

        GameObject unitObj = Instantiate(prefab, unitPosition, Quaternion.identity, _boardParent);
        Unit unit = unitObj.GetComponent<Unit>();

        // Инициализируем с правильной позицией
        unit.Initialize(team, targetCell);

        // Принудительно устанавливаем позицию
        unit.transform.position = unitPosition;

        _units.Add(unit);
    }

    public Cell GetCell(Vector2Int position)
    {
        if (position.x >= 0 && position.x < BOARD_SIZE &&
            position.y >= 0 && position.y < BOARD_SIZE)
            return _cells[position.x, position.y];

        return null;
    }

    public Cell GetCell(int x, int y)
    {
        return GetCell(new Vector2Int(x, y));
    }
}
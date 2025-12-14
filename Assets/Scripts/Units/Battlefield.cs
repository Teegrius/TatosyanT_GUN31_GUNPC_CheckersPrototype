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
                bool isBlack = (x + y) % 2 == 1; // Только черные клетки для шашек
                cell.Initialize(new Vector2Int(x, y), isBlack);

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
        GameObject unitObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, _boardParent);
        Unit unit = unitObj.GetComponent<Unit>();
        unit.Initialize(team, _cells[x, y]);
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
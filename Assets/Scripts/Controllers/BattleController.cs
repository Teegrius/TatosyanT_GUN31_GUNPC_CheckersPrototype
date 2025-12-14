using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance { get; private set; }

    [Inject] private Battlefield _battlefield;
    [Inject] private PlayerController _playerController;
    [Inject] private Unit.Factory _unitFactory;

    private Cell _selectedCell;
    private Unit _selectedUnit;
    private Team _currentPlayer;
    private GameState _gameState = GameState.PlayerTurn;
    private List<Cell> _availableMoves = new List<Cell>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Случайный первый ход
        _currentPlayer = Random.Range(0, 2) == 0 ? Team.White : Team.Black;
        Debug.Log($"First player: {_currentPlayer}");
    }

    private void Update()
    {
        if (_gameState != GameState.PlayerTurn) return;

        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSelection();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmAction();
        }
    }

    public void OnCellSelected(Cell cell)
    {
        if (_gameState != GameState.PlayerTurn) return;

        Debug.Log($"Cell selected: {cell.BoardPosition}");

        if (_selectedUnit == null)
        {
            if (cell.CurrentUnit != null && cell.CurrentUnit.Team == _currentPlayer)
            {
                _selectedUnit = cell.CurrentUnit;
                _selectedCell = cell;
                HighlightAvailableMoves(_selectedUnit);
            }
        }
        else
        {
            TryMoveUnit(_selectedUnit, cell);
        }
    }

    public void OnUnitSelected(Unit unit)
    {
        if (_gameState != GameState.PlayerTurn || unit.Team != _currentPlayer) return;

        _selectedUnit = unit;
        _selectedCell = unit.CurrentCell;
        HighlightAvailableMoves(_selectedUnit);
    }

    private void HighlightAvailableMoves(Unit unit)
    {
        ClearHighlights();
        _availableMoves = GetAvailableMoves(unit);

        foreach (var cell in _availableMoves)
        {
            cell.Highlight(Color.blue);
        }
    }

    private List<Cell> GetAvailableMoves(Unit unit)
    {
        List<Cell> moves = new List<Cell>();
        Vector2Int pos = unit.CurrentCell.BoardPosition;

        if (unit.PieceType == PieceType.Pawn)
        {
            // Для обычной шашки
            int direction = (unit.Team == Team.White) ? 1 : -1;

            // Простые ходы вперед
            CheckDiagonalMove(pos, new Vector2Int(1, direction), unit, moves);
            CheckDiagonalMove(pos, new Vector2Int(-1, direction), unit, moves);

            // Ходы с взятием
            CheckCaptureMove(pos, new Vector2Int(2, 2 * direction), new Vector2Int(1, direction), unit, moves);
            CheckCaptureMove(pos, new Vector2Int(-2, 2 * direction), new Vector2Int(-1, direction), unit, moves);
        }
        else
        {
            // Для дамки все 4 направления
            Vector2Int[] directions = {
                new Vector2Int(1, 1), new Vector2Int(-1, 1),
                new Vector2Int(1, -1), new Vector2Int(-1, -1)
            };

            foreach (var dir in directions)
            {
                CheckDiagonalMove(pos, dir, unit, moves);
                CheckCaptureMove(pos, dir * 2, dir, unit, moves);
            }
        }

        return moves;
    }

    private void CheckDiagonalMove(Vector2Int startPos, Vector2Int direction, Unit unit, List<Cell> moves)
    {
        Vector2Int targetPos = startPos + direction;
        Cell targetCell = _battlefield.GetCell(targetPos);

        if (targetCell != null && targetCell.CurrentUnit == null && targetCell.IsBlack)
        {
            moves.Add(targetCell);
        }
    }

    private void CheckCaptureMove(Vector2Int startPos, Vector2Int targetDir, Vector2Int enemyDir, Unit unit, List<Cell> moves)
    {
        Vector2Int targetPos = startPos + targetDir;
        Vector2Int enemyPos = startPos + enemyDir;

        Cell enemyCell = _battlefield.GetCell(enemyPos);
        Cell targetCell = _battlefield.GetCell(targetPos);

        if (enemyCell != null && enemyCell.CurrentUnit != null &&
            enemyCell.CurrentUnit.Team != unit.Team &&
            targetCell != null && targetCell.CurrentUnit == null && targetCell.IsBlack)
        {
            moves.Add(targetCell);
        }
    }

    private void TryMoveUnit(Unit unit, Cell targetCell)
    {
        if (!_availableMoves.Contains(targetCell)) return;

        _gameState = GameState.Animation;

        // Проверяем, является ли ход взятием
        bool isCapture = IsCaptureMove(unit.CurrentCell.BoardPosition, targetCell.BoardPosition);

        if (isCapture)
        {
            // Удаляем взятую фигуру
            Cell capturedCell = GetCellBetween(unit.CurrentCell, targetCell);
            if (capturedCell != null && capturedCell.CurrentUnit != null)
            {
                Destroy(capturedCell.CurrentUnit.gameObject);
                capturedCell.CurrentUnit = null;
            }
        }

        // Выполняем ход
        StartCoroutine(_playerController.VisualizeMoveCoroutine(unit, targetCell, () =>
        {
            // Проверяем превращение в дамку
            CheckForKing(unit, targetCell);

            // Передаем ход
            _currentPlayer = (_currentPlayer == Team.White) ? Team.Black : Team.White;
            _gameState = GameState.PlayerTurn;

            ClearHighlights();
            _selectedUnit = null;
            _selectedCell = null;

            Debug.Log($"Now player: {_currentPlayer}");
        }));
    }

    private bool IsCaptureMove(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) == 2 && Mathf.Abs(from.y - to.y) == 2;
    }

    private Cell GetCellBetween(Cell from, Cell to)
    {
        Vector2Int fromPos = from.BoardPosition;
        Vector2Int toPos = to.BoardPosition;
        Vector2Int middlePos = new Vector2Int(
            (fromPos.x + toPos.x) / 2,
            (fromPos.y + toPos.y) / 2
        );

        return _battlefield.GetCell(middlePos);
    }

    private void CheckForKing(Unit unit, Cell newCell)
    {
        int lastRow = (unit.Team == Team.White) ? 7 : 0;

        if (newCell.BoardPosition.y == lastRow && unit.PieceType != PieceType.King)
        {
            unit.MakeKing();
        }
    }

    public void CancelSelection()
    {
        ClearHighlights();
        _selectedUnit = null;
        _selectedCell = null;
    }

    private void ClearHighlights()
    {
        foreach (var cell in _availableMoves)
        {
            cell.Unhighlight();
        }
        _availableMoves.Clear();
    }

    public void ConfirmAction()
    {
        Debug.Log("Confirm action pressed");
    }
}
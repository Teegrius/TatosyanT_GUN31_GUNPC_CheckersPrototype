using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance { get; private set; }

    [Inject] private Battlefield _battlefield;
    [Inject] private PlayerController _playerController;
    [Inject] private UIController _uiController;
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

        _currentPlayer = Random.Range(0, 2) == 0 ? Team.White : Team.Black;
        Debug.Log($"First player: {_currentPlayer}");

        if (_uiController != null)
            _uiController.UpdateTurnDisplay(_currentPlayer);
    }

    private void Update()
    {
        if (_gameState != GameState.PlayerTurn) return;

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
            int direction = (unit.Team == Team.White) ? 1 : -1;

            // Обычные ходы вперед
            Vector2Int forwardLeft = new Vector2Int(pos.x - 1, pos.y + direction);
            Vector2Int forwardRight = new Vector2Int(pos.x + 1, pos.y + direction);

            CheckMove(forwardLeft, unit, moves);
            CheckMove(forwardRight, unit, moves);

            // Атаки
            Vector2Int attackLeft = new Vector2Int(pos.x - 2, pos.y + 2 * direction);
            Vector2Int attackRight = new Vector2Int(pos.x + 2, pos.y + 2 * direction);

            CheckAttack(pos, attackLeft, new Vector2Int(-1, direction), unit, moves);
            CheckAttack(pos, attackRight, new Vector2Int(1, direction), unit, moves);
        }
        else
        {
            // Для дамки - УПРОЩЕННАЯ ВЕРСИЯ 
            CheckKingMovesSimple(pos, unit, moves);
        }

        return moves;
    }

    private void CheckMove(Vector2Int targetPos, Unit unit, List<Cell> moves)
    {
        Cell targetCell = _battlefield.GetCell(targetPos);
        if (targetCell != null && targetCell.CurrentUnit == null && targetCell.IsBlack)
        {
            moves.Add(targetCell);
        }
    }

    private void CheckAttack(Vector2Int startPos, Vector2Int targetPos, Vector2Int enemyDir, Unit unit, List<Cell> moves)
    {
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

    // УПРОЩЕННАЯ ВЕРСИЯ - без бесконечного цикла
    private void CheckKingMovesSimple(Vector2Int startPos, Unit unit, List<Cell> moves)
    {
        // Дамка может ходить на 1 клетку в любом диагональном направлении
        // (позже можно расширить до полной логики)

        Vector2Int[] directions = {
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        foreach (var dir in directions)
        {
            Vector2Int targetPos = startPos + dir;
            Cell targetCell = _battlefield.GetCell(targetPos);

            if (targetCell != null && targetCell.CurrentUnit == null && targetCell.IsBlack)
            {
                moves.Add(targetCell);
            }

            // Также проверяем возможность атаки на 2 клетки
            Vector2Int attackPos = startPos + dir * 2;
            Vector2Int enemyPos = startPos + dir;

            Cell enemyCell = _battlefield.GetCell(enemyPos);
            Cell attackCell = _battlefield.GetCell(attackPos);

            if (enemyCell != null && enemyCell.CurrentUnit != null &&
                enemyCell.CurrentUnit.Team != unit.Team &&
                attackCell != null && attackCell.CurrentUnit == null && attackCell.IsBlack)
            {
                moves.Add(attackCell);
            }
        }
    }

    private void TryMoveUnit(Unit unit, Cell targetCell)
    {
        if (!_availableMoves.Contains(targetCell)) return;

        _gameState = GameState.Animation;

        bool isCapture = Mathf.Abs(unit.CurrentCell.BoardPosition.x - targetCell.BoardPosition.x) == 2;

        if (isCapture)
        {
            Vector2Int fromPos = unit.CurrentCell.BoardPosition;
            Vector2Int toPos = targetCell.BoardPosition;
            Vector2Int middlePos = new Vector2Int((fromPos.x + toPos.x) / 2, (fromPos.y + toPos.y) / 2);

            Cell capturedCell = _battlefield.GetCell(middlePos);
            if (capturedCell != null && capturedCell.CurrentUnit != null)
            {
                Destroy(capturedCell.CurrentUnit.gameObject);
                capturedCell.CurrentUnit = null;
            }
        }

        StartCoroutine(_playerController.VisualizeMoveCoroutine(unit, targetCell, () =>
        {
            // Обновляем позицию фигуры
            if (unit.CurrentCell != null)
                unit.CurrentCell.CurrentUnit = null;

            unit.CurrentCell = targetCell;
            targetCell.CurrentUnit = unit;

            // Проверка на дамку
            int lastRow = (unit.Team == Team.White) ? 7 : 0;
            if (targetCell.BoardPosition.y == lastRow && unit.PieceType != PieceType.King)
            {
                unit.MakeKing();
            }

            // Смена хода
            _currentPlayer = (_currentPlayer == Team.White) ? Team.Black : Team.White;
            _gameState = GameState.PlayerTurn;

            if (_uiController != null)
                _uiController.UpdateTurnDisplay(_currentPlayer);

            ClearHighlights();
            _selectedUnit = null;
            _selectedCell = null;

            Debug.Log($"Now player: {_currentPlayer}");
        }));
    }

    public void CancelSelection()
    {
        ClearHighlights();
        _selectedUnit = null;
        _selectedCell = null;
    }

    public void ConfirmAction()
    {
        Debug.Log("Confirm action pressed");
    }

    private void ClearHighlights()
    {
        foreach (var cell in _availableMoves)
        {
            cell.Unhighlight();
        }
        _availableMoves.Clear();
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ForcedAttackState : IGameState
{
    [Inject] private Battlefield _battlefield;
    [Inject] private AttackChecker _attackChecker;
    [Inject] private PlayerController _playerController;

    private Team _currentPlayer;
    private Unit _selectedUnit;
    private List<Cell> _forcedAttackMoves = new List<Cell>();
    private Dictionary<Unit, List<Cell>> _allForcedAttacks;

    public void Enter()
    {
        Debug.Log("ForcedAttackState: Player must attack!");
        _allForcedAttacks = _attackChecker.GetForcedAttackMoves(_currentPlayer);
    }

    public void Exit()
    {
        ClearHighlights();
        _selectedUnit = null;
    }

    public void Update() { }

    public void OnCellSelected(Cell cell)
    {
        if (cell.CurrentUnit != null && cell.CurrentUnit.Team == _currentPlayer)
        {
            if (_allForcedAttacks.ContainsKey(cell.CurrentUnit))
            {
                SelectUnit(cell.CurrentUnit);
            }
            else
            {
                Debug.Log("You must select a piece that can attack!");
            }
        }
        else if (_selectedUnit != null && cell.CurrentUnit == null)
        {
            if (_forcedAttackMoves.Contains(cell))
            {
                ExecuteAttack(cell);
            }
        }
    }

    public void OnUnitSelected(Unit unit)
    {
        if (unit.Team == _currentPlayer && _allForcedAttacks.ContainsKey(unit))
        {
            SelectUnit(unit);
        }
    }

    public void SetCurrentPlayer(Team player)
    {
        _currentPlayer = player;
    }

    private void SelectUnit(Unit unit)
    {
        ClearHighlights();
        _selectedUnit = unit;
        _forcedAttackMoves = _allForcedAttacks[unit];

        foreach (var cell in _forcedAttackMoves)
        {
            cell.Highlight(Color.red);
        }
    }

    private void ExecuteAttack(Cell targetCell)
    {
        Debug.Log($"Executing forced attack to {targetCell.BoardPosition}");
        // Реализация атаки будет позже
    }

    private void ClearHighlights()
    {
        foreach (var cell in _forcedAttackMoves)
        {
            cell.Unhighlight();
        }
        _forcedAttackMoves.Clear();
    }
}
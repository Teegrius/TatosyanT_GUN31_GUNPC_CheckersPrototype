using System.Collections.Generic;
using UnityEngine;

public class AttackChecker
{
    private Battlefield _battlefield;

    public AttackChecker(Battlefield battlefield)
    {
        _battlefield = battlefield;
    }

    public bool HasForcedAttacks(Team team)
    {
        var allUnits = GetAllUnits(team);

        foreach (var unit in allUnits)
        {
            var attackMoves = GetAttackMovesForUnit(unit);
            if (attackMoves.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    public Dictionary<Unit, List<Cell>> GetForcedAttackMoves(Team team)
    {
        var result = new Dictionary<Unit, List<Cell>>();
        var allUnits = GetAllUnits(team);

        foreach (var unit in allUnits)
        {
            var attackMoves = GetAttackMovesForUnit(unit);
            if (attackMoves.Count > 0)
            {
                result[unit] = attackMoves;
            }
        }

        return result;
    }

    private List<Cell> GetAttackMovesForUnit(Unit unit)
    {
        var attackMoves = new List<Cell>();
        Vector2Int pos = unit.CurrentCell.BoardPosition;

        if (unit.PieceType == PieceType.Pawn)
        {
            int direction = (unit.Team == Team.White) ? 1 : -1;

            CheckPawnAttack(pos, new Vector2Int(2, 2 * direction),
                          new Vector2Int(1, direction), unit, attackMoves);
            CheckPawnAttack(pos, new Vector2Int(-2, 2 * direction),
                          new Vector2Int(-1, direction), unit, attackMoves);
        }
        else
        {
            CheckKingAttacks(pos, unit, attackMoves);
        }

        return attackMoves;
    }

    private void CheckPawnAttack(Vector2Int startPos, Vector2Int targetDir,
                                Vector2Int enemyDir, Unit unit, List<Cell> moves)
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

    private void CheckKingAttacks(Vector2Int startPos, Unit unit, List<Cell> moves)
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        foreach (var dir in directions)
        {
            CheckKingAttackDirection(startPos, dir, unit, moves);
        }
    }

    private void CheckKingAttackDirection(Vector2Int startPos, Vector2Int direction,
                                         Unit unit, List<Cell> moves)
    {
        Vector2Int currentPos = startPos + direction;

        while (true)
        {
            Cell cell = _battlefield.GetCell(currentPos);
            if (cell == null || !cell.IsBlack) break;

            if (cell.CurrentUnit == null)
            {
                currentPos += direction;
                continue;
            }

            if (cell.CurrentUnit.Team != unit.Team)
            {
                Vector2Int jumpPos = currentPos + direction;
                Cell jumpCell = _battlefield.GetCell(jumpPos);
                if (jumpCell != null && jumpCell.CurrentUnit == null && jumpCell.IsBlack)
                {
                    moves.Add(jumpCell);
                }
            }
            break;
        }
    }

    private List<Unit> GetAllUnits(Team team)
    {
        var units = new List<Unit>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Cell cell = _battlefield.GetCell(x, y);
                if (cell != null && cell.CurrentUnit != null &&
                    cell.CurrentUnit.Team == team)
                {
                    units.Add(cell.CurrentUnit);
                }
            }
        }

        return units;
    }
}
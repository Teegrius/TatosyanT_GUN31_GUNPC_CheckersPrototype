using System.Collections.Generic;
using UnityEngine;

public class MultiJumpChecker
{
    private Battlefield _battlefield;

    public MultiJumpChecker(Battlefield battlefield)
    {
        _battlefield = battlefield;
    }

    public List<Cell> GetAdditionalJumps(Unit unit, Cell fromCell)
    {
        var additionalJumps = new List<Cell>();
        Vector2Int pos = fromCell.BoardPosition;

        if (unit.PieceType == PieceType.Pawn)
        {
            int direction = (unit.Team == Team.White) ? 1 : -1;

            CheckPawnJump(pos, new Vector2Int(2, 2 * direction),
                         new Vector2Int(1, direction), additionalJumps);
            CheckPawnJump(pos, new Vector2Int(-2, 2 * direction),
                         new Vector2Int(-1, direction), additionalJumps);

            if (unit.Team == Team.White)
            {
                CheckPawnJump(pos, new Vector2Int(2, -2 * direction),
                            new Vector2Int(1, -direction), additionalJumps);
                CheckPawnJump(pos, new Vector2Int(-2, -2 * direction),
                            new Vector2Int(-1, -direction), additionalJumps);
            }
        }
        else
        {
            CheckKingJumps(pos, additionalJumps);
        }

        return additionalJumps;
    }

    private void CheckPawnJump(Vector2Int startPos, Vector2Int targetDir,
                              Vector2Int enemyDir, List<Cell> moves)
    {
        Vector2Int targetPos = startPos + targetDir;
        Vector2Int enemyPos = startPos + enemyDir;

        Cell enemyCell = _battlefield.GetCell(enemyPos);
        Cell targetCell = _battlefield.GetCell(targetPos);

        if (enemyCell != null && enemyCell.CurrentUnit != null &&
            targetCell != null && targetCell.CurrentUnit == null && targetCell.IsBlack)
        {
            moves.Add(targetCell);
        }
    }

    private void CheckKingJumps(Vector2Int startPos, List<Cell> moves)
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        foreach (var dir in directions)
        {
            CheckKingJumpDirection(startPos, dir, moves);
        }
    }

    private void CheckKingJumpDirection(Vector2Int startPos, Vector2Int direction,
                                       List<Cell> moves)
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

            Vector2Int jumpPos = currentPos + direction;
            Cell jumpCell = _battlefield.GetCell(jumpPos);
            if (jumpCell != null && jumpCell.CurrentUnit == null && jumpCell.IsBlack)
            {
                moves.Add(jumpCell);
            }
            break;
        }
    }
}
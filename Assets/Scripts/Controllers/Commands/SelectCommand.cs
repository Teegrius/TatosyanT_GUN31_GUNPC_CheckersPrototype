using UnityEngine;

public class SelectCommand : IGameplayCommand
{
    private BattleController _battleController;

    public SelectCommand(BattleController battleController)
    {
        _battleController = battleController;
    }

    public void Interact(Cell cell)
    {
        if (cell.CurrentUnit != null)
        {
            _battleController.OnUnitSelected(cell.CurrentUnit);
        }
        else
        {
            _battleController.OnCellSelected(cell);
        }
    }
}
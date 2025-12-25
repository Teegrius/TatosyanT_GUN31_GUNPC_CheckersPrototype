public interface IGameState
{
    void Enter();
    void Exit();
    void Update();
    void OnCellSelected(Cell cell);
    void OnUnitSelected(Unit unit);
}
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private MeshRenderer _meshRenderer;

    private Team _team;
    private PieceType _pieceType = PieceType.Pawn;
    private Color _originalColor;

    public Team Team => _team;
    public PieceType PieceType => _pieceType;
    public Cell CurrentCell { get; set; }

    private void Start()
    {
        if (_meshRenderer != null)
            _originalColor = _meshRenderer.material.color;
    }

    public void Initialize(Team team, Cell cell)
    {
        _team = team;
        CurrentCell = cell;
        cell.CurrentUnit = this;

        if (_meshRenderer != null)
        {
            _meshRenderer.material.color = (team == Team.White) ? Color.white : Color.gray;
            _originalColor = _meshRenderer.material.color;
        }

        // Устанавливаем позицию точно над клеткой
        if (cell != null)
        {
            Vector3 cellPos = cell.transform.position;
            transform.position = new Vector3(cellPos.x, 0.5f, cellPos.z);
        }

        name = $"Unit_{team}_{cell.BoardPosition.x}_{cell.BoardPosition.y}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(Color.yellow);
        if (CurrentCell != null)
            CurrentCell.Highlight(Color.green);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unhighlight();
        if (CurrentCell != null)
            CurrentCell.Unhighlight();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Unit clicked: {_team} {_pieceType}");

        BattleController battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            battleController.OnUnitSelected(this);
        }
    }

    public void MoveToCell(Cell targetCell)
    {
        if (CurrentCell != null)
            CurrentCell.CurrentUnit = null;

        CurrentCell = targetCell;
        CurrentCell.CurrentUnit = this;
        transform.position = targetCell.transform.position + Vector3.up * 0.5f;
    }

    public void Highlight(Color color)
    {
        if (_meshRenderer != null)
            _meshRenderer.material.color = color;
    }

    public void Unhighlight()
    {
        if (_meshRenderer != null)
            _meshRenderer.material.color = _originalColor;
    }

    public void MakeKing()
    {
        _pieceType = PieceType.King;
        transform.localScale = Vector3.one * 1.2f;
        Debug.Log($"Unit became KING at {CurrentCell.BoardPosition}");
    }

    public class Factory : PlaceholderFactory<Team, Unit> { }
}
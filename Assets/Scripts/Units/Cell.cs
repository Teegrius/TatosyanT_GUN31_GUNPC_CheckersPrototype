using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private MeshRenderer _meshRenderer;

    private Vector2Int _boardPosition;
    private Color _originalColor;

    public Vector2Int BoardPosition => _boardPosition;
    public Unit CurrentUnit { get; set; }
    public bool IsBlack { get; private set; }

    private void Start()
    {
        if (_meshRenderer != null)
            _originalColor = _meshRenderer.material.color;
    }

    public void Initialize(Vector2Int position, bool isBlack)
    {
        _boardPosition = position;
        IsBlack = isBlack;
        name = $"Cell_{position.x}_{position.y}";

        if (_meshRenderer != null)
        {
            _meshRenderer.material.color = isBlack ? Color.black : Color.white;
            _originalColor = _meshRenderer.material.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(Color.yellow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unhighlight();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Cell clicked at position: {_boardPosition}");

        // »щем BattleController
        BattleController battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            battleController.OnCellSelected(this);
        }
        else
        {
            Debug.LogError("BattleController not found!");
        }
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

    public void SetHighlight(bool highlight)
    {
        if (_meshRenderer != null)
            _meshRenderer.material.color = highlight ? Color.green : _originalColor;
    }

    public bool IsEmpty => CurrentUnit == null;

    public class Factory : PlaceholderFactory<Cell> { }
}
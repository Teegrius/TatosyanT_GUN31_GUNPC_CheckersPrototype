using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private Image _whiteIndicator;
    [SerializeField] private Image _blackIndicator;

    public void UpdateTurn(Team currentTeam)
    {
        if (_turnText != null)
            _turnText.text = $"Turn: {currentTeam}";

        if (_whiteIndicator != null && _blackIndicator != null)
        {
            if (currentTeam == Team.White)
            {
                _whiteIndicator.color = Color.green;
                _blackIndicator.color = Color.gray;
            }
            else
            {
                _whiteIndicator.color = Color.white;
                _blackIndicator.color = Color.green;
            }
        }
    }
}
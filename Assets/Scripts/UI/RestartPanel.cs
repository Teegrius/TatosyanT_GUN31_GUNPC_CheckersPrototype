using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestartPanel : MonoBehaviour
{
    [SerializeField] private GameObject _restartPanel;
    [SerializeField] private Image _progressFill;
    [SerializeField] private TextMeshProUGUI _restartText;

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        if (_restartPanel != null)
            _restartPanel.SetActive(true);
    }

    public void Hide()
    {
        if (_restartPanel != null)
        {
            _restartPanel.SetActive(false);
            if (_progressFill != null)
                _progressFill.fillAmount = 0f;
        }
    }

    public void UpdateProgress(float progress)
    {
        if (_progressFill != null)
            _progressFill.fillAmount = progress;

        if (_restartText != null)
        {
            if (progress >= 1f)
            {
                _restartText.text = "Restarting...";
            }
            else if (progress > 0f)
            {
                _restartText.text = $"Hold TAB: {Mathf.RoundToInt(progress * 100)}%";
            }
        }
    }
}
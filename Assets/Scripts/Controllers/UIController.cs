using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TurnPanel _turnPanel;
    [SerializeField] private RestartPanel _restartPanel;
    [SerializeField] private GameSettings _settings;

    private float _restartHoldTime = 0f;
    private bool _isRestarting = false;

    public void UpdateTurnDisplay(Team currentTeam)
    {
        if (_turnPanel != null)
            _turnPanel.UpdateTurn(currentTeam);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && !_isRestarting)
        {
            _isRestarting = true;
            if (_restartPanel != null)
                _restartPanel.Show();

            _restartHoldTime += Time.deltaTime;

            float progress = Mathf.Clamp01(_restartHoldTime / _settings.RestartHoldDuration);
            if (_restartPanel != null)
                _restartPanel.UpdateProgress(progress);

            if (progress >= 1f)
            {
                RestartGame();
            }
        }
        else if (_isRestarting)
        {
            _isRestarting = false;
            _restartHoldTime = 0f;
            if (_restartPanel != null)
                _restartPanel.Hide();
        }
    }

    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
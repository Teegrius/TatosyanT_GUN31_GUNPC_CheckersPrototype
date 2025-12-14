using UnityEngine;

public class InputController : MonoBehaviour
{
    public bool IsRestartPressed()
    {
        return Input.GetKey(KeyCode.Tab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && BattleController.Instance != null)
        {
            BattleController.Instance.CancelSelection();
        }

        if (Input.GetKeyDown(KeyCode.Space) && BattleController.Instance != null)
        {
            BattleController.Instance.ConfirmAction();
        }
    }
}
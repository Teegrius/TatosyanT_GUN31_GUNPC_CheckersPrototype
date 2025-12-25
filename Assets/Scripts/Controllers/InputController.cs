using UnityEngine;

public class InputController : MonoBehaviour
{
    public bool IsRestartPressed()
    {
        return Input.GetKey(KeyCode.Tab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (BattleController.Instance != null)
            {
                BattleController.Instance.CancelSelection();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BattleController.Instance != null)
            {
                BattleController.Instance.ConfirmAction();
            }
        }
    }
}
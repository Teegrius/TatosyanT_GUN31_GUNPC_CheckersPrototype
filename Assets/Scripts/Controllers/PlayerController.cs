using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    [Inject] private Battlefield _battlefield;

    private bool _inputLocked = false;

    public void LockInput()
    {
        _inputLocked = true;
    }

    public void UnlockInput()
    {
        _inputLocked = false;
    }

    public bool IsInputLocked => _inputLocked;

    public IEnumerator VisualizeMoveCoroutine(Unit unit, Cell targetCell, Action onComplete)
    {
        LockInput();

        // Анимация движения
        Vector3 startPos = unit.transform.position;
        Vector3 endPos = targetCell.transform.position + Vector3.up * 0.5f;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            unit.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Обновляем логику клеток
        if (unit.CurrentCell != null)
            unit.CurrentCell.CurrentUnit = null;

        unit.CurrentCell = targetCell;
        targetCell.CurrentUnit = unit;
        unit.transform.position = endPos;

        UnlockInput();
        onComplete?.Invoke();
    }

    private void OnMoveComplete()
    {
        UnlockInput();
    }
}
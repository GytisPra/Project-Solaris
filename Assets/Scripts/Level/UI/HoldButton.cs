using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action OnHoldAction;

    [SerializeField] private float initialDelay = 0.3f;
    [SerializeField] private float repeatRate = 0.05f;

    private bool isHolding;
    private Coroutine holdCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdCoroutine = StartCoroutine(HoldRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
        }
    }

    private IEnumerator HoldRoutine()
    {
        yield return new WaitForSeconds(initialDelay); // Wait before repeating

        while (isHolding)
        {
            OnHoldAction?.Invoke();
            yield return new WaitForSeconds(repeatRate);
        }
    }
}

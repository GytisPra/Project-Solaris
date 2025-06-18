using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickTouchDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsTouched { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsTouched = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouched = false;
    }
}

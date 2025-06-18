using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchGestureManager : MonoBehaviour
{
    public static TouchGestureManager Instance { get; private set; }

    public event Action<float> OnPinch; 
    public event Action<bool> OnGestureActive;

    [Header("Gesture Threshold")]
    [Tooltip("Minimum change in finger distance to detect pinch.")]
    public float pinchThreshold = 5f;

    private bool isGestureActive = false;
    private Vector2 prevPos0, prevPos1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        var touches = Touch.activeTouches;
        if (touches.Count != 2)
        {
            EndGesture();
            return;
        }

        var t0 = touches[0];
        var t1 = touches[1];
        Vector2 p0 = t0.screenPosition;
        Vector2 p1 = t1.screenPosition;

        if (!isGestureActive)
        {
            BeginGesture(p0, p1);
            InvokeGestureActive(true);
        }

        float prevDistance = Vector2.Distance(prevPos0, prevPos1);
        float curDistance = Vector2.Distance(p0, p1);
        float distanceDelta = curDistance - prevDistance;

        if (Mathf.Abs(distanceDelta) >= pinchThreshold)
        {
            OnPinch?.Invoke(distanceDelta);
        }

        // Save current positions for next frame
        prevPos0 = p0;
        prevPos1 = p1;
    }

    private void BeginGesture(Vector2 p0, Vector2 p1)
    {
        isGestureActive = true;
        prevPos0 = p0;
        prevPos1 = p1;
    }

    private void EndGesture()
    {
        if (isGestureActive)
        {
            isGestureActive = false;
            InvokeGestureActive(false);
        }
    }

    private void InvokeGestureActive(bool active)
    {
        OnGestureActive?.Invoke(active);
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchGestureManager : MonoBehaviour
{
    public static TouchGestureManager Instance { get; private set; }

    public event Action<float> OnPinch;        // Passes zoom delta
    public event Action<float> OnRotate;       // Passes angle delta
    public event Action<Vector2> OnDrag;       // Passes average drag delta
    public event Action<bool> OnGestureActive; // Passes true if a gesture is being performed

    [Header("Gesture Thresholds")]
    [Tooltip("Minimum change in finger distance to detect pinch.")]
    public float pinchThreshold = 5f;
    [Tooltip("Minimum angle change (degrees) to detect rotation.")]
    public float rotateThreshold = 5f;
    [Tooltip("Minimum average movement (pixels) to detect drag.")]
    public float dragThreshold = 2f;

    private enum GestureType { None, Pinch, Rotate, Drag }
    private GestureType activeGesture = GestureType.None;

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

    /// <summary>
    ///  With zoom
    /// </summary>
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
        Vector2 d0 = t0.delta;
        Vector2 d1 = t1.delta;

        if (!isGestureActive)
        {
            BeginGesture(p0, p1);
            InvokeGestureActive(true);
        }

        // Compute metric deltas
        float prevDistance = Vector2.Distance(prevPos0, prevPos1);
        float curDistance = Vector2.Distance(p0, p1);
        float distanceDelta = curDistance - prevDistance;

        float prevAngle = Mathf.Atan2(prevPos1.y - prevPos0.y, prevPos1.x - prevPos0.x) * Mathf.Rad2Deg;
        float curAngle = Mathf.Atan2(p1.y - p0.y, p1.x - p0.x) * Mathf.Rad2Deg;
        float angleDelta = Mathf.DeltaAngle(prevAngle, curAngle);

        Vector2 avgDelta = (d0 + d1) * 0.5f;

        // Determine or maintain gesture
        if (activeGesture == GestureType.None)
        {
            if (Mathf.Abs(distanceDelta) >= pinchThreshold)
                activeGesture = GestureType.Pinch;
            else if (Mathf.Abs(angleDelta) >= rotateThreshold)
                activeGesture = GestureType.Rotate;
            else if (avgDelta.magnitude >= dragThreshold)
                activeGesture = GestureType.Drag;
        }

        // Fire the active gesture event only
        switch (activeGesture)
        {
            case GestureType.Pinch:
                OnPinch?.Invoke(distanceDelta);
                break;
            case GestureType.Rotate:
                OnRotate?.Invoke(angleDelta);
                break;
            case GestureType.Drag:
                OnDrag?.Invoke(avgDelta);
                break;
        }

        // Save positions for next frame
        prevPos0 = p0;
        prevPos1 = p1;
    }

    /// <summary>
    ///  With no zoom
    /// </summary>
    //private void Update()
    //{
    //    var touches = Touch.activeTouches;
    //    if (touches.Count != 2)
    //    {
    //        EndGesture();
    //        return;
    //    }

    //    var t0 = touches[0];
    //    var t1 = touches[1];
    //    Vector2 p0 = t0.screenPosition;
    //    Vector2 p1 = t1.screenPosition;
    //    Vector2 d0 = t0.delta;
    //    Vector2 d1 = t1.delta;

    //    if (!isGestureActive)
    //    {
    //        BeginGesture(p0, p1);
    //        InvokeGestureActive(true);
    //    }

    //    // Compute metric deltas
    //    float prevDistance = Vector2.Distance(prevPos0, prevPos1);

    //    float prevAngle = Mathf.Atan2(prevPos1.y - prevPos0.y, prevPos1.x - prevPos0.x) * Mathf.Rad2Deg;
    //    float curAngle = Mathf.Atan2(p1.y - p0.y, p1.x - p0.x) * Mathf.Rad2Deg;
    //    float angleDelta = Mathf.DeltaAngle(prevAngle, curAngle);

    //    Vector2 avgDelta = (d0 + d1) * 0.5f;

    //    // Determine or maintain gesture
    //    if (activeGesture == GestureType.None)
    //    {
    //        if (Mathf.Abs(angleDelta) >= rotateThreshold)
    //            activeGesture = GestureType.Rotate;
    //        else if (avgDelta.magnitude >= dragThreshold)
    //            activeGesture = GestureType.Drag;
    //    }

    //    // Fire the active gesture event only
    //    switch (activeGesture)
    //    {
    //        case GestureType.Rotate:
    //            OnRotate?.Invoke(angleDelta);
    //            break;
    //        case GestureType.Drag:
    //            OnDrag?.Invoke(avgDelta);
    //            break;
    //    }

    //    // Save positions for next frame
    //    prevPos0 = p0;
    //    prevPos1 = p1;
    //}

    private void BeginGesture(Vector2 p0, Vector2 p1)
    {
        isGestureActive = true;
        activeGesture = GestureType.None;
        prevPos0 = p0;
        prevPos1 = p1;
    }

    private void EndGesture()
    {
        if (isGestureActive)
        {
            isGestureActive = false;
            activeGesture = GestureType.None;
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
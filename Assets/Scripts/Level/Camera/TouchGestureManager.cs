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

    private Vector2 prevPos0, prevPos1;
    private float prevMagnitude;
    private float prevAngle;
    private bool isGestureActive = false;
    private bool isPinching = false;
    private bool isRotatingOrDarging = false;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        var touches = Touch.activeTouches;

        if (touches.Count == 2)
        {
            var touch0 = touches[0];
            var touch1 = touches[1];

            Vector2 pos0 = touch0.screenPosition;
            Vector2 pos1 = touch1.screenPosition;

            Vector2 delta0 = touch0.delta;
            Vector2 delta1 = touch1.delta;

            Vector2 currentVector = pos1 - pos0;
            float currentMagnitude = currentVector.magnitude;
            float currentAngle = Mathf.Atan2(currentVector.y, currentVector.x) * Mathf.Rad2Deg;

            if (!isGestureActive)
            {
                isGestureActive = true;
                OnGestureActive?.Invoke(true);

                prevPos0 = pos0;
                prevPos1 = pos1;
                prevMagnitude = (pos1 - pos0).magnitude;
                prevAngle = Mathf.Atan2((pos1 - pos0).y, (pos1 - pos0).x) * Mathf.Rad2Deg;
            }

            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                // Pinch
                float pinchDelta = currentMagnitude - prevMagnitude;
                const float pinchThreshold = 0.1f;
                if (Mathf.Abs(pinchDelta) > pinchThreshold && !isRotatingOrDarging)
                {
                    OnPinch?.Invoke(pinchDelta);
                    isPinching = true;
                }
                else if (!isPinching)
                {
                    // Drag
                    Vector2 avgDelta = (delta0 + delta1) * 0.5f;
                    OnDrag?.Invoke(avgDelta);

                    // Rotate
                    float angleDelta = Mathf.DeltaAngle(prevAngle, currentAngle);
                    OnRotate?.Invoke(angleDelta);

                    isRotatingOrDarging = true;
                }
            }

            prevPos0 = pos0;
            prevPos1 = pos1;
            prevMagnitude = currentMagnitude;
            prevAngle = currentAngle;
        }
        else
        {
            if (isPinching)
            {
                isPinching = false;
            }

            if (isGestureActive)
            {
                isGestureActive = false;
                OnGestureActive?.Invoke(false);
            }
        }
    }
}

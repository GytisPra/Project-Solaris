using UnityEngine;

public class JoystickPositionHandler : MonoBehaviour
{
    public RectTransform joystick;

    private ScreenOrientation lastOrientation;

    void Start()
    {
        lastOrientation = Screen.orientation;
        UpdateJoystickPosition(lastOrientation);
    }

    void Update()
    {
        if (Screen.orientation != lastOrientation)
        {
            lastOrientation = Screen.orientation;
            UpdateJoystickPosition(lastOrientation);
        }
    }

    void UpdateJoystickPosition(ScreenOrientation orientation)
    {
        if (orientation == ScreenOrientation.Portrait)
        {
            joystick.anchoredPosition = new Vector2(0, 300);
        }
        else
        {
            joystick.anchoredPosition = new Vector2(-1000, 300);
        }
    }
}

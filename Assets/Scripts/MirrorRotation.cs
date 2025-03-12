using UnityEngine;

public class MoveAndRotate2D : MonoBehaviour
{
    public float rotationSpeed = 200f;
    private Vector3 lastMousePosition;
    private Vector3 offset;
    private bool isDragging = false;
    private bool isRotating = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Left Mouse Button - Move
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOverObject())
            {
                isDragging = true;
                offset = transform.position - GetMouseWorldPosition();
            }
        }
        // Right Mouse Button - Rotate
        else if (Input.GetMouseButtonDown(1))
        {
            if (IsMouseOverObject())
            {
                isRotating = true;
                lastMousePosition = Input.mousePosition;
            }
        }

        if (Input.GetMouseButtonUp(0)) isDragging = false;
        if (Input.GetMouseButtonUp(1)) isRotating = false;

        // Move the object while dragging
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
        // Rotate the object while holding right-click
        else if (isRotating)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationZ = -delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, rotationZ);
            lastMousePosition = Input.mousePosition;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.nearClipPlane;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    bool IsMouseOverObject()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        return hit.collider != null && hit.transform == transform;
    }
}

using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    public Renderer rend;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDrag()
    {
        rend.material.color -= Color.red * Time.deltaTime;
        Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = newPos;
    }
}

using UnityEngine;

public class TrackUI : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    [SerializeField] private Transform subject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (subject)
        {
            transform.position = playerCamera.WorldToScreenPoint(subject.position);
        }
    }
}

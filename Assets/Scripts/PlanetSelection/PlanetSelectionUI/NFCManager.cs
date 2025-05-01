using UnityEngine;

public class NFCManager : MonoBehaviour
{
    [SerializeField] public bool AllowedNFC;

    private void Start()
    {
        AllowedNFC = false;
    }
    public void Toggler()
    {
        AllowedNFC = !AllowedNFC;
    }
}

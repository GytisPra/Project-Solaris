using System.Collections;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    public float openingSpeed = 2f;

    [SerializeField] private GameObject gateLeft;
    [SerializeField] private GameObject gateRight;

    public IEnumerator Open()
    {
        transform.GetComponent<Collider>().enabled = false;

        Quaternion targetRotationLeft = Quaternion.Euler(-90f, 0f, -90f);
        Quaternion targetRotationRight = Quaternion.Euler(-90f, 0f, 90f);

        while (Quaternion.Angle(gateLeft.transform.rotation, targetRotationLeft) > 0.1f || Quaternion.Angle(gateRight.transform.rotation, targetRotationRight) > 0.1f)
        {
            gateLeft.transform.rotation = Quaternion.Lerp(
                gateLeft.transform.rotation,
                targetRotationLeft,
                Time.deltaTime * openingSpeed
            );

            gateRight.transform.rotation = Quaternion.Lerp(
                gateRight.transform.rotation,
                targetRotationRight,
                Time.deltaTime * openingSpeed
            );

            yield return null;
        }

        gateLeft.transform.rotation = targetRotationLeft;
        gateRight.transform.rotation = targetRotationRight;
    }
}

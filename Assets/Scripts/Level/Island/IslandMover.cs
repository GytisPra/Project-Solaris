using System.Collections;
using UnityEngine;

public class IslandMover : MonoBehaviour
{
    [SerializeField] private Transform island;

    public IEnumerator MoveDown(float speed = 1f)
    {
        Vector3 targetPos = new(island.localPosition.x, 0f, island.localPosition.z);

        while (Vector3.Distance(island.localPosition, targetPos) > 0.01f)
        {
            island.localPosition = Vector3.MoveTowards(island.localPosition, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        island.localPosition = targetPos; // Ensure final alignment
    }
}

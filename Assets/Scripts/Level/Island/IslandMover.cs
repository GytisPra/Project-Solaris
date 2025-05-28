using System.Collections;
using UnityEngine;

public class IslandMover : MonoBehaviour
{
    [SerializeField] private Transform island;
    private Vector3 initialPos;

    private Coroutine moveCoroutine;

    private void Start()
    {
        initialPos = island.localPosition;
    }

    public void StartMoving(float speed = 1f)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTarget(speed));
    }

    public void MoveBack(float speed = 1f)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToInitial(speed));
    }

    private IEnumerator MoveToTarget(float speed = 1f)
    {
        Vector3 targetPos = new(initialPos.x, 0f, initialPos.z);
        while (Vector3.Distance(island.localPosition, targetPos) > 0.01f)
        {
            island.localPosition = Vector3.MoveTowards(island.localPosition, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        island.localPosition = targetPos;
    }

    private IEnumerator MoveToInitial(float speed = 1f)
    {
        Vector3 targetPos = initialPos;
        while (Vector3.Distance(island.localPosition, targetPos) > 0.01f)
        {
            island.localPosition = Vector3.MoveTowards(island.localPosition, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        island.localPosition = targetPos;
    }
}

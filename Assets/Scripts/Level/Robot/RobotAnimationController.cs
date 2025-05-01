using UnityEngine;

public class RobotAnimationController : MonoBehaviour
{
    private Animator animator;
    private RobotUIScript uiScript;

    void Start()
    {
        animator = GetComponent<Animator>();
        uiScript = GetComponentInChildren<RobotUIScript>();

        uiScript.OnRobotInteracted += StopWaving;

        animator.SetBool("isWaving", true);
    }
    void OnDestroy()
    {
        uiScript.OnRobotInteracted -= StopWaving;
    }
    public void StopWaving()
    {
        animator.SetBool("isWaving", false);
    }
}

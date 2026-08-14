using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", playerMovement.CurrentSpeed);
        animator.SetFloat("MotionSpeed", playerMovement.CurrentMovementStrength);
        animator.SetBool("Grounded", playerMovement.IsGrounded);
        animator.SetBool("Jump", playerMovement.IsJumping);
        animator.SetBool("FreeFall", playerMovement.IsFalling);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}

using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private float lockedMoveBlendDampTime = 0.1f;
    [SerializeField] private float freeMoveBlendDampTime = 0.1f;

    private Animator animator;
    private PlayerMovement playerMovement;
    private int attackIndex;
    private PlayerTargeting playerTargeting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTargeting = GetComponent<PlayerTargeting>();
    }

    private void Update()
    {
        float animationPlaybackSpeed = playerMovement.CurrentMovementStrength;
        if (animationPlaybackSpeed == 0f)
        {
            animationPlaybackSpeed = 1f;
        }

        animator.SetFloat("Speed", playerMovement.CurrentSpeed, freeMoveBlendDampTime, Time.deltaTime);
        animator.SetFloat("MotionSpeed", animationPlaybackSpeed);
        animator.SetBool("Grounded", playerMovement.IsGrounded);
        animator.SetBool("Jump", playerMovement.IsJumping);
        animator.SetBool("FreeFall", playerMovement.IsFalling);
        animator.SetFloat("MoveX", playerMovement.CurrentLocalMoveDirection.x, lockedMoveBlendDampTime, Time.deltaTime);
        animator.SetFloat("MoveZ", playerMovement.CurrentLocalMoveDirection.z, lockedMoveBlendDampTime, Time.deltaTime);
        animator.SetBool("IsLockedOn", playerTargeting.IsLockedOn);
    }

    public void PlayAttack(int attackIndex)
    {
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetTrigger("Attack");
    }
}

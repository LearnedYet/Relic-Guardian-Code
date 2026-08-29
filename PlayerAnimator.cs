using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private float lockedMoveBlendDampTime = 0.1f;
    [SerializeField] private float freeMoveBlendDampTime = 0.1f;
    [SerializeField] private float blockCrossFadeDuration = 0.12f;
    [SerializeField] private float blockExitCrossFadeDuration = 0.45f;
    [SerializeField] private float softRecoveryInterruptCrossFadeDuration = 0.05f;
    [SerializeField] private float debugBodyYawOffset;

    private Animator animator;
    private PlayerMovement playerMovement;
    private int attackIndex;
    private PlayerTargeting playerTargeting;
    private bool isSoftRecoveryActive;
    private bool hasSoftRecoveryTransitionStarted;
    private bool isBlockHoldPresentationActive;
    private bool isBlockHoldPresentationLocked;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTargeting = GetComponent<PlayerTargeting>();
    }

    public void PlayBlockStart()
    {
        isBlockHoldPresentationActive = false;
        isSoftRecoveryActive = false;
        hasSoftRecoveryTransitionStarted = false;

        animator.CrossFadeInFixedTime(
            "Base Layer.Block_Start",
            blockCrossFadeDuration
        );
    }

    public void PlayBlockHold()
    {
        isBlockHoldPresentationActive = true;
        isBlockHoldPresentationLocked = playerTargeting.IsLockedOn;

        if (playerTargeting.IsLockedOn)
        {
            animator.CrossFadeInFixedTime(
                "Base Layer.Guard_Locked_Locomotion",
                blockCrossFadeDuration
            );
        }
        else
        {
            animator.CrossFadeInFixedTime(
                "Base Layer.Guard_Free_Locomotion",
                blockCrossFadeDuration
            );
        }
    }

    public void PlayBlockEnd()
    {
        isBlockHoldPresentationActive = false;

        animator.CrossFadeInFixedTime(
            "Base Layer.Block_End",
            blockCrossFadeDuration
        );
    }

    public void BeginSoftRecovery()
    {
        isSoftRecoveryActive = true;
        hasSoftRecoveryTransitionStarted = false;
    }

    public void PlayLocomotion()
    {
        BeginSoftRecovery();
        CrossFadeToLocomotion(blockExitCrossFadeDuration);
    }

    private void CrossFadeToLocomotion(float crossFadeDuration)
    {
        if (playerTargeting.IsLockedOn)
        {
            animator.CrossFadeInFixedTime(
                "Base Layer.Locked Locomotion",
                crossFadeDuration
            );
        }
        else
        {
            animator.CrossFadeInFixedTime(
                "Base Layer.Idle Walk Run Blend",
                crossFadeDuration
            );
        }
    }

    private void Update()
    {
        if (isBlockHoldPresentationActive
            && isBlockHoldPresentationLocked != playerTargeting.IsLockedOn)
        {
            PlayBlockHold();
        }

        if (isSoftRecoveryActive && animator.IsInTransition(0))
        {
            hasSoftRecoveryTransitionStarted = true;
        }
        else if (isSoftRecoveryActive && hasSoftRecoveryTransitionStarted)
        {
            isSoftRecoveryActive = false;
            hasSoftRecoveryTransitionStarted = false;
        }

        float animationPlaybackSpeed = playerMovement.CurrentMovementStrength;

        if (isSoftRecoveryActive
            && playerMovement.CurrentMovementStrength > 0f)
        {
            isSoftRecoveryActive = false;
            hasSoftRecoveryTransitionStarted = false;
            CrossFadeToLocomotion(softRecoveryInterruptCrossFadeDuration);
        }

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

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 bodyForward = animator.bodyRotation * Vector3.forward;
        debugBodyYawOffset = Vector3.SignedAngle(
            transform.forward,
            bodyForward,
            Vector3.up
        );
    }

    public void PlayAttack(int attackIndex)
    {
        animator.SetInteger("AttackIndex", attackIndex);

        if (isSoftRecoveryActive && attackIndex == 0)
        {
            isSoftRecoveryActive = false;
            hasSoftRecoveryTransitionStarted = false;
            animator.CrossFadeInFixedTime(
                "Base Layer.BasicAttack",
                softRecoveryInterruptCrossFadeDuration
            );
            return;
        }

        animator.SetTrigger("Attack");
    }
}

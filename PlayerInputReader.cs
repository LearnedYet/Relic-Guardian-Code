using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2 lookInput;
    private bool jumpRequested;
    private bool attackRequested;
    private bool lockOnRequested;

    public Vector2 MoveInput
    {
        get
        {
            return moveInput;
        }
    }

    public Vector2 LookInput
    {
        get
        {
            return lookInput;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnAttack()
    {
        attackRequested = true;
    }

    public bool ConsumeAttack()
    {
        bool wasAttackRequested = attackRequested;
        attackRequested = false;
        return wasAttackRequested;
    }

    public void OnJump()
    {
        jumpRequested = true;
    }

    public bool ConsumeJump()
    {
        bool wasJumpRequested = jumpRequested;
        jumpRequested = false;
        return wasJumpRequested;
    }

    public void OnLockOn()
    {
        lockOnRequested = true;
    }

    public bool ConsumeLockOn()
    {
        bool wasLockOnRequested = lockOnRequested;
        lockOnRequested = false;
        return wasLockOnRequested;
    }
}

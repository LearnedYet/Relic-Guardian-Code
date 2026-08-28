using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    private PlayerActionState currentActionState;
    private PlayerInputReader playerInputReader;
    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerBlock playerBlock;

    private int lastResolvedActionRequestFrame = -1;
    private bool wasJumpAcceptedThisFrame;

    public PlayerActionState CurrentActionState
    {
        get { return currentActionState; }
    }

    public bool CanMove
    {
        get { return currentActionState == PlayerActionState.Free; }
    }

    public bool CanJump
    {
        get { return currentActionState == PlayerActionState.Free; }
    }

    public bool WasJumpAcceptedThisFrame
    {
        get { return wasJumpAcceptedThisFrame; }
    }

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
        playerCombat = GetComponent<PlayerCombat>();
        playerMovement = GetComponent<PlayerMovement>();
        playerBlock = GetComponent<PlayerBlock>();
    }

    public void ResolveActionRequests()
    {
        if (lastResolvedActionRequestFrame == Time.frameCount)
        {
            return;
        }

        lastResolvedActionRequestFrame = Time.frameCount;
        wasJumpAcceptedThisFrame = false;

        bool blockRequested = playerInputReader.ConsumeBlock();
        bool attackRequested = playerInputReader.ConsumeAttack();
        bool jumpRequested = playerInputReader.ConsumeJump();

        if (blockRequested && TryStartBlock())
        {
            return;
        }

        if (attackRequested && playerCombat.TryHandleAttackRequest())
        {
            return;
        }

        if (jumpRequested && playerMovement.CanStartJump)
        {
            wasJumpAcceptedThisFrame = true;
        }
    }

    private bool TryStartBlock()
    {
        if (!playerMovement.IsGrounded)
        {
            return false;
        }

        if (currentActionState == PlayerActionState.Free)
        {
            currentActionState = PlayerActionState.Blocking;
            playerBlock.BeginBlock();
            return true;
        }

        if (currentActionState == PlayerActionState.Attacking && playerCombat.TryCancelAttack())
        {
            currentActionState = PlayerActionState.Blocking;
            playerBlock.BeginBlock();
            return true;
        }

        return false;
    }

    public bool TryStartAttack(bool isGrounded)
    {
        if (currentActionState == PlayerActionState.Free && isGrounded)
        {
            currentActionState = PlayerActionState.Attacking;
            return true;
        }

        return false;
    }

    public void FinishAttack()
    {
        if (currentActionState == PlayerActionState.Attacking)
        {
            currentActionState = PlayerActionState.Free;
        }
    }

    public void FinishBlock()
    {
        if (currentActionState == PlayerActionState.Blocking)
        {
            currentActionState = PlayerActionState.Free;
        }
    }
}

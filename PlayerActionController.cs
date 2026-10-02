using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    private PlayerActionState currentActionState;
    private PlayerInputReader playerInputReader;
    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerBlock playerBlock;
    private PlayerDodge playerDodge;

    private int lastResolvedActionRequestFrame = -1;
    private bool wasJumpAcceptedThisFrame;

    public PlayerActionState CurrentActionState
    {
        get { return currentActionState; }
    }

    public bool CanMove
    {
        get
        {
            return currentActionState == PlayerActionState.Free
                || (currentActionState == PlayerActionState.Blocking
                    && playerBlock.AllowsMovement);
        }
    }

    public bool CanFaceLockedTarget
    {
        get
        {
            return currentActionState == PlayerActionState.Free || currentActionState == PlayerActionState.Blocking || currentActionState == PlayerActionState.Dodging;
        }
    }

    public bool CanJump
    {
        get { return currentActionState == PlayerActionState.Free; }
    }

    public bool WasJumpAcceptedThisFrame
    {
        get { return wasJumpAcceptedThisFrame; }
    }

    public bool CanSprint
    {
        get { return currentActionState == PlayerActionState.Free; }
    }

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
        playerCombat = GetComponent<PlayerCombat>();
        playerMovement = GetComponent<PlayerMovement>();
        playerBlock = GetComponent<PlayerBlock>();
        playerDodge = GetComponent<PlayerDodge>();
    }

    public void ResolveActionRequests()
    {
        if (lastResolvedActionRequestFrame == Time.frameCount)
        {
            return;
        }

        lastResolvedActionRequestFrame = Time.frameCount;
        wasJumpAcceptedThisFrame = false;

        bool dodgeRequested = playerInputReader.ConsumeDodge();
        bool blockRequested = playerInputReader.ConsumeBlock();
        bool attackRequested = playerInputReader.ConsumeAttack();
        bool jumpRequested = playerInputReader.ConsumeJump();

        if (dodgeRequested && TryStartDodge())
        {
            playerBlock.ClearGuardCounterOpportunity();
            return;
        }
        if (blockRequested && TryStartBlock())
        {
            playerBlock.ClearGuardCounterOpportunity();
            return;
        }

        if (attackRequested && TryStartGuardCounter())
        {
            return;
        }

        if (attackRequested && playerCombat.TryHandleAttackRequest())
        {
            playerBlock.ClearGuardCounterOpportunity();
            return;
        }

        if (jumpRequested && playerMovement.CanStartJump)
        {
            wasJumpAcceptedThisFrame = true;
            playerBlock.ClearGuardCounterOpportunity();
        }
    }

    //尝试开始闪避
    private bool TryStartDodge()
    {
        if (!playerMovement.IsGrounded)
        {
            return false;
        }

        if (currentActionState == PlayerActionState.Free)
        {
            currentActionState = PlayerActionState.Dodging;
            playerDodge.BeginDodge();
            return true;
        }

        //闪避取消攻击
        if (currentActionState == PlayerActionState.Attacking && playerCombat.TryCancelAttack())
        {
            currentActionState = PlayerActionState.Dodging;
            playerDodge.BeginDodge();
            return true;
        }

        return false;
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

    private bool TryStartGuardCounter()
    {
        if (!playerMovement.IsGrounded
            || (currentActionState != PlayerActionState.Blocking
                && currentActionState != PlayerActionState.Free))
        {
            return false;
        }

        if (!playerBlock.TryConsumeGuardCounterOpportunity())
        {
            return false;
        }

        if (currentActionState == PlayerActionState.Blocking)
        {
            playerBlock.CancelBlock();
        }

        currentActionState = PlayerActionState.Attacking;
        playerCombat.BeginGuardCounter();
        return true;
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

    public void FinishDodge()
    {
        if (currentActionState == PlayerActionState.Dodging)
        {
            currentActionState = PlayerActionState.Free;
        }
    }
}

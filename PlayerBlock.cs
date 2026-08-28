using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    private PlayerInputReader playerInputReader;
    private PlayerActionController playerActionController;
    private PlayerAnimator playerAnimator;

    private enum BlockPhase
    {
        Startup,
        Hold,
        Release
    }

    private BlockPhase currentBlockPhase;

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
        playerActionController = GetComponent<PlayerActionController>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        playerActionController.ResolveActionRequests();

        if (playerActionController.CurrentActionState == PlayerActionState.Blocking
            && currentBlockPhase == BlockPhase.Hold
            && !playerInputReader.IsBlockHeld)
        {
            EnterRelease();
        }
    }

    public void BeginBlock()
    {
        currentBlockPhase = BlockPhase.Startup;
        playerAnimator.PlayBlockStart();
    }

    public void StartupDecisionPoint()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Blocking
            || currentBlockPhase != BlockPhase.Startup)
        {
            return;
        }

        if (playerInputReader.IsBlockHeld)
        {
            EnterHold();
        }
        else
        {
            EnterRelease();
        }
    }

    public void FinishRelease()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Blocking
            || currentBlockPhase != BlockPhase.Release)
        {
            return;
        }

        playerActionController.FinishBlock();
        playerAnimator.PlayLocomotion();
    }

    private void EnterHold()
    {
        currentBlockPhase = BlockPhase.Hold;
        playerAnimator.PlayBlockLoop();
    }

    private void EnterRelease()
    {
        currentBlockPhase = BlockPhase.Release;
        playerAnimator.PlayBlockEnd();
    }
}

using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    [SerializeField] private float guardCoverageHalfAngle = 90f;
    [SerializeField] private float facingAssistHalfAngle = 60f;

    private bool isPerfectGuardWindowOpen;
    private PlayerInputReader playerInputReader;
    private PlayerActionController playerActionController;
    private PlayerAnimator playerAnimator;
    private PlayerHitReceiver playerHitReceiver;

    private enum BlockPhase
    {
        Startup,
        Hold,
        Release
    }

    private BlockPhase currentBlockPhase;
    private Vector3 guardFacingAssistDirection;
    private Transform guardFacingAssistSource;
    private Vector3 guardFacingBeforeAssist;
    private float guardFacingAssistEndTime;

    public bool AllowsMovement
    {
        get
        {
            return currentBlockPhase == BlockPhase.Hold
                && playerInputReader.IsBlockHeld;
        }
    }

    public bool IsGuardFacingAssistActive
    {
        get
        {
            return playerActionController.CurrentActionState == PlayerActionState.Blocking
                && (currentBlockPhase == BlockPhase.Startup
                    || currentBlockPhase == BlockPhase.Hold)
                && Time.time < guardFacingAssistEndTime;
        }
    }

    public Vector3 GuardFacingAssistDirection
    {
        get { return guardFacingAssistDirection; }
    }

    public bool TryStartFacingAssist(AttackThreatContext attackThreatContext)
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Blocking
            || (currentBlockPhase != BlockPhase.Startup
                && currentBlockPhase != BlockPhase.Hold)
            || IsGuardFacingAssistActive
            || attackThreatContext.Source == null
            || attackThreatContext.ExpectedImpactTime <= Time.time)
        {
            return false;
        }

        Vector3 horizontalIncomingDirection = attackThreatContext.IncomingDirection;
        horizontalIncomingDirection.y = 0f;

        if (horizontalIncomingDirection == Vector3.zero)
        {
            return false;
        }

        Vector3 directionTowardAttack = -horizontalIncomingDirection.normalized;

        Vector3 horizontalForward = transform.forward;
        horizontalForward.y = 0f;
        horizontalForward.Normalize();

        float threatAngle = Vector3.Angle(
            horizontalForward,
            directionTowardAttack
        );

        if (threatAngle > guardCoverageHalfAngle
            || threatAngle > facingAssistHalfAngle)
        {
            return false;
        }

        guardFacingAssistSource = attackThreatContext.Source;
        guardFacingBeforeAssist = horizontalForward;
        guardFacingAssistDirection = directionTowardAttack;
        guardFacingAssistEndTime = attackThreatContext.ExpectedImpactTime;
        return true;
    }

    public GuardResult ResolveGuardHit(HitContext hitContext)
    {
        if (currentBlockPhase != BlockPhase.Startup
            && currentBlockPhase != BlockPhase.Hold)
        {
            return GuardResult.Unhandled;
        }

        bool matchesFacingAssistSource = hitContext.Source == guardFacingAssistSource
            && guardFacingBeforeAssist != Vector3.zero;

        Vector3 horizontalIncomingDirection = hitContext.IncomingDirection;
        horizontalIncomingDirection.y = 0f;

        if (horizontalIncomingDirection == Vector3.zero)
        {
            return GuardResult.Unhandled;
        }

        Vector3 directionTowardAttack = -horizontalIncomingDirection.normalized;

        Vector3 horizontalForward = matchesFacingAssistSource
            ? guardFacingBeforeAssist
            : transform.forward;

        horizontalForward.y = 0f;
        horizontalForward.Normalize();

        float hitAngle = Vector3.Angle(
            horizontalForward,
            directionTowardAttack
        );

        if (matchesFacingAssistSource)
        {
            ClearGuardFacingAssist();
        }

        if (hitAngle > guardCoverageHalfAngle)
        {
            return GuardResult.Unhandled;
        }

        if (isPerfectGuardWindowOpen)
        {
            return GuardResult.Perfect;
        }
        else
        {
            return GuardResult.Ordinary;
        }
    }

    private void ClearGuardFacingAssist()
    {
        guardFacingAssistSource = null;
        guardFacingBeforeAssist = Vector3.zero;
        guardFacingAssistDirection = Vector3.zero;
        guardFacingAssistEndTime = 0f;
    }

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
        playerActionController = GetComponent<PlayerActionController>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerHitReceiver = GetComponent<PlayerHitReceiver>();
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
        ClosePerfectGuardWindow();
        ClearGuardFacingAssist();
        currentBlockPhase = BlockPhase.Startup;
        OpenPerfectGuardWindow();

        if (playerHitReceiver != null
            && playerHitReceiver.TryGetNextAttackThreat(
                out AttackThreatContext nextAttackThreat
            ))
        {
            TryStartFacingAssist(nextAttackThreat);
        }

        playerAnimator.PlayBlockStart();
    }

    private void OpenPerfectGuardWindow()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Blocking
            || currentBlockPhase != BlockPhase.Startup)
        {
            return;
        }

        isPerfectGuardWindowOpen = true;
    }

    public void ClosePerfectGuardWindow()
    {
        isPerfectGuardWindowOpen = false;
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
        ClosePerfectGuardWindow();
        currentBlockPhase = BlockPhase.Hold;
        playerAnimator.PlayBlockHold();
    }

    private void EnterRelease()
    {
        ClearGuardFacingAssist();
        ClosePerfectGuardWindow();
        currentBlockPhase = BlockPhase.Release;
        playerAnimator.PlayBlockEnd();
    }
}

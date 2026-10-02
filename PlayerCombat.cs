using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerAttackData[] attacks;
    [SerializeField] private PlayerAttackData guardCounterAttackData = new PlayerAttackData();
    [SerializeField] private LayerMask hitTargetLayers;

    private PlayerAnimator playerAnimator;
    private PlayerActionController playerActionController;
    private PlayerMovement playerMovement;
    private PlayerTargeting playerTargeting;
    private PlayerAttackPresentation playerAttackPresentation;

    private bool isHitWindowOpen;
    private bool isAttackFacingActive;
    private bool isBasicAttackLungeActive;
    private float basicAttackLungeDistanceTraveled;
    private bool isComboWindowOpen;
    private bool isAttackQueued;
    private bool isRestartWindowOpen;
    private bool hasReachedComboTransitionPoint;
    private Collider currentAttackTarget;
    private Collider confirmedAttackTarget;
    private int currentAttackIndex;
    private int lastGuardCounterHitIndex;
    private PlayerAttackType currentAttackType = PlayerAttackType.Basic;

    public bool IsHitWindowOpen
    {
        get { return isHitWindowOpen; }
    }

    private PlayerAttackData CurrentAttackData
    {
        get
        {
            if (currentAttackType == PlayerAttackType.GuardCounter)
            {
                return guardCounterAttackData;
            }

            return attacks[currentAttackIndex];
        }
    }

    private bool HasNextAttack
    {
        get { return currentAttackIndex + 1 < attacks.Length; }
    }

    private bool IsCurrentAttackStep(int attackIndex)
    {
        return playerActionController.CurrentActionState == PlayerActionState.Attacking
            && currentAttackType == PlayerAttackType.Basic
            && attackIndex == currentAttackIndex;
    }

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerActionController = GetComponent<PlayerActionController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTargeting = GetComponent<PlayerTargeting>();
        playerAttackPresentation = GetComponent<PlayerAttackPresentation>();
    }

    public void OpenHitWindow(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        isHitWindowOpen = true;
        isAttackFacingActive = false;
        isBasicAttackLungeActive = false;

        ResolveCurrentAttackHit();
    }

    public void OpenGuardCounterHitWindow(int hitIndex)
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking
            || currentAttackType != PlayerAttackType.GuardCounter
            || hitIndex < 1
            || hitIndex > 2
            || hitIndex <= lastGuardCounterHitIndex)
        {
            return;
        }

        lastGuardCounterHitIndex = hitIndex;
        isHitWindowOpen = true;
        isAttackFacingActive = false;
        isBasicAttackLungeActive = false;
        ResolveCurrentAttackHit();
    }

    public void CloseGuardCounterHitWindow(int hitIndex)
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking
            || currentAttackType != PlayerAttackType.GuardCounter
            || hitIndex != lastGuardCounterHitIndex)
        {
            return;
        }

        isHitWindowOpen = false;
        confirmedAttackTarget = null;
    }

    public void OpenGuardCounterWeaponTrail()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking
            || currentAttackType != PlayerAttackType.GuardCounter)
        {
            return;
        }

        playerAttackPresentation.OpenCounterWeaponTrail();
    }

    public void CloseGuardCounterWeaponTrail()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking
            || currentAttackType != PlayerAttackType.GuardCounter)
        {
            return;
        }

        playerAttackPresentation.CloseCounterWeaponTrail();
    }

    public void PlayGuardCounterWeaponWhoosh(int swingIndex)
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking
            || currentAttackType != PlayerAttackType.GuardCounter)
        {
            return;
        }

        playerAttackPresentation.PlayGuardCounterWhoosh(swingIndex);
    }

    private void ResolveCurrentAttackHit()
    {
        if (IsCurrentAttackTargetInRange())
        {
            confirmedAttackTarget = currentAttackTarget;
            EnemyHitReceiver enemyHitReceiver = confirmedAttackTarget.GetComponent<EnemyHitReceiver>();

            if (enemyHitReceiver != null)
            {
                Vector3 incomingDirection = confirmedAttackTarget.transform.position - transform.position;
                HitFeedbackType feedbackType = HitFeedbackType.Default;
                int hitIndex = 0;

                if (currentAttackType == PlayerAttackType.GuardCounter)
                {
                    feedbackType = HitFeedbackType.GuardCounter;
                    hitIndex = lastGuardCounterHitIndex;
                }
                //传递信息
                HitContext hitContext = new HitContext(CurrentAttackData.Damage, transform, incomingDirection, feedbackType, hitIndex);
                enemyHitReceiver.ReceiveHit(hitContext);
            }
        }
        else
        {
            confirmedAttackTarget = null;
        }
    }

    public void CloseHitWindow(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        isHitWindowOpen = false;
        confirmedAttackTarget = null;
    }

    public void OpenWeaponTrail(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        playerAttackPresentation.OpenWeaponTrail();
    }

    public void CloseWeaponTrail(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        playerAttackPresentation.CloseWeaponTrail();
    }

    public void PlayWeaponWhoosh(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        playerAttackPresentation.PlayWeaponWhoosh(attackIndex);
    }

    public void PlayWeaponWindup(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        playerAttackPresentation.PlayWeaponWindup(attackIndex);
    }

    public void OpenComboWindow(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        isComboWindowOpen = true;
    }

    public void ComboTransitionPoint(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        hasReachedComboTransitionPoint = true;
        TryStartQueuedAttack();
    }

    public void EnterRestartWindow(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        isComboWindowOpen = false;
        isRestartWindowOpen = true;
    }

    public void FinishAttack(int attackIndex)
    {
        if (!IsCurrentAttackStep(attackIndex))
        {
            return;
        }

        CleanupAttack();
        playerActionController.FinishAttack();
    }

    public void FinishGuardCounter()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking
            || currentAttackType != PlayerAttackType.GuardCounter)
        {
            return;
        }

        CleanupAttack();
        playerActionController.FinishAttack();
    }

    public bool TryCancelAttack()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking
            || currentAttackType != PlayerAttackType.Basic)
        {
            return false;
        }

        CleanupAttack();
        return true;
    }

    private void CleanupAttack()
    {
        playerAttackPresentation.CloseWeaponTrail();
        playerAttackPresentation.CloseCounterWeaponTrail();

        isHitWindowOpen = false;
        isComboWindowOpen = false;
        isRestartWindowOpen = false;
        isAttackQueued = false;
        hasReachedComboTransitionPoint = false;
        currentAttackTarget = null;
        confirmedAttackTarget = null;
        isAttackFacingActive = false;
        isBasicAttackLungeActive = false;
        basicAttackLungeDistanceTraveled = 0f;
        currentAttackIndex = 0;
        currentAttackType = PlayerAttackType.Basic;
        playerAnimator.BeginSoftRecovery();
    }

    public bool TryHandleAttackRequest()
    {
        if (playerActionController.CurrentActionState == PlayerActionState.Free
            && playerActionController.TryStartAttack(playerMovement.IsGrounded))
        {
            isAttackQueued = false;
            hasReachedComboTransitionPoint = false;
            StartAttackStep(0, PlayerAttackType.Basic);
            return true;
        }
        else if (playerActionController.CurrentActionState == PlayerActionState.Attacking
            && isComboWindowOpen)
        {
            isAttackQueued = true;

            if (hasReachedComboTransitionPoint)
            {
                TryStartQueuedAttack();
            }

            return true;
        }
        else if (playerActionController.CurrentActionState == PlayerActionState.Attacking
            && isRestartWindowOpen)
        {
            isAttackQueued = false;
            hasReachedComboTransitionPoint = false;
            StartAttackStep(0, PlayerAttackType.Basic);
            return true;
        }

        return false;
    }

    public void BeginGuardCounter()
    {
        isAttackQueued = false;
        isComboWindowOpen = false;
        isRestartWindowOpen = false;
        hasReachedComboTransitionPoint = false;
        StartAttackStep(0, PlayerAttackType.GuardCounter);
    }

    private void Update()
    {
        playerActionController.ResolveActionRequests();

        if (isAttackFacingActive && EnemyHitReceiver.IsValidTarget(currentAttackTarget))
        {
            Vector3 directionToTarget = currentAttackTarget.bounds.center - transform.position;
            directionToTarget.y = 0f;
            playerMovement.FaceDirection(directionToTarget);

            if (isBasicAttackLungeActive)
            {
                float requestedMoveDistance = CurrentAttackData.LungeSpeed * Time.deltaTime;
                float remainingDistance = CurrentAttackData.LungeDistance - basicAttackLungeDistanceTraveled;
                float moveDistance = Mathf.Min(requestedMoveDistance, remainingDistance);
                playerMovement.MoveDuringAttack(directionToTarget, moveDistance);
                basicAttackLungeDistanceTraveled += moveDistance;

                if (basicAttackLungeDistanceTraveled >= CurrentAttackData.LungeDistance)
                {
                    isBasicAttackLungeActive = false;
                }
            }
        }
    }

    private void TryStartQueuedAttack()
    {
        if (!isAttackQueued || !HasNextAttack)
        {
            return;
        }

        int nextAttackIndex = currentAttackIndex + 1;
        isAttackQueued = false;
        isComboWindowOpen = false;
        hasReachedComboTransitionPoint = false;
        StartAttackStep(nextAttackIndex, PlayerAttackType.Basic);
    }

    private void StartAttackStep(int attackIndex, PlayerAttackType attackType)
    {
        playerAttackPresentation.CloseWeaponTrail();

        currentAttackType = attackType;
        lastGuardCounterHitIndex = 0;
        currentAttackIndex = attackIndex;
        isRestartWindowOpen = false;

        if (playerTargeting.IsLockedOn)
        {
            currentAttackTarget = playerTargeting.CurrentTarget;

            if (!IsCurrentAttackTargetInRange())
            {
                currentAttackTarget = null;
            }
        }
        else
        {
            currentAttackTarget = FindNearestBasicAttackTarget();
        }

        isAttackFacingActive = currentAttackTarget != null;
        basicAttackLungeDistanceTraveled = 0f;
        isBasicAttackLungeActive = currentAttackTarget != null;

        if (currentAttackType == PlayerAttackType.GuardCounter)
        {
            playerAnimator.PlayGuardCounter();
        }
        else
        {
            playerAnimator.PlayAttack(currentAttackIndex);
        }
    }

    private Collider[] FindBasicAttackCandidates()
    {
        return Physics.OverlapSphere(
            transform.position,
            CurrentAttackData.TargetRange,
            hitTargetLayers
        );
    }

    private bool IsCurrentAttackTargetInRange()
    {
        if (!EnemyHitReceiver.IsValidTarget(currentAttackTarget))
        {
            return false;
        }

        Collider[] candidates = FindBasicAttackCandidates();
        foreach (Collider candidate in candidates)
        {
            if (candidate == currentAttackTarget)
            {
                return true;
            }
        }

        return false;
    }

    private Collider FindNearestBasicAttackTarget()
    {
        Collider[] candidates = FindBasicAttackCandidates();
        Collider nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider candidate in candidates)
        {
            if (!EnemyHitReceiver.IsValidTarget(candidate))
            {
                continue;
            }

            Vector3 directionToCandidate = candidate.bounds.center - transform.position;
            directionToCandidate.y = 0f;
            float distanceToCandidate = directionToCandidate.magnitude;

            if (distanceToCandidate < nearestDistance)
            {
                nearestDistance = distanceToCandidate;
                nearestTarget = candidate;
            }
        }

        return nearestTarget;
    }
}

using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerAttackData[] attacks;
    [SerializeField] private LayerMask hitTargetLayers;

    private PlayerAnimator playerAnimator;
    private PlayerActionController playerActionController;
    private PlayerMovement playerMovement;
    private PlayerTargeting playerTargeting;

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

    public bool IsHitWindowOpen
    {
        get { return isHitWindowOpen; }
    }

    private PlayerAttackData CurrentAttackData
    {
        get { return attacks[currentAttackIndex]; }
    }

    private bool HasNextAttack
    {
        get { return currentAttackIndex + 1 < attacks.Length; }
    }

    private bool IsCurrentAttackStep(int attackIndex)
    {
        return playerActionController.CurrentActionState == PlayerActionState.Attacking
            && attackIndex == currentAttackIndex;
    }

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerActionController = GetComponent<PlayerActionController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTargeting = GetComponent<PlayerTargeting>();
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

        if (IsCurrentAttackTargetInRange())
        {
            confirmedAttackTarget = currentAttackTarget;
            EnemyHealth enemyHealth = confirmedAttackTarget.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(CurrentAttackData.Damage);
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

        EndAttack();
    }

    public bool TryCancelAttack()
    {
        if (playerActionController.CurrentActionState != PlayerActionState.Attacking)
        {
            return false;
        }

        EndAttack();
        return true;
    }

    private void EndAttack()
    {
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
        playerActionController.FinishAttack();
        playerAnimator.BeginSoftRecovery();
    }

    public bool TryHandleAttackRequest()
    {
        if (playerActionController.CurrentActionState == PlayerActionState.Free
            && playerActionController.TryStartAttack(playerMovement.IsGrounded))
        {
            isAttackQueued = false;
            hasReachedComboTransitionPoint = false;
            StartAttackStep(0);
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
            StartAttackStep(0);
            return true;
        }

        return false;
    }

    private void Update()
    {
        playerActionController.ResolveActionRequests();

        if (isAttackFacingActive && currentAttackTarget != null)
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
        StartAttackStep(nextAttackIndex);
    }

    private void StartAttackStep(int attackIndex)
    {
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

        playerAnimator.PlayAttack(currentAttackIndex);
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
        if (currentAttackTarget == null)
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

using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private MeleeAttackData[] attackOptions;
    [SerializeField] private GameObject startupTelegraph;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyStateController enemyStateController;

    private EnemyAttackPhase currentPhase;
    private PlayerHitReceiver currentAttackTarget;
    private MeleeAttackData currentAttackData;
    private float phaseElapsedTime;
    private float attackAnimationElapsedTime;
    private bool hasAttackAnimationStarted;

    public EnemyAttackPhase CurrentPhase => currentPhase;

    private void Update()
    {

        if (currentPhase != EnemyAttackPhase.Ready)
        {
            phaseElapsedTime += Time.deltaTime;
        }

        if (currentPhase == EnemyAttackPhase.Startup
            && !hasAttackAnimationStarted
            && phaseElapsedTime >= currentAttackData.StartupDuration - currentAttackData.AnimationLeadTime)
        {
            hasAttackAnimationStarted = true;
            animator.Play(currentAttackData.AnimationStateName, 0,0f);
        }

        UpdateAttackTracking();
        UpdateAttackMovement();

        if (currentPhase == EnemyAttackPhase.Startup && phaseElapsedTime >= currentAttackData.StartupDuration)
        {
            OpenHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.HitWindow && phaseElapsedTime >= currentAttackData.HitWindowDuration)
        {
            CloseHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.Recovery && phaseElapsedTime >= currentAttackData.RecoveryDuration)
        {
            FinishRecovery();
        }
    }

    private void UpdateAttackTracking()
    {
        if (!hasAttackAnimationStarted
            || attackAnimationElapsedTime < currentAttackData.TrackingStartTime
            || attackAnimationElapsedTime >= currentAttackData.TrackingEndTime
            || currentAttackTarget == null
            || !currentAttackTarget.isActiveAndEnabled)
        {
            return;
        }

        Vector3 directionToTarget =
            currentAttackTarget.transform.position - transform.position;

        enemyMovement.Turn(directionToTarget);
    }

    private void UpdateAttackMovement()
    {
        if (!hasAttackAnimationStarted
            || currentAttackData.MovementEndTime <= currentAttackData.MovementStartTime
            || currentAttackData.MovementDistance <= 0f)
        {
            return;
        }

        float previousProgress = Mathf.InverseLerp(
            currentAttackData.MovementStartTime,
            currentAttackData.MovementEndTime,
            attackAnimationElapsedTime);

        attackAnimationElapsedTime += Time.deltaTime;

        float currentProgress = Mathf.InverseLerp(
            currentAttackData.MovementStartTime,
            currentAttackData.MovementEndTime,
            attackAnimationElapsedTime);

        float frameDistance = currentAttackData.MovementDistance * (currentProgress - previousProgress);

        enemyMovement.MoveDuringAttack(transform.forward, frameDistance);
    }

    public bool TryStartAttack(PlayerHitReceiver target)
    {
        if (currentPhase != EnemyAttackPhase.Ready
            || target == null
            || !target.isActiveAndEnabled
            || attackOptions == null)
        {
            return false;
        }
        Vector3 horizontalOffsetToTarget =
            target.transform.position - transform.position;

        horizontalOffsetToTarget.y = 0f;

        float distanceToTarget = horizontalOffsetToTarget.magnitude;

        currentAttackData = null;

        foreach (MeleeAttackData attackOption in attackOptions)
        {
            if (attackOption != null
                && distanceToTarget >= attackOption.MinimumRange
                && distanceToTarget <= attackOption.MaximumRange)
            {
                currentAttackData = attackOption;
                break;
            }
        }

        if (currentAttackData == null)
        {
            return false;
        }

        currentAttackTarget = target;
        currentPhase = EnemyAttackPhase.Startup;
        phaseElapsedTime = 0f;
        hasAttackAnimationStarted = false;
        attackAnimationElapsedTime = 0f;

        Vector3 incomingDirection =
            target.transform.position - transform.position;

        float expectedImpactTime =
            Time.time + currentAttackData.StartupDuration;

        AttackThreatContext attackThreatContext =
            new AttackThreatContext(
                transform,
                incomingDirection,
                expectedImpactTime);

        target.ReceiveAttackThreat(attackThreatContext);
        startupTelegraph.SetActive(true);
        return true;
    }

    private void OnDisable()
    {
        CancelAttack();
    }

    public void OpenHitWindow()
    {
        if (currentPhase == EnemyAttackPhase.Startup)
        {
            currentPhase = EnemyAttackPhase.HitWindow;
            phaseElapsedTime = 0f;

            if (currentAttackTarget != null)
            {
                currentAttackTarget.RemoveAttackThreat(transform);
            }

            ApplyDamage(currentAttackTarget);

            if (currentPhase != EnemyAttackPhase.HitWindow)
            {
                return;
            }

            startupTelegraph.SetActive(false);
        }
    }

    public void CloseHitWindow()
    {
        if (currentPhase == EnemyAttackPhase.HitWindow)
        {
            currentPhase = EnemyAttackPhase.Recovery;
            phaseElapsedTime = 0f;
        }
    }

    public void FinishRecovery()
    {
        if (currentPhase == EnemyAttackPhase.Recovery)
        {
            CancelAttack();
            enemyStateController.FinishAttack();
        }
    }

    public void CancelAttack()
    {
        bool wasAttackActive = currentPhase != EnemyAttackPhase.Ready;

        if (currentAttackTarget != null)
        {
            currentAttackTarget.RemoveAttackThreat(transform);
        }

        currentAttackTarget = null;
        currentAttackData = null;
        currentPhase = EnemyAttackPhase.Ready;
        phaseElapsedTime = 0f;
        hasAttackAnimationStarted = false;
        attackAnimationElapsedTime = 0f;
        animator.ResetTrigger("Attack");
        startupTelegraph.SetActive(false);

        if (wasAttackActive)
        {
            enemyStateController.StartGlobalAttackCooldown();
        }
    }

    private bool IsImpactValid(PlayerHitReceiver target)
    {
        if (target == null || !target.isActiveAndEnabled)
        {
            return false;
        }

        Vector3 directionToTarget = target.transform.position - transform.position;
        directionToTarget.y = 0f;

        float distanceToTarget = directionToTarget.magnitude;
        float impactFacingAngle = Vector3.Angle(transform.forward, directionToTarget);

        return distanceToTarget <= currentAttackData.ImpactRange && impactFacingAngle <= currentAttackData.MaximumImpactFacingAngle;
    }

    public void ApplyDamage(PlayerHitReceiver target)
    {
        if (!IsImpactValid(target))
        {
            return;
        }

        Vector3 incomingDirection = target.transform.position - transform.position;

        HitContext hitContext = new HitContext(currentAttackData.Damage, transform, incomingDirection);

        HitResult hitResult = target.ReceiveHit(hitContext);

        if (currentPhase != EnemyAttackPhase.HitWindow || enemyStateController.CurrentState != EnemyState.Attacking)
        {
            return;
        }

        if (hitResult == HitResult.PerfectGuard)
        {
            enemyStateController.TryStartPerfectGuardStagger();
        }
    }
}

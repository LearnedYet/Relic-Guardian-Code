using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyAttackData attackData = new EnemyAttackData();
    [SerializeField] private GameObject startupTelegraph;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyStateController enemyStateController;

    private EnemyAttackPhase currentPhase;
    private PlayerHitReceiver currentAttackTarget;
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
            && phaseElapsedTime >= attackData.StartupDuration - attackData.AnimationLeadTime)
        {
            hasAttackAnimationStarted = true;
            animator.Play(attackData.AnimationStateName, 0,0f);
        }

        UpdateAttackTracking();
        UpdateAttackMovement();

        if (currentPhase == EnemyAttackPhase.Startup && phaseElapsedTime >= attackData.StartupDuration)
        {
            OpenHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.HitWindow && phaseElapsedTime >= attackData.HitWindowDuration)
        {
            CloseHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.Recovery && phaseElapsedTime >= attackData.RecoveryDuration)
        {
            FinishRecovery();
        }
    }

    private void UpdateAttackTracking()
    {
        if (!hasAttackAnimationStarted
            || attackAnimationElapsedTime < attackData.MovementStartTime
            || attackAnimationElapsedTime >= attackData.TrackingEndTime
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
            || attackData.MovementEndTime <= attackData.MovementStartTime
            || attackData.MovementDistance <= 0f)
        {
            return;
        }

        float previousProgress = Mathf.InverseLerp(
            attackData.MovementStartTime,
            attackData.MovementEndTime,
            attackAnimationElapsedTime);

        attackAnimationElapsedTime += Time.deltaTime;

        float currentProgress = Mathf.InverseLerp(
            attackData.MovementStartTime,
            attackData.MovementEndTime,
            attackAnimationElapsedTime);

        float frameDistance = attackData.MovementDistance * (currentProgress - previousProgress);

        enemyMovement.MoveDuringAttack(transform.forward, frameDistance);
    }

    public bool TryStartAttack(PlayerHitReceiver target)
    {
        if (currentPhase == EnemyAttackPhase.Ready && target != null)
        {
            currentAttackTarget = target;
            currentPhase = EnemyAttackPhase.Startup;
            phaseElapsedTime = 0f;
            hasAttackAnimationStarted = false;
            attackAnimationElapsedTime = 0f;

            Vector3 incomingDirection = target.transform.position - transform.position;
            float expectedImpactTime = Time.time + attackData.StartupDuration;

            AttackThreatContext attackThreatContext = new AttackThreatContext(transform, incomingDirection, expectedImpactTime);

            target.ReceiveAttackThreat(attackThreatContext);
            startupTelegraph.SetActive(true);
            return true;
        }

        return false;
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

    public void ApplyDamage(PlayerHitReceiver target)
    {
        if (target == null)
        {
            return;
        }

        Vector3 incomingDirection = target.transform.position - transform.position;

        HitContext hitContext = new HitContext(attackData.Damage, transform, incomingDirection);

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

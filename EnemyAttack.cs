using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float startupDuration = 0.5f;
    [SerializeField] private float hitWindowDuration = 0.2f;
    [SerializeField] private float recoveryDuration = 0.4f;
    [SerializeField] private GameObject startupTelegraph;
    [SerializeField] private float attackAnimationLeadTime = 0.15f;
    [SerializeField] private Animator animator;

    private EnemyAttackPhase currentPhase;
    private PlayerHitReceiver currentAttackTarget;
    private float phaseElapsedTime;
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
            && phaseElapsedTime >= startupDuration - attackAnimationLeadTime)
        {
            hasAttackAnimationStarted = true;
            animator.SetTrigger("Attack");
        }

        if (currentPhase == EnemyAttackPhase.Startup && phaseElapsedTime >= startupDuration)
        {
            OpenHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.HitWindow && phaseElapsedTime >= hitWindowDuration)
        {
            CloseHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.Recovery && phaseElapsedTime >= recoveryDuration)
        {
            FinishRecovery();
        }
    }

    public bool TryStartAttack(PlayerHitReceiver target)
    {
        if (currentPhase == EnemyAttackPhase.Ready && target != null)
        {
            currentAttackTarget = target;
            currentPhase = EnemyAttackPhase.Startup;
            phaseElapsedTime = 0f;
            hasAttackAnimationStarted = false;

            Vector3 incomingDirection = target.transform.position - transform.position;
            float expectedImpactTime = Time.time + startupDuration;

            AttackThreatContext attackThreatContext = new AttackThreatContext(transform, incomingDirection, expectedImpactTime);

            target.ReceiveAttackThreat(attackThreatContext);
            startupTelegraph.SetActive(true);
            return true;
        }

        return false;
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
            currentPhase = EnemyAttackPhase.Ready;
            phaseElapsedTime = 0f;
            currentAttackTarget = null;
        }
    }

    public void ApplyDamage(PlayerHitReceiver target)
    {
        if (target == null)
        {
            return;
        }

        Vector3 incomingDirection = target.transform.position - transform.position;

        HitContext hitContext = new HitContext(attackDamage, transform, incomingDirection);

        target.ReceiveHit(hitContext);
    }
}

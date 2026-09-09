using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private EnemyAnimator enemyAnimator;
    [SerializeField] private float hitReactionDuration = 0.3f;
    [SerializeField] private float hitReactionCooldown = 0.3f;
    [SerializeField] private float globalAttackCooldownDuration = 1f;

    private EnemyState currentState = EnemyState.Chase;
    public EnemyState CurrentState => currentState;
    private float hitReactionEndTime;
    private float nextHitReactionAllowedTime;
    private float nextAttackAllowedTime;

    private void Update()
    {
        if (currentState == EnemyState.Staggered && Time.time >= hitReactionEndTime)
        {
            currentState = EnemyState.Chase;
            nextHitReactionAllowedTime = Time.time + hitReactionCooldown;
        }
    }

    public void StartGlobalAttackCooldown()
    {
        nextAttackAllowedTime = Mathf.Max(nextAttackAllowedTime, Time.time + globalAttackCooldownDuration);
    }

    public bool TryStartAttack(PlayerHitReceiver target)
    {
        if (currentState != EnemyState.Chase || Time.time < nextAttackAllowedTime)
        {
            return false;
        }

        if (!enemyAttack.TryStartAttack(target))
        {
            return false;
        }

        currentState = EnemyState.Attacking;
        return true;
    }

    public void FinishAttack()
    {
        if (currentState == EnemyState.Attacking)
        {
            currentState = EnemyState.Chase;
        }
    }

    public bool TryStartHitReaction()
    {
        if (currentState == EnemyState.Staggered)
        {
            enemyAnimator.PlayHitReaction();
            return false;
        }

        if (currentState != EnemyState.Chase || Time.time < nextHitReactionAllowedTime)
        {
            return false;
        }

        currentState = EnemyState.Staggered;
        hitReactionEndTime = Time.time + hitReactionDuration;
        enemyAnimator.PlayHitReaction();
        return true;
    }
}

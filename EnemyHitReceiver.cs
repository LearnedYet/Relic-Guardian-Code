using UnityEngine;

public class EnemyHitReceiver : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private EnemyHitPresentation enemyHitPresentation;
    private EnemyStateController enemyStateController;

    public bool CanReceiveHit => isActiveAndEnabled && enemyHealth != null && enemyHealth.IsAlive && (enemyStateController == null || enemyStateController.CurrentState != EnemyState.Dead);

    public static bool IsValidTarget(Collider target)
    {
        if (target == null || !target.enabled)
        {
            return false;
        }

        EnemyHitReceiver receiver = target.GetComponent<EnemyHitReceiver>();
        return receiver != null && receiver.CanReceiveHit;
    }

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHitPresentation = GetComponent<EnemyHitPresentation>();
        enemyStateController = GetComponent<EnemyStateController>();
    }

    public void ReceiveHit(HitContext hitContext)
    {
        if (!CanReceiveHit)
        {
            return;
        }

        enemyHealth.TakeDamage(hitContext.DamageAmount);

        if (!enemyHealth.IsAlive)
        {
            if (enemyStateController != null)
            {
                enemyStateController.EnterDead();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        if (enemyHitPresentation != null)
        {
            enemyHitPresentation.PresentHit();
        }

        if (enemyHealth.IsAlive && enemyStateController != null)
        {
            enemyStateController.TryStartHitReaction(hitContext.IncomingDirection);
        }
    }
}

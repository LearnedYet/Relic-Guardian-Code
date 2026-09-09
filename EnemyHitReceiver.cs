using UnityEngine;

public class EnemyHitReceiver : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private EnemyHitPresentation enemyHitPresentation;
    private EnemyStateController enemyStateController;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHitPresentation = GetComponent<EnemyHitPresentation>();
        enemyStateController = GetComponent<EnemyStateController>();
    }

    public void ReceiveHit(HitContext hitContext)
    {
        enemyHealth.TakeDamage(hitContext.DamageAmount);

        if (enemyHitPresentation != null)
        {
            enemyHitPresentation.PresentHit();
        }

        if (enemyHealth.IsAlive && enemyStateController != null)
        {
            enemyStateController.TryStartHitReaction();
        }
    }
}

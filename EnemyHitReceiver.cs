using UnityEngine;

public class EnemyHitReceiver : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private EnemyHitPresentation enemyHitPresentation;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHitPresentation = GetComponent<EnemyHitPresentation>();
    }

    public void ReceiveHit(HitContext hitContext)
    {
        enemyHealth.TakeDamage(hitContext.DamageAmount);

        if (enemyHitPresentation != null)
        {
            enemyHitPresentation.PresentHit();
        }
    }
}

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyStateController enemyStateController;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private PlayerHitReceiver attackTarget;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maximumAttackFacingAngle = 15f;

    private void Update()
    {
        if (enemyStateController.CurrentState != EnemyState.Chase)
        {
            enemyMovement.Stop();
            return;
        }
        Vector3 directionToTarget = attackTarget.transform.position - transform.position;
        directionToTarget.y = 0f;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= attackRange)
        {
            enemyMovement.Stop();
            float facingAngle = Vector3.Angle(transform.forward, directionToTarget);

            if (facingAngle <= maximumAttackFacingAngle)
            {
                enemyStateController.TryStartAttack(attackTarget);
            }
            else
            {
                enemyMovement.Turn(directionToTarget);
            }
        }
        else
        {
            enemyMovement.Move(directionToTarget);
        }
    }
}

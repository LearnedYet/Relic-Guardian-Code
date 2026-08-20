using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private PlayerHealth attackTarget;
    [SerializeField] private float attackRange = 2f;

    private void Update()
    {
        if (enemyAttack.CurrentPhase != EnemyAttackPhase.Ready)
        {
            enemyMovement.Stop();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.transform.position);

        if (distanceToTarget <= attackRange)
        {
            enemyMovement.Stop();
            enemyAttack.TryStartAttack(attackTarget);
        }
        else
        {
            Vector3 directionToTarget = attackTarget.transform.position - transform.position;
            enemyMovement.Move(directionToTarget);
        }
    }
}

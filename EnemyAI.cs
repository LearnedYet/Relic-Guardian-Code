using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private PlayerHealth attackTarget;
    [SerializeField] private float attackRange = 2f;

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.transform.position);

        if (distanceToTarget <= attackRange)
        {
            enemyAttack.TryStartAttack(attackTarget);
        }
    }
}

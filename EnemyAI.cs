using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private PlayerHealth attackTarget;

    private void Update()
    {
        enemyAttack.TryStartAttack(attackTarget);
    }
}

using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyMovement enemyMovement;

    private void Update()
    {
        animator.SetFloat("Speed", enemyMovement.CurrentHorizontalSpeed);
    }
}

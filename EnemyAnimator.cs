using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField, Min(0f)] private float locomotionDampTime = 0.12f;

    private bool isPlayingPerfectGuardStagger;

    private void Update()
    {
        Vector3 localHorizontalVelocity =
            enemyMovement.CurrentLocalHorizontalVelocity;

        animator.SetFloat("Speed", localHorizontalVelocity.magnitude, locomotionDampTime, Time.deltaTime);
        animator.SetFloat("MoveX", localHorizontalVelocity.x, locomotionDampTime, Time.deltaTime);
        animator.SetFloat("MoveZ", localHorizontalVelocity.z, locomotionDampTime, Time.deltaTime);
    }

    public void PlayHitReaction()
    {
        if (isPlayingPerfectGuardStagger)
        {
            PlayPerfectGuardStagger();
            return;
        }
        animator.SetTrigger("HitReaction");
    }

    public void PlayPerfectGuardStagger()
    {
        isPlayingPerfectGuardStagger = true;
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("HitReaction");
        animator.Play("Base Layer.PerfectGuardStagger",0,0f);
    }

    public void FinishPerfectGuardStagger()
    {
        if (!isPlayingPerfectGuardStagger)
        {
            return;
        }

        isPlayingPerfectGuardStagger = false;
        animator.ResetTrigger("HitReaction");
        animator.CrossFadeInFixedTime("Base Layer.IdleSwordShield",0.1f, 0, 0f);
    }

    public void PlayDeath()
    {
        isPlayingPerfectGuardStagger = false;
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("HitReaction");
        animator.Play("Base Layer.DeathSwordShield", 0, 0f);
    }
}

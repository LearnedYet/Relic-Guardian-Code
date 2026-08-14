using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float startupDuration = 0.5f;
    [SerializeField] private float hitWindowDuration = 0.2f;
    [SerializeField] private float recoveryDuration = 0.4f;

    private EnemyAttackPhase currentPhase;
    private PlayerHealth currentAttackTarget;
    private float phaseElapsedTime;

    public EnemyAttackPhase CurrentPhase => currentPhase;

    private void Update()
    {
        if (currentPhase != EnemyAttackPhase.Ready)
        {
            phaseElapsedTime += Time.deltaTime;
        }
        if (currentPhase == EnemyAttackPhase.Startup && phaseElapsedTime >= startupDuration)
        {
            OpenHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.HitWindow && phaseElapsedTime >= hitWindowDuration)
        {
            CloseHitWindow();
        }
        else if (currentPhase == EnemyAttackPhase.Recovery && phaseElapsedTime >= recoveryDuration)
        {
            FinishRecovery();
        }
    }

    public bool TryStartAttack(PlayerHealth target)
    {
        if (currentPhase == EnemyAttackPhase.Ready && target != null)
        {
            currentAttackTarget = target;
            currentPhase = EnemyAttackPhase.Startup;
            phaseElapsedTime = 0f;
            return true;
        }

        return false;
    }

    public void OpenHitWindow()
    {
        if (currentPhase == EnemyAttackPhase.Startup)
        {
            currentPhase = EnemyAttackPhase.HitWindow;
            phaseElapsedTime = 0f;
            ApplyDamage(currentAttackTarget);
        }
    }

    public void CloseHitWindow()
    {
        if (currentPhase == EnemyAttackPhase.HitWindow)
        {
            currentPhase = EnemyAttackPhase.Recovery;
            phaseElapsedTime = 0f;
        }
    }

    public void FinishRecovery()
    {
        if (currentPhase == EnemyAttackPhase.Recovery)
        {
            currentPhase = EnemyAttackPhase.Ready;
            phaseElapsedTime = 0f;
            currentAttackTarget = null;
        }
    }

    public void ApplyDamage(PlayerHealth target)
    {
        if (target == null)
        {
            return;
        }

        target.TakeDamage(attackDamage);
    }
}

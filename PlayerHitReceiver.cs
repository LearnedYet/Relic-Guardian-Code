using System.Collections.Generic;
using UnityEngine;

public class PlayerHitReceiver : MonoBehaviour
{
    private PlayerActionController playerActionController;
    private PlayerHealth playerHealth;
    private PlayerBlock playerBlock;
    private PlayerDodge playerDodge;
    private PlayerDodgePresentation playerDodgePresentation;
    private PlayerGuardPresentation playerGuardPresentation;
    private readonly Dictionary<Transform, AttackThreatContext> activeAttackThreats = new Dictionary<Transform, AttackThreatContext>();

    private void Awake()
    {
        playerBlock = GetComponent<PlayerBlock>();
        playerActionController = GetComponent<PlayerActionController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerGuardPresentation = GetComponent<PlayerGuardPresentation>();
        playerDodge = GetComponent<PlayerDodge>();
        playerDodgePresentation = GetComponent<PlayerDodgePresentation>();
    }

    public void ReceiveAttackThreat(AttackThreatContext attackThreatContext)
    {
        if (attackThreatContext.Source == null)
        {
            return;
        }

        activeAttackThreats[attackThreatContext.Source] = attackThreatContext;

        if (playerActionController.CurrentActionState == PlayerActionState.Blocking
            && TryGetNextAttackThreat(
                out AttackThreatContext nextAttackThreat
            ))
        {
            playerBlock.TryStartFacingAssist(nextAttackThreat);
        }
    }

    public void RemoveAttackThreat(Transform source)
    {
        if (source == null)
        {
            return;
        }

        activeAttackThreats.Remove(source);
    }

    public bool TryGetNextAttackThreat(out AttackThreatContext attackThreatContext)
    {
        attackThreatContext = default;
        bool foundThreat = false;

        foreach (AttackThreatContext candidateThreat in activeAttackThreats.Values)
        {
            if (candidateThreat.Source == null || candidateThreat.ExpectedImpactTime <= Time.time)
            {
                continue;
            }

            if (!foundThreat || candidateThreat.ExpectedImpactTime < attackThreatContext.ExpectedImpactTime)
            {
                attackThreatContext = candidateThreat;
                foundThreat = true;
            }
        }

        return foundThreat;
    }

    public HitResult ReceiveHit(HitContext hitContext)
    {
        playerActionController.ResolveActionRequests();

        if (playerActionController.CurrentActionState == PlayerActionState.Dodging)
        {
            DodgeResult dodgeResult = playerDodge.ResolveDodgeHit();

            if (dodgeResult == DodgeResult.Perfect)
            {
                if (playerDodgePresentation != null)
                {
                    playerDodgePresentation.PresentPerfectDodge();
                }
                return HitResult.PerfectDodge;
            }

            if (dodgeResult == DodgeResult.Ordinary)
            {
                return HitResult.OrdinaryDodge;
            }
        }

        if (playerActionController.CurrentActionState == PlayerActionState.Blocking)
        {
            GuardResult guardResult = playerBlock.ResolveGuardHit(hitContext);

            if (guardResult != GuardResult.Unhandled)
            {
                playerGuardPresentation.PresentGuardResult(guardResult, hitContext.IncomingDirection);

                if (guardResult == GuardResult.Perfect)
                {
                    return HitResult.PerfectGuard;
                }

                return HitResult.OrdinaryGuard;
            }
        }

        playerHealth.TakeDamage(hitContext.DamageAmount);
        return HitResult.Damaged;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PlayerHitReceiver : MonoBehaviour
{
    private PlayerActionController playerActionController;
    private PlayerHealth playerHealth;
    private PlayerBlock playerBlock;
    private PlayerGuardPresentation playerGuardPresentation;
    private readonly Dictionary<Transform, AttackThreatContext> activeAttackThreats = new Dictionary<Transform, AttackThreatContext>();

    private void Awake()
    {
        playerBlock = GetComponent<PlayerBlock>();
        playerActionController = GetComponent<PlayerActionController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerGuardPresentation = GetComponent<PlayerGuardPresentation>();
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

    public void ReceiveHit(HitContext hitContext)
    {
        playerActionController.ResolveActionRequests();

        if (playerActionController.CurrentActionState == PlayerActionState.Blocking)
        {
            GuardResult guardResult = playerBlock.ResolveGuardHit(hitContext);

            if (guardResult != GuardResult.Unhandled)
            {
                playerGuardPresentation.PresentGuardResult(guardResult, hitContext.IncomingDirection);
                return;
            }
        }

        playerHealth.TakeDamage(hitContext.DamageAmount);
    }
}

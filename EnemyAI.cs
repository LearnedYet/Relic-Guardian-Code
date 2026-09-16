using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum SpacingBehavior
    {
        Run,
        Approach,
        Retreat,
        Strafe,
        Wait
    }

    [SerializeField] private EnemyStateController enemyStateController;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private PlayerHitReceiver attackTarget;
    [SerializeField] private EnemySpacingData spacingData;

    private SpacingBehavior currentSpacingBehavior = SpacingBehavior.Approach;
    private float spacingBehaviorEndTime;
    private int strafeDirectionSign = 1;

    private void Update()
    {
        if (enemyStateController.CurrentState != EnemyState.Chase)
        {
            currentSpacingBehavior = SpacingBehavior.Approach;
            enemyMovement.Stop();
            return;
        }
        Vector3 directionToTarget = attackTarget.transform.position - transform.position;
        directionToTarget.y = 0f;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= spacingData.AttackRange)
        {
            UpdateInsideAttackRange(directionToTarget, distanceToTarget);
        }
        else
        {
            UpdateForwardSpacingDecision(directionToTarget, distanceToTarget);
        }
    }

    private void UpdateForwardSpacingDecision(
        Vector3 directionToTarget,
        float distanceToTarget)
    {
        if (distanceToTarget >= spacingData.RunEnterDistance)
        {
            currentSpacingBehavior = SpacingBehavior.Run;
        }
        else if (distanceToTarget <= spacingData.ApproachEnterDistance)
        {
            currentSpacingBehavior = SpacingBehavior.Approach;
        }
        else if (currentSpacingBehavior != SpacingBehavior.Run &&
                 currentSpacingBehavior != SpacingBehavior.Approach)
        {
            currentSpacingBehavior = SpacingBehavior.Approach;
        }

        float speedMultiplier =
            currentSpacingBehavior == SpacingBehavior.Run
                ? spacingData.RunSpeedMultiplier
                : spacingData.ApproachSpeedMultiplier;

        enemyMovement.Move(
            directionToTarget,
            directionToTarget,
            speedMultiplier);
    }

    private void UpdateInsideAttackRange(
        Vector3 directionToTarget,
        float distanceToTarget)
    {
        float facingAngle = Vector3.Angle(transform.forward, directionToTarget);

        if (facingAngle > spacingData.MaximumAttackFacingAngle)
        {
            enemyMovement.Stop();
            enemyMovement.Turn(directionToTarget);
            return;
        }

        if (enemyStateController.TryStartAttack(attackTarget))
        {
            enemyMovement.Stop();
            return;
        }

        if (TryHandleRetreat(directionToTarget, distanceToTarget))
        {
            return;
        }

        UpdateFallbackSpacing(directionToTarget);
    }

    private void UpdateFallbackSpacing(Vector3 directionToTarget)
    {
        if (currentSpacingBehavior == SpacingBehavior.Strafe &&
            Time.time >= spacingBehaviorEndTime)
        {
            EnterWait();
        }
        else if (currentSpacingBehavior == SpacingBehavior.Wait &&
                 Time.time >= spacingBehaviorEndTime)
        {
            EnterStrafe();
        }
        else if (currentSpacingBehavior != SpacingBehavior.Strafe &&
                 currentSpacingBehavior != SpacingBehavior.Wait)
        {
            EnterStrafe();
        }

        if (currentSpacingBehavior == SpacingBehavior.Strafe)
        {
            Vector3 strafeDirection =
                Vector3.Cross(Vector3.up, directionToTarget) * strafeDirectionSign;

            enemyMovement.Move(
                strafeDirection,
                directionToTarget,
                spacingData.StrafeSpeedMultiplier);
            return;
        }

        enemyMovement.Stop();
    }

    private void EnterStrafe()
    {
        currentSpacingBehavior = SpacingBehavior.Strafe;
        strafeDirectionSign = Random.Range(0, 2) == 0 ? -1 : 1;
        spacingBehaviorEndTime =
            Time.time + spacingData.StrafeDuration;
    }

    private void EnterWait()
    {
        currentSpacingBehavior = SpacingBehavior.Wait;
        spacingBehaviorEndTime =
            Time.time + Random.Range(
                spacingData.MinimumWaitDuration,
                spacingData.MaximumWaitDuration);
    }

    private bool TryHandleRetreat(
        Vector3 directionToTarget,
        float distanceToTarget)
    {
        if (currentSpacingBehavior != SpacingBehavior.Retreat &&
            distanceToTarget > spacingData.RetreatEnterDistance)
        {
            return false;
        }

        currentSpacingBehavior = SpacingBehavior.Retreat;

        if (distanceToTarget >= spacingData.RetreatExitDistance)
        {
            EnterWait();
            enemyMovement.Stop();
            return true;
        }

        enemyMovement.Move(
            -directionToTarget,
            directionToTarget,
            spacingData.RetreatSpeedMultiplier);

        return true;
    }
}

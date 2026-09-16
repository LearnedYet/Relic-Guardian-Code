using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemySpacingData",
    menuName = "Relic Guardian/Enemy/Enemy Spacing Data")]
public class EnemySpacingData : ScriptableObject
{
    [Header("Attack Admission")]
    [SerializeField, Min(0f)] private float attackRange = 2.2f;
    [SerializeField, Range(0f, 180f)] private float maximumAttackFacingAngle = 15f;

    [Header("Distance Bands")]
    [SerializeField, Min(0f)] private float retreatEnterDistance = 1f;
    [SerializeField, Min(0f)] private float retreatExitDistance = 1.6f;
    [SerializeField, Min(0f)] private float approachEnterDistance = 1.5f;
    [SerializeField, Min(0f)] private float runEnterDistance = 5f;

    [Header("Behavior Timing")]
    [SerializeField, Min(0f)] private float strafeDuration = 1.75f;
    [SerializeField, Min(0f)] private float minimumWaitDuration = 1.5f;
    [SerializeField, Min(0f)] private float maximumWaitDuration = 2f;

    [Header("Speed Multipliers")]
    [SerializeField, Min(0f)] private float runSpeedMultiplier = 1f;
    [SerializeField, Min(0f)] private float approachSpeedMultiplier = 0.5f;
    [SerializeField, Min(0f)] private float retreatSpeedMultiplier = 0.25f;
    [SerializeField, Min(0f)] private float strafeSpeedMultiplier = 0.35f;

    public float AttackRange => attackRange;
    public float MaximumAttackFacingAngle => maximumAttackFacingAngle;
    public float RetreatEnterDistance => retreatEnterDistance;
    public float RetreatExitDistance => retreatExitDistance;
    public float ApproachEnterDistance => approachEnterDistance;
    public float RunEnterDistance => runEnterDistance;
    public float StrafeDuration => strafeDuration;
    public float MinimumWaitDuration => minimumWaitDuration;
    public float MaximumWaitDuration => maximumWaitDuration;
    public float RunSpeedMultiplier => runSpeedMultiplier;
    public float ApproachSpeedMultiplier => approachSpeedMultiplier;
    public float RetreatSpeedMultiplier => retreatSpeedMultiplier;
    public float StrafeSpeedMultiplier => strafeSpeedMultiplier;
}

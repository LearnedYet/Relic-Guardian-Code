using UnityEngine;

[CreateAssetMenu(
    fileName = "MeleeAttackData",
    menuName = "Relic Guardian/Enemy/Melee Attack Data")]
public class MeleeAttackData : ScriptableObject
{
    [Header("Phase Timing")]
    [SerializeField] private float startupDuration = 0.5f;
    [SerializeField] private float hitWindowDuration = 0.2f;
    [SerializeField] private float recoveryDuration = 0.4f;

    [Header("Animation")]
    [SerializeField] private string animationStateName = "Base Layer.Attack1SwordShield";
    [SerializeField] private float animationLeadTime = 0.15f;

    [Header("Motion")]
    [SerializeField] private float movementStartTime = 1f / 30f;
    [SerializeField] private float trackingStartTime = 1f / 30f;
    [SerializeField] private float trackingEndTime = 2f / 30f;
    [SerializeField] private float movementEndTime = 10f / 30f;
    [SerializeField] private float movementDistance = 0.6f;

    [Header("Impact")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float impactRange = 2f;
    [SerializeField] private float maximumImpactFacingAngle = 30f;

    [Header("Selection")]
    [SerializeField] private float minimumRange = 0f;
    [SerializeField] private float maximumRange = 2f;

    public float MinimumRange => minimumRange;
    public float MaximumRange => maximumRange;
    public int Damage => damage;
    public float StartupDuration => startupDuration;
    public float HitWindowDuration => hitWindowDuration;
    public float RecoveryDuration => recoveryDuration;
    public float AnimationLeadTime => animationLeadTime;
    public float MovementStartTime => movementStartTime;
    public float TrackingStartTime => trackingStartTime;
    public float TrackingEndTime => trackingEndTime;
    public float MovementEndTime => movementEndTime;
    public float MovementDistance => movementDistance;
    public float ImpactRange => impactRange;
    public float MaximumImpactFacingAngle => maximumImpactFacingAngle;
    public string AnimationStateName => animationStateName;
}

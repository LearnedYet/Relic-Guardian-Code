using UnityEngine;

[System.Serializable]
public class EnemyAttackData
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float startupDuration = 0.5f;
    [SerializeField] private float hitWindowDuration = 0.2f;
    [SerializeField] private float recoveryDuration = 0.4f;
    [SerializeField] private float movementStartTime = 1f / 30f;
    [SerializeField] private float trackingEndTime = 2f / 30f;
    [SerializeField] private float movementEndTime = 10f / 30f;
    [SerializeField] private float movementDistance = 0.6f;
    [SerializeField] private float animationLeadTime = 0.15f;
    [SerializeField] private string animationStateName = "Base Layer.Attack1SwordShield";

    public int Damage => damage;
    public float StartupDuration => startupDuration;
    public float HitWindowDuration => hitWindowDuration;
    public float RecoveryDuration => recoveryDuration;
    public float AnimationLeadTime => animationLeadTime;
    public float MovementStartTime => movementStartTime;
    public float TrackingEndTime => trackingEndTime;
    public float MovementEndTime => movementEndTime;
    public float MovementDistance => movementDistance;
    public string AnimationStateName => animationStateName;
}

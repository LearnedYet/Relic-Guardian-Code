using UnityEngine;

public readonly struct HitContext
{
    public readonly int DamageAmount;
    public readonly Transform Source;
    public readonly Vector3 IncomingDirection;

    public HitContext(int damageAmount, Transform source, Vector3 incomingDirection)
    {
        DamageAmount = damageAmount;
        Source = source;
        IncomingDirection = incomingDirection.normalized;
    }
}

using UnityEngine;

public readonly struct AttackThreatContext
{
    public readonly Transform Source;
    public readonly Vector3 IncomingDirection;
    public readonly float ExpectedImpactTime;

    public AttackThreatContext(Transform source, Vector3 incomingDirection, float expectedImpactTime)
    {
        Source = source;
        IncomingDirection = incomingDirection.normalized;
        ExpectedImpactTime = expectedImpactTime;
    }
}

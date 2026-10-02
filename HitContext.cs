using UnityEngine;

public readonly struct HitContext
{
    public readonly int DamageAmount;
    public readonly Transform Source;
    public readonly Vector3 IncomingDirection;
    public readonly HitFeedbackType FeedbackType;
    public readonly int HitIndex;

    public HitContext(int damageAmount, Transform source, Vector3 incomingDirection, HitFeedbackType feedbackType = HitFeedbackType.Default, int hitIndex = 0)
    {
        DamageAmount = damageAmount;
        Source = source;
        IncomingDirection = incomingDirection.normalized;
        FeedbackType = feedbackType;
        HitIndex = hitIndex;
    }
}

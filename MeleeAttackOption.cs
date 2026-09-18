using UnityEngine;

[System.Serializable]
public class MeleeAttackOption
{
    [SerializeField] private MeleeAttackData attackData;
    [SerializeField] private float weight = 1f;

    public MeleeAttackData AttackData => attackData;
    public float Weight => weight;
}

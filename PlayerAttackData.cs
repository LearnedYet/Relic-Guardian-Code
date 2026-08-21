using UnityEngine;

[System.Serializable]
public class PlayerAttackData
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float targetRange = 2f;
    [SerializeField] private float lungeSpeed = 5f;
    [SerializeField] private float lungeDistance = 1f;

    public int Damage
    {
        get { return damage; }
    }

    public float TargetRange
    {
        get { return targetRange; }
    }

    public float LungeSpeed
    {
        get { return lungeSpeed; }
    }

    public float LungeDistance
    {
        get { return lungeDistance; }
    }
}

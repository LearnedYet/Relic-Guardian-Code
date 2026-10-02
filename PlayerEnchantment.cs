using UnityEngine;

public class PlayerEnchantment : MonoBehaviour
{
    [SerializeField] private float enchantmentDuration;

    private float enchantmentEndTime;

    public bool HasEnchantment
    {
        get { return Time.time < enchantmentEndTime; }
    }

    public void RefreshEnchantment()
    {
        enchantmentEndTime = Time.time + Mathf.Max(0f, enchantmentDuration);
    }
}

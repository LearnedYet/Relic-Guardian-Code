using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth = 3;

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
    }
}

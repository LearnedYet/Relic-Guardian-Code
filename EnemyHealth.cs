using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth = 3;

    public bool IsAlive => currentHealth > 0;

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
    }
}

using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool destroyOnDeath = true;

    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        Debug.Log($"{name} took {amount} damage. HP: {_currentHealth}/{maxHealth}");

        if (_currentHealth == 0 && destroyOnDeath)
        {
            Debug.Log($"{name} died.");
            Destroy(gameObject);
        }
    }
}

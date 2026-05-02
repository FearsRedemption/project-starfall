using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.78f, 0.56f);
    [SerializeField] private float hitFlashDuration = 0.08f;

    private int _currentHealth;
    private MeshRenderer[] _renderers;
    private Color[] _baseColors;
    private float _flashUntil;

    public float Health01 => maxHealth > 0 ? Mathf.Clamp01((float)_currentHealth / maxHealth) : 0f;
    public bool IsDead => _currentHealth <= 0;

    private void Awake()
    {
        _currentHealth = maxHealth;
        CacheRenderers();
    }

    private void Update()
    {
        UpdateHitFlash();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        FlashOnHit();
        Debug.Log($"Player took {amount} damage. HP: {_currentHealth}/{maxHealth}");

        if (IsDead)
            Debug.Log("Player died.");
    }

    private void CacheRenderers()
    {
        _renderers = GetComponentsInChildren<MeshRenderer>();
        _baseColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
            _baseColors[i] = _renderers[i].material.color;
    }

    private void FlashOnHit()
    {
        _flashUntil = Time.time + hitFlashDuration;
        SetRendererColors(hitFlashColor);
    }

    private void UpdateHitFlash()
    {
        if (_flashUntil <= 0f || Time.time < _flashUntil)
            return;

        _flashUntil = 0f;
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i])
                _renderers[i].material.color = _baseColors[i];
        }
    }

    private void SetRendererColors(Color color)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i])
                _renderers[i].material.color = color;
        }
    }
}

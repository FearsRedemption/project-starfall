using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Scene Feedback")]
    [SerializeField] private Transform healthBarRoot;
    [SerializeField] private Transform healthBarFill;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 1.25f, 0f);
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.82f, 0.52f);
    [SerializeField] private float hitFlashDuration = 0.08f;

    private int _currentHealth;
    private MeshRenderer[] _renderers;
    private Color[] _baseColors;
    private Vector3 _healthBarFillFullScale;
    private Camera _mainCamera;
    private float _flashUntil;

    public float Health01 => maxHealth > 0 ? Mathf.Clamp01((float)_currentHealth / maxHealth) : 0f;
    public bool IsDead => _currentHealth <= 0;

    private void Awake()
    {
        _currentHealth = maxHealth;
        CacheRenderers();
        if (healthBarFill)
            _healthBarFillFullScale = healthBarFill.localScale;
    }

    private void Update()
    {
        UpdateHitFlash();
    }

    private void LateUpdate()
    {
        if (!healthBarRoot)
            return;

        if (!_mainCamera)
            _mainCamera = Camera.main;

        if (!_mainCamera)
            return;

        healthBarRoot.position = transform.position + healthBarOffset;
        healthBarRoot.rotation = Quaternion.LookRotation(healthBarRoot.position - _mainCamera.transform.position, Vector3.up);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        UpdateHealthBar();
        FlashOnHit();
        Debug.Log($"{name} took {amount} damage. HP: {_currentHealth}/{maxHealth}");

        if (IsDead && destroyOnDeath)
        {
            Debug.Log($"{name} died.");
            Destroy(gameObject);
        }
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

    private void UpdateHealthBar()
    {
        if (!healthBarFill)
            return;

        float healthPercent = maxHealth > 0 ? (float)_currentHealth / maxHealth : 0f;
        healthBarFill.localScale = new Vector3(_healthBarFillFullScale.x * healthPercent, _healthBarFillFullScale.y, _healthBarFillFullScale.z);
        healthBarFill.localPosition = new Vector3(-_healthBarFillFullScale.x * 0.5f * (1f - healthPercent), healthBarFill.localPosition.y, healthBarFill.localPosition.z);
    }
}

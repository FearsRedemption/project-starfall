using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private bool showHealthBar = true;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 1.25f, 0f);
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.82f, 0.52f);
    [SerializeField] private float hitFlashDuration = 0.08f;

    private int _currentHealth;
    private Transform _healthBarRoot;
    private Transform _healthBarFill;
    private MeshRenderer[] _renderers;
    private Color[] _baseColors;
    private float _flashUntil;

    private void Awake()
    {
        _currentHealth = maxHealth;
        CacheRenderers();

        if (showHealthBar)
            CreateHealthBar();
    }

    private void Update()
    {
        UpdateHitFlash();
    }

    private void LateUpdate()
    {
        if (!_healthBarRoot || !Camera.main)
            return;

        _healthBarRoot.position = transform.position + healthBarOffset;
        _healthBarRoot.rotation = Quaternion.LookRotation(_healthBarRoot.position - Camera.main.transform.position, Vector3.up);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        UpdateHealthBar();
        FlashOnHit();
        Debug.Log($"{name} took {amount} damage. HP: {_currentHealth}/{maxHealth}");

        if (_currentHealth == 0 && destroyOnDeath)
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

    private void CreateHealthBar()
    {
        _healthBarRoot = new GameObject("HealthBar").transform;
        _healthBarRoot.SetParent(transform, false);
        _healthBarRoot.localPosition = healthBarOffset;

        Transform background = CreateBarPart("Background", _healthBarRoot, new Vector3(0.7f, 0.08f, 0.03f), Color.black);
        background.localPosition = Vector3.zero;

        _healthBarFill = CreateBarPart("Fill", _healthBarRoot, new Vector3(0.66f, 0.045f, 0.035f), new Color(0.75f, 0.16f, 0.1f));
        _healthBarFill.localPosition = new Vector3(0f, 0f, -0.01f);
    }

    private void UpdateHealthBar()
    {
        if (!_healthBarFill)
            return;

        float healthPercent = maxHealth > 0 ? (float)_currentHealth / maxHealth : 0f;
        _healthBarFill.localScale = new Vector3(0.66f * healthPercent, 0.045f, 0.035f);
        _healthBarFill.localPosition = new Vector3(-0.33f * (1f - healthPercent), 0f, -0.01f);
    }

    private static Transform CreateBarPart(string partName, Transform parent, Vector3 scale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = partName;
        part.transform.SetParent(parent, false);
        part.transform.localScale = scale;

        Collider collider = part.GetComponent<Collider>();
        if (collider)
            Destroy(collider);

        if (part.TryGetComponent(out MeshRenderer renderer))
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = partName;
            material.color = color;
            renderer.sharedMaterial = material;
        }

        return part.transform;
    }
}

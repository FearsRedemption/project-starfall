using UnityEngine;

public class TestEnemyAttackCue : MonoBehaviour
{
    [SerializeField] private Color readyColor = new Color(0.52f, 0.1f, 0.08f);
    [SerializeField] private Color attackColor = new Color(1f, 0.45f, 0.15f);
    [SerializeField] private float flashDuration = 0.12f;

    private MeshRenderer _renderer;
    private float _flashUntil;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (!_renderer)
            return;

        _renderer.material.color = Time.time < _flashUntil ? attackColor : readyColor;
    }

    public void Flash()
    {
        _flashUntil = Time.time + flashDuration;
    }
}

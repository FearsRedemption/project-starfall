using UnityEngine;

public class EnemyAttackCue : MonoBehaviour
{
    [SerializeField] private Color readyColor = new Color(0.52f, 0.1f, 0.08f);
    [SerializeField] private Color attackColor = new Color(1f, 0.45f, 0.15f);
    [SerializeField] private float flashDuration = 0.12f;

    private MeshRenderer _renderer;
    private Material _material;
    private bool _showingAttackColor;
    private float _flashUntil;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer ? _renderer.material : null;
        SetColor(false);
    }

    private void Update()
    {
        if (!_material)
            return;

        bool shouldShowAttackColor = Time.time < _flashUntil;
        if (shouldShowAttackColor == _showingAttackColor)
            return;

        SetColor(shouldShowAttackColor);
    }

    public void Flash()
    {
        _flashUntil = Time.time + flashDuration;
        SetColor(true);
    }

    private void SetColor(bool useAttackColor)
    {
        if (!_material)
            return;

        _showingAttackColor = useAttackColor;
        _material.color = useAttackColor ? attackColor : readyColor;
    }
}

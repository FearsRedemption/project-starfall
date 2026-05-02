using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerMove movement;

    [Header("Scene UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    private void Awake()
    {
        if (!health)
            health = GetComponent<PlayerHealth>();

        if (!movement)
            movement = GetComponent<PlayerMove>();
    }

    private void Update()
    {
        if (healthFill)
            healthFill.fillAmount = health ? health.Health01 : 0f;

        if (staminaFill)
            staminaFill.fillAmount = movement ? movement.Stamina01 : 0f;
    }
}

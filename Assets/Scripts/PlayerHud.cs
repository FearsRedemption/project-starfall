using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerMove movement;
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

    public void Bind(Image newHealthFill, Image newStaminaFill)
    {
        healthFill = newHealthFill;
        staminaFill = newStaminaFill;
    }
}

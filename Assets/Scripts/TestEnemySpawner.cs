using UnityEngine;

public class TestEnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset = new Vector3(3f, 0f, 4f);
    [SerializeField] private Color enemyColor = new Color(0.7f, 0.12f, 0.1f);

    private void Start()
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.name = "Test Enemy";
        enemy.transform.position = transform.position + spawnOffset;
        enemy.transform.localScale = new Vector3(0.9f, 1f, 0.9f);

        if (enemy.TryGetComponent(out MeshRenderer renderer))
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = "Test Enemy";
            material.color = enemyColor;
            renderer.sharedMaterial = material;
        }

        Rigidbody rb = enemy.AddComponent<Rigidbody>();
        rb.freezeRotation = true;

        enemy.AddComponent<Damageable>();
        TestEnemy testEnemy = enemy.AddComponent<TestEnemy>();
        testEnemy.SetTarget(transform);
    }
}

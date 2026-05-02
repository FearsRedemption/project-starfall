using UnityEngine;

public class TestEnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset = new Vector3(3f, 0f, 4f);
    [SerializeField] private Color enemyColor = new Color(0.52f, 0.1f, 0.08f);
    [SerializeField] private Color eyeColor = new Color(1f, 0.42f, 0.14f);

    private void Start()
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.name = "Training Husk";
        enemy.transform.position = transform.position + spawnOffset;
        enemy.transform.localScale = new Vector3(0.9f, 1.08f, 0.9f);

        if (enemy.TryGetComponent(out MeshRenderer renderer))
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = "Training Husk";
            material.color = enemyColor;
            renderer.sharedMaterial = material;
        }

        CreateEyeMarker(enemy.transform);

        Rigidbody rb = enemy.AddComponent<Rigidbody>();
        rb.freezeRotation = true;

        enemy.AddComponent<Damageable>();
        enemy.AddComponent<TestEnemyAttackCue>();
        TestEnemy testEnemy = enemy.AddComponent<TestEnemy>();
        testEnemy.SetTarget(transform);
    }

    private void CreateEyeMarker(Transform parent)
    {
        GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Cube);
        eye.name = "Threat Eye";
        eye.transform.SetParent(parent, false);
        eye.transform.localPosition = new Vector3(0f, 0.34f, 0.48f);
        eye.transform.localScale = new Vector3(0.34f, 0.08f, 0.04f);

        Collider collider = eye.GetComponent<Collider>();
        if (collider)
            Destroy(collider);

        if (eye.TryGetComponent(out MeshRenderer renderer))
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = "Threat Eye";
            material.color = eyeColor;
            renderer.sharedMaterial = material;
        }
    }
}

using UnityEngine;

public class TestArenaBuilder : MonoBehaviour
{
    [SerializeField] private Vector3 arenaCenter = new Vector3(0f, -0.1f, 5f);
    [SerializeField] private Vector2 arenaSize = new Vector2(22f, 18f);
    [SerializeField] private Color groundColor = new Color(0.31f, 0.34f, 0.28f);
    [SerializeField] private Color stoneColor = new Color(0.42f, 0.4f, 0.34f);
    [SerializeField] private Color targetColor = new Color(0.65f, 0.2f, 0.14f);
    [SerializeField] private bool hideOriginalGround = true;

    private const string ArenaRootName = "GeneratedTestArena";

    private void Start()
    {
        if (GameObject.Find(ArenaRootName))
            return;

        if (hideOriginalGround)
        {
            GameObject originalGround = GameObject.Find("Ground");
            if (originalGround)
                originalGround.SetActive(false);
        }

        Transform root = new GameObject(ArenaRootName).transform;
        Material ground = MakeMaterial("Arena Ground", groundColor);
        Material stone = MakeMaterial("Arena Stone", stoneColor);
        Material target = MakeMaterial("Arena Target", targetColor);

        CreateBlock("Movement Floor", root, arenaCenter, Vector3.zero, new Vector3(arenaSize.x, 0.25f, arenaSize.y), ground, false);

        float halfX = arenaSize.x * 0.5f;
        float halfZ = arenaSize.y * 0.5f;
        CreateBlock("North Boundary", root, arenaCenter + new Vector3(0f, 0.65f, halfZ), Vector3.zero, new Vector3(arenaSize.x, 1.4f, 0.45f), stone, false);
        CreateBlock("South Boundary", root, arenaCenter + new Vector3(0f, 0.65f, -halfZ), Vector3.zero, new Vector3(arenaSize.x, 1.4f, 0.45f), stone, false);
        CreateBlock("West Boundary", root, arenaCenter + new Vector3(-halfX, 0.65f, 0f), Vector3.zero, new Vector3(0.45f, 1.4f, arenaSize.y), stone, false);
        CreateBlock("East Boundary", root, arenaCenter + new Vector3(halfX, 0.65f, 0f), Vector3.zero, new Vector3(0.45f, 1.4f, arenaSize.y), stone, false);

        CreateBlock("Sprint Lane Marker", root, arenaCenter + new Vector3(0f, 0.05f, -3f), Vector3.zero, new Vector3(1.8f, 0.12f, 8f), stone, false);
        CreateBlock("Low Platform", root, arenaCenter + new Vector3(-4.5f, 0.35f, 1f), Vector3.zero, new Vector3(3f, 0.75f, 3f), stone, false);
        CreateBlock("High Platform", root, arenaCenter + new Vector3(-6.2f, 0.95f, 5f), Vector3.zero, new Vector3(2.8f, 1.95f, 2.8f), stone, false);
        CreateBlock("Jump Gap Platform", root, arenaCenter + new Vector3(4.5f, 0.55f, 3.5f), Vector3.zero, new Vector3(3f, 1.1f, 3f), stone, false);
        CreateBlock("Ramp", root, arenaCenter + new Vector3(1.5f, 0.25f, 4f), new Vector3(-18f, 0f, 0f), new Vector3(3.5f, 0.45f, 5f), stone, false);

        CreateBlock("Combat Target A", root, arenaCenter + new Vector3(5f, 1f, -2.5f), Vector3.zero, new Vector3(0.9f, 2f, 0.9f), target, true);
        CreateBlock("Combat Target B", root, arenaCenter + new Vector3(7.2f, 1f, 0.5f), Vector3.zero, new Vector3(0.9f, 2f, 0.9f), target, true);
        CreateBlock("Combat Target C", root, arenaCenter + new Vector3(4f, 1f, 2.7f), Vector3.zero, new Vector3(0.9f, 2f, 0.9f), target, true);
    }

    private static Material MakeMaterial(string materialName, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Material material = new Material(shader);
        material.name = materialName;
        material.color = color;
        return material;
    }

    private static void CreateBlock(string blockName, Transform parent, Vector3 position, Vector3 eulerAngles, Vector3 scale, Material material, bool damageable)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = blockName;
        block.transform.SetParent(parent, true);
        block.transform.position = position;
        block.transform.rotation = Quaternion.Euler(eulerAngles);
        block.transform.localScale = scale;

        if (block.TryGetComponent(out MeshRenderer renderer))
            renderer.sharedMaterial = material;

        if (damageable)
            block.AddComponent<Damageable>();
    }
}

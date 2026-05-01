using UnityEngine;

public class PlayerCharacterVisual : MonoBehaviour
{
    [SerializeField] private Color clothColor = new Color(0.22f, 0.25f, 0.22f);
    [SerializeField] private Color armorColor = new Color(0.34f, 0.33f, 0.28f);
    [SerializeField] private Color leatherColor = new Color(0.18f, 0.13f, 0.09f);
    [SerializeField] private Color accentColor = new Color(0.42f, 0.56f, 0.72f);
    [SerializeField] private Color skinColor = new Color(0.72f, 0.52f, 0.38f);

    private const string VisualRootName = "GeneratedCharacterVisual";
    private const string PlayerModelPath = "Models/PrototypePlayer";

    private void Awake()
    {
        MeshRenderer capsuleRenderer = GetComponent<MeshRenderer>();
        if (capsuleRenderer)
            capsuleRenderer.enabled = false;

        if (transform.Find(VisualRootName))
            return;

        if (TryCreateModelVisual())
            return;

        Material cloth = MakeMaterial("Muted Cloth", clothColor);
        Material armor = MakeMaterial("Worn Armor", armorColor);
        Material leather = MakeMaterial("Dark Leather", leatherColor);
        Material accent = MakeMaterial("Readable Accent", accentColor);
        Material skin = MakeMaterial("Natural Skin", skinColor);

        Transform root = new GameObject(VisualRootName).transform;
        root.SetParent(transform, false);

        CreatePart("Hips", PrimitiveType.Cube, root, new Vector3(0f, -0.12f, 0f), Quaternion.identity, new Vector3(0.62f, 0.32f, 0.36f), leather);
        CreatePart("Torso", PrimitiveType.Cube, root, new Vector3(0f, 0.42f, 0f), Quaternion.identity, new Vector3(0.72f, 0.88f, 0.38f), cloth);
        CreatePart("ChestPlate", PrimitiveType.Cube, root, new Vector3(0f, 0.5f, 0.22f), Quaternion.identity, new Vector3(0.5f, 0.56f, 0.05f), armor);
        CreatePart("ChestAccent", PrimitiveType.Cube, root, new Vector3(0f, 0.5f, 0.255f), Quaternion.identity, new Vector3(0.18f, 0.46f, 0.025f), accent);
        CreatePart("Belt", PrimitiveType.Cube, root, new Vector3(0f, -0.08f, 0.01f), Quaternion.identity, new Vector3(0.76f, 0.12f, 0.42f), leather);

        CreatePart("Neck", PrimitiveType.Cylinder, root, new Vector3(0f, 0.92f, 0f), Quaternion.identity, new Vector3(0.22f, 0.12f, 0.22f), skin);
        CreatePart("Head", PrimitiveType.Sphere, root, new Vector3(0f, 1.17f, 0f), Quaternion.identity, new Vector3(0.42f, 0.46f, 0.4f), skin);
        CreatePart("BrowGuard", PrimitiveType.Cube, root, new Vector3(0f, 1.24f, 0.22f), Quaternion.identity, new Vector3(0.38f, 0.08f, 0.04f), leather);

        CreatePart("LeftShoulder", PrimitiveType.Cube, root, new Vector3(-0.48f, 0.77f, 0f), Quaternion.Euler(0f, 0f, -12f), new Vector3(0.32f, 0.18f, 0.44f), armor);
        CreatePart("RightShoulder", PrimitiveType.Cube, root, new Vector3(0.48f, 0.77f, 0f), Quaternion.Euler(0f, 0f, 12f), new Vector3(0.32f, 0.18f, 0.44f), armor);
        CreatePart("LeftArm", PrimitiveType.Capsule, root, new Vector3(-0.58f, 0.32f, 0f), Quaternion.Euler(0f, 0f, 10f), new Vector3(0.2f, 0.58f, 0.2f), cloth);
        CreatePart("RightArm", PrimitiveType.Capsule, root, new Vector3(0.58f, 0.32f, 0f), Quaternion.Euler(0f, 0f, -10f), new Vector3(0.2f, 0.58f, 0.2f), cloth);
        CreatePart("LeftGlove", PrimitiveType.Sphere, root, new Vector3(-0.65f, -0.05f, 0.03f), Quaternion.identity, new Vector3(0.22f, 0.18f, 0.2f), leather);
        CreatePart("RightGlove", PrimitiveType.Sphere, root, new Vector3(0.65f, -0.05f, 0.03f), Quaternion.identity, new Vector3(0.22f, 0.18f, 0.2f), leather);

        CreatePart("LeftLeg", PrimitiveType.Capsule, root, new Vector3(-0.2f, -0.58f, 0f), Quaternion.identity, new Vector3(0.23f, 0.68f, 0.23f), cloth);
        CreatePart("RightLeg", PrimitiveType.Capsule, root, new Vector3(0.2f, -0.58f, 0f), Quaternion.identity, new Vector3(0.23f, 0.68f, 0.23f), cloth);
        CreatePart("LeftBoot", PrimitiveType.Cube, root, new Vector3(-0.2f, -0.98f, 0.07f), Quaternion.identity, new Vector3(0.27f, 0.18f, 0.38f), leather);
        CreatePart("RightBoot", PrimitiveType.Cube, root, new Vector3(0.2f, -0.98f, 0.07f), Quaternion.identity, new Vector3(0.27f, 0.18f, 0.38f), leather);
    }

    private bool TryCreateModelVisual()
    {
        GameObject model = Resources.Load<GameObject>(PlayerModelPath);
        if (!model)
            return false;

        GameObject instance = Instantiate(model, transform);
        instance.name = VisualRootName;
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        return true;
    }

    private static Material MakeMaterial(string materialName, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Material material = new Material(shader);
        material.name = materialName;
        material.color = color;
        return material;
    }

    private static void CreatePart(string partName, PrimitiveType primitive, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = partName;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localRotation = localRotation;
        part.transform.localScale = localScale;

        Collider collider = part.GetComponent<Collider>();
        if (collider)
            Destroy(collider);

        if (part.TryGetComponent(out MeshRenderer renderer))
            renderer.sharedMaterial = material;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerHudBuilder : MonoBehaviour
{
    [SerializeField] private PlayerHud hud;
    [SerializeField] private Vector2 anchorPosition = new Vector2(24f, -24f);

    private const string HudName = "Player HUD";
    private static Font _hudFont;

    private void Awake()
    {
        if (!hud)
            hud = GetComponent<PlayerHud>();

        if (GameObject.Find(HudName))
            return;

        Canvas canvas = CreateCanvas();
        CreatePanel(canvas.transform);
        CreateTitle(canvas.transform);
        Image healthFill = CreateBar(canvas.transform, "VITALS", new Vector2(0f, -26f), new Vector2(260f, 18f), new Color(0.74f, 0.13f, 0.08f));
        Image staminaFill = CreateBar(canvas.transform, "STAMINA", new Vector2(0f, -54f), new Vector2(260f, 14f), new Color(0.38f, 0.58f, 0.76f));

        if (hud)
            hud.Bind(healthFill, staminaFill);

        EnsureEventSystem();
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject(HudName);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private void CreatePanel(Transform parent)
    {
        RectTransform panel = CreateImage(parent, "Vitals Panel", anchorPosition + new Vector2(-10f, 10f), new Vector2(286f, 96f), new Color(0.03f, 0.035f, 0.04f, 0.78f));
        Image image = panel.GetComponent<Image>();
        image.raycastTarget = false;
    }

    private void CreateTitle(Transform parent)
    {
        Text title = CreateText(parent, "HUD Title", "PROJECT STARFALL", anchorPosition, new Vector2(260f, 20f), 12, TextAnchor.MiddleLeft);
        title.color = new Color(0.86f, 0.88f, 0.84f);
    }

    private Image CreateBar(Transform parent, string label, Vector2 offset, Vector2 size, Color fillColor)
    {
        RectTransform background = CreateImage(parent, $"{label} Bar", anchorPosition + offset, size, new Color(0f, 0f, 0f, 0.72f));
        Text text = CreateText(background, $"{label} Label", label, Vector2.zero, size, 11, TextAnchor.MiddleLeft);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.offsetMin = new Vector2(8f, 0f);
        text.rectTransform.offsetMax = new Vector2(-8f, 0f);

        RectTransform fill = CreateImage(background, "Fill", new Vector2(2f, -2f), new Vector2(size.x - 4f, size.y - 4f), fillColor);
        fill.anchorMin = new Vector2(0f, 0.5f);
        fill.anchorMax = new Vector2(0f, 0.5f);
        fill.pivot = new Vector2(0f, 0.5f);

        Image fillImage = fill.GetComponent<Image>();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;
        return fillImage;
    }

    private static RectTransform CreateImage(Transform parent, string objectName, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        return rect;
    }

    private static Text CreateText(Transform parent, string objectName, string value, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject labelObject = new GameObject(objectName);
        labelObject.transform.SetParent(parent, false);

        RectTransform rect = labelObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Text text = labelObject.AddComponent<Text>();
        text.text = value;
        text.font = GetHudFont();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static Font GetHudFont()
    {
        if (_hudFont)
            return _hudFont;

        _hudFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        return _hudFont;
    }

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>())
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }
}

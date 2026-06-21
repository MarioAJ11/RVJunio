using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Crea el Canvas VR con ScriptUI (apunte 10).
/// Tecla H (PC/simulador) o boton B del mando (Quest) muestra/oculta la UI (apunte 11).
/// </summary>
public class VRUiBootstrap : MonoBehaviour
{
    [SerializeField] private Vector3 canvasPosition = new Vector3(0f, 1.5f, 1.2f);
    [SerializeField] private Vector3 canvasScale = new Vector3(0.001f, 0.001f, 0.001f);

    private Canvas _canvas;
    private ScriptUI _scriptUi;

    private void Start()
    {
        BuildUi();
    }

    private void Update()
    {
        if (_canvas == null || !WasUiTogglePressed())
        {
            return;
        }

        var root = _canvas.transform.parent != null ? _canvas.transform.parent.gameObject : _canvas.gameObject;
        root.SetActive(!root.activeSelf);
    }

    private static bool WasUiTogglePressed()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.hKey.wasPressedThisFrame)
        {
            return true;
        }

#if !UNITY_EDITOR
        return OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)
            || OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch);
#else
        return false;
#endif
    }

    private void BuildUi()
    {
        var uiRoot = new GameObject("VR_UI_Root");
        uiRoot.transform.SetPositionAndRotation(canvasPosition, Quaternion.identity);
        uiRoot.transform.localScale = canvasScale;

        _canvas = uiRoot.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;

        var scaler = uiRoot.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;
        uiRoot.AddComponent<GraphicRaycaster>();

        var panel = CreateUiPanel(uiRoot.transform);
        _scriptUi = uiRoot.AddComponent<ScriptUI>();

        WireUi(panel);

        // Oculta al inicio; pulsa H para mostrar (apunte 11 / grabacion limpia).
        uiRoot.SetActive(false);
    }

    private static GameObject CreateUiPanel(Transform parent)
    {
        var panelGo = new GameObject("Panel");
        panelGo.transform.SetParent(parent, false);

        var panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(420f, 320f);

        var image = panelGo.AddComponent<Image>();
        image.color = new Color(0.12f, 0.14f, 0.18f, 0.92f);

        var layout = panelGo.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 16, 16);
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        return panelGo;
    }

    private void WireUi(GameObject panel)
    {
        CreateLabel(panel.transform, "UI interactiva (T2P2)");
        CreateButton(panel.transform, "Generar primitivas", _scriptUi.GenerateObjects);

        var sliderGo = new GameObject("RotationSlider");
        sliderGo.transform.SetParent(panel.transform, false);
        var slider = sliderGo.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.onValueChanged.AddListener(_scriptUi.RotateObj);
        CreateSliderVisual(sliderGo, slider);

        var dropdownGo = new GameObject("ShapeDropdown");
        dropdownGo.transform.SetParent(panel.transform, false);
        var dropdown = dropdownGo.AddComponent<Dropdown>();
        dropdown.options.Add(new Dropdown.OptionData("Cubo"));
        dropdown.options.Add(new Dropdown.OptionData("Esfera"));
        dropdown.options.Add(new Dropdown.OptionData("Cilindro"));
        dropdown.onValueChanged.AddListener(_scriptUi.ChangeObj);
        CreateDropdownVisual(dropdownGo, dropdown);

        var toggleGo = new GameObject("VisibilityToggle");
        toggleGo.transform.SetParent(panel.transform, false);
        var toggle = toggleGo.AddComponent<Toggle>();
        toggle.isOn = true;
        toggle.onValueChanged.AddListener(_scriptUi.ToggleObj);
        CreateToggleVisual(toggleGo, toggle, "Mostrar objeto");

        CreateLabel(panel.transform, "Tecla H (PC) o boton B (Quest): mostrar/ocultar UI");
    }

    private static void CreateLabel(Transform parent, string text)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        var label = go.AddComponent<Text>();
        label.text = text;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 18;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(380f, 28f);
    }

    private static Button CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject(label.Replace(" ", ""));
        go.transform.SetParent(parent, false);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.2f, 0.45f, 0.85f, 1f);

        var button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(380f, 42f);

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var text = textGo.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    private static void CreateSliderVisual(GameObject sliderGo, Slider slider)
    {
        var rect = sliderGo.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(380f, 30f);

        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(sliderGo.transform, false);
        var bgImage = bgGo.AddComponent<Image>();
        bgImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        var bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.25f);
        bgRect.anchorMax = new Vector2(1f, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGo.transform, false);
        var fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRect.offsetMin = new Vector2(10f, 0f);
        fillAreaRect.offsetMax = new Vector2(-10f, 0f);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.65f, 0.35f, 1f);
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        var handleSlideArea = new GameObject("Handle Slide Area");
        handleSlideArea.transform.SetParent(sliderGo.transform, false);
        var handleAreaRect = handleSlideArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10f, 0f);
        handleAreaRect.offsetMax = new Vector2(-10f, 0f);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleSlideArea.transform, false);
        var handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(18f, 18f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;
    }

    private static void CreateDropdownVisual(GameObject dropdownGo, Dropdown dropdown)
    {
        var rect = dropdownGo.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(380f, 36f);

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(dropdownGo.transform, false);
        var label = labelGo.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 16;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        var labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 0f);
        labelRect.offsetMax = new Vector2(-30f, 0f);
        dropdown.captionText = label;

        var templateGo = new GameObject("Template");
        templateGo.transform.SetParent(dropdownGo.transform, false);
        templateGo.SetActive(false);
        var templateRect = templateGo.AddComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.sizeDelta = new Vector2(0f, 120f);

        var templateImage = templateGo.AddComponent<Image>();
        templateImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        var scroll = templateGo.AddComponent<ScrollRect>();

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(templateGo.transform, false);
        var viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = Color.white;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.sizeDelta = new Vector2(0f, 28f);

        var item = new GameObject("Item");
        item.transform.SetParent(content.transform, false);
        var itemRect = item.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0f, 28f);
        var itemToggle = item.AddComponent<Toggle>();

        var itemBg = new GameObject("Item Background");
        itemBg.transform.SetParent(item.transform, false);
        var itemBgImage = itemBg.AddComponent<Image>();
        itemBgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        var itemBgRect = itemBg.GetComponent<RectTransform>();
        itemBgRect.anchorMin = Vector2.zero;
        itemBgRect.anchorMax = Vector2.one;
        itemBgRect.offsetMin = Vector2.zero;
        itemBgRect.offsetMax = Vector2.zero;

        var itemLabelGo = new GameObject("Item Label");
        itemLabelGo.transform.SetParent(item.transform, false);
        var itemLabel = itemLabelGo.AddComponent<Text>();
        itemLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        itemLabel.fontSize = 16;
        itemLabel.color = Color.white;
        var itemLabelRect = itemLabelGo.GetComponent<RectTransform>();
        itemLabelRect.anchorMin = Vector2.zero;
        itemLabelRect.anchorMax = Vector2.one;
        itemLabelRect.offsetMin = new Vector2(10f, 0f);
        itemLabelRect.offsetMax = new Vector2(-10f, 0f);

        itemToggle.targetGraphic = itemBgImage;
        scroll.content = contentRect;
        scroll.viewport = viewportRect;
        dropdown.template = templateRect;
        dropdown.itemText = itemLabel;
    }

    private static void CreateToggleVisual(GameObject toggleGo, Toggle toggle, string labelText)
    {
        var rect = toggleGo.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(380f, 30f);

        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(toggleGo.transform, false);
        var bgImage = bgGo.AddComponent<Image>();
        bgImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        var bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.5f);
        bgRect.anchorMax = new Vector2(0f, 0.5f);
        bgRect.sizeDelta = new Vector2(22f, 22f);
        bgRect.anchoredPosition = new Vector2(16f, 0f);

        var checkGo = new GameObject("Checkmark");
        checkGo.transform.SetParent(bgGo.transform, false);
        var checkImage = checkGo.AddComponent<Image>();
        checkImage.color = new Color(0.2f, 0.75f, 0.35f, 1f);
        var checkRect = checkGo.GetComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = new Vector2(4f, 4f);
        checkRect.offsetMax = new Vector2(-4f, -4f);

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(toggleGo.transform, false);
        var label = labelGo.AddComponent<Text>();
        label.text = labelText;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 16;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        var labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(44f, 0f);
        labelRect.offsetMax = Vector2.zero;

        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
    }
}

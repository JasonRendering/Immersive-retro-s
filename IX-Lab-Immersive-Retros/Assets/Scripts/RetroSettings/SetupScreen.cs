using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Holds the choices so other scenes can read them.
public static class SetupData
{
    public static string Method = "";
    public static string Theme = "";
}

[System.Serializable]
public class MethodOption
{
    public string name;                 // text on the button
    [TextArea] public string description; // shown when hovering
}

[System.Serializable]
public class ThemeOption
{
    public string name;      // e.g. "Ruimte"
    public Sprite image;     // picture shown on the button
    public string sceneName; // scene that opens when this theme is chosen
}

public class SetupScreen : MonoBehaviour
{
    [Header("Containers (the button rows)")]
    [SerializeField] private RectTransform methodContainer;
    [SerializeField] private RectTransform themeContainer;

    [Header("Prefab + Start button")]
    [SerializeField] private SelectButton buttonPrefab;
    [SerializeField] private Button startButton;

    [Header("Tooltip (direct child of the Canvas)")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private float tooltipOffset = 20f;

    [Header("Options (edit in the Inspector)")]
    [SerializeField] private List<MethodOption> methods = new List<MethodOption>();
    [SerializeField] private List<ThemeOption> themes = new List<ThemeOption>();

    [Header("Colours")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.8f, 0.2f);

    private readonly List<SelectButton> methodButtons = new List<SelectButton>();
    private readonly List<SelectButton> themeButtons = new List<SelectButton>();
    private int selectedMethod = -1;
    private int selectedTheme = -1;

    private RectTransform tooltipRect;
    private RectTransform canvasRect;

    private void Start()
    {
        tooltipRect = (RectTransform)tooltipPanel.transform;
        canvasRect = (RectTransform)tooltipPanel.GetComponentInParent<Canvas>().rootCanvas.transform;
        tooltipPanel.SetActive(false);

        // Methode buttons (text + hover description)
        for (int i = 0; i < methods.Count; i++)
        {
            int index = i; // copy so the click remembers its own index
            SelectButton b = Instantiate(buttonPrefab, methodContainer);
            RectTransform rect = (RectTransform)b.transform;

            b.Setup(methods[i].name, null,
                    () => SelectMethod(index),
                    () => ShowTooltip(methods[index].description, rect),
                    HideTooltip);
            methodButtons.Add(b);
        }

        // Thema buttons (image)
        for (int i = 0; i < themes.Count; i++)
        {
            int index = i;
            SelectButton b = Instantiate(buttonPrefab, themeContainer);
            b.Setup("", themes[i].image, () => SelectTheme(index));
            themeButtons.Add(b);
        }

        startButton.onClick.AddListener(OnStartClicked);
        Refresh();
    }

    private void SelectMethod(int index)
    {
        selectedMethod = index;
        Refresh();
    }

    private void SelectTheme(int index)
    {
        selectedTheme = index;
        Refresh();
    }

    // Updates highlights and only enables Start when both are chosen
    private void Refresh()
    {
        for (int i = 0; i < methodButtons.Count; i++)
            methodButtons[i].SetSelected(i == selectedMethod, normalColor, selectedColor);

        for (int i = 0; i < themeButtons.Count; i++)
            themeButtons[i].SetSelected(i == selectedTheme, normalColor, selectedColor);

        startButton.interactable = selectedMethod >= 0 && selectedTheme >= 0;
    }

    // Shows the description box next to the hovered button, always fully on screen
    private void ShowTooltip(string description, RectTransform target)
    {
        if (string.IsNullOrEmpty(description)) return;

        tooltipText.text = description;
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.SetAsLastSibling(); // draw on top of everything

        // Make sure the box has its final size before we measure it
        tooltipText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);

        Vector3[] button = new Vector3[4];
        target.GetWorldCorners(button);
        Vector3 rightMiddle = (button[2] + button[3]) / 2f;
        Vector3 leftMiddle = (button[0] + button[1]) / 2f;

        Vector3[] screen = new Vector3[4];
        canvasRect.GetWorldCorners(screen);

        // 1. Try the right side of the button
        tooltipRect.position = rightMiddle + Vector3.right * tooltipOffset;

        Vector3[] tip = new Vector3[4];
        tooltipRect.GetWorldCorners(tip);

        // 2. Doesn't fit on the right? Flip to the left side
        if (tip[2].x > screen[2].x)
        {
            float width = tip[2].x - tip[0].x;
            tooltipRect.position = leftMiddle + Vector3.left * (tooltipOffset + width);
            tooltipRect.GetWorldCorners(tip);
        }

        // 3. Still sticking out (or too high/low)? Nudge it back inside
        Vector3 shift = Vector3.zero;
        if (tip[0].x < screen[0].x) shift.x = screen[0].x - tip[0].x;
        else if (tip[2].x > screen[2].x) shift.x = screen[2].x - tip[2].x;
        if (tip[0].y < screen[0].y) shift.y = screen[0].y - tip[0].y;
        else if (tip[2].y > screen[2].y) shift.y = screen[2].y - tip[2].y;

        tooltipRect.position += shift;
    }

    private void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    private void OnStartClicked()
    {
        ThemeOption theme = themes[selectedTheme];

        if (string.IsNullOrEmpty(theme.sceneName))
        {
            Debug.LogError($"Theme '{theme.name}' has no scene name filled in!");
            return;
        }

        SetupData.Method = methods[selectedMethod].name;
        SetupData.Theme = theme.name;
        SceneManager.LoadScene(theme.sceneName);
    }
}
using System.Collections.Generic;
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
public class ThemeOption
{
    public string name;   // e.g. "Ruimte"
    public Sprite image;  // picture shown on the button
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

    [Header("Options (edit in the Inspector)")]
    [SerializeField] private List<string> methods = new List<string> { "Methode 1", "Methode 2", "Methode 3" };
    [SerializeField] private List<ThemeOption> themes = new List<ThemeOption>();

    [Header("Colours")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.8f, 0.2f);

    private readonly List<SelectButton> methodButtons = new List<SelectButton>();
    private readonly List<SelectButton> themeButtons = new List<SelectButton>();
    private int selectedMethod = -1;
    private int selectedTheme = -1;

    private void Start()
    {
        // Methode buttons (text only)
        for (int i = 0; i < methods.Count; i++)
        {
            int index = i; // copy so the click remembers its own index
            SelectButton b = Instantiate(buttonPrefab, methodContainer);
            b.Setup(methods[i], null, () => SelectMethod(index));
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

    private void OnStartClicked()
    {
        ThemeOption theme = themes[selectedTheme];

        if (string.IsNullOrEmpty(theme.sceneName))
        {
            Debug.LogError($"Theme '{theme.name}' has no scene name filled in!");
            return;
        }

        SetupData.Method = methods[selectedMethod];
        SetupData.Theme = theme.name;
        SceneManager.LoadScene(theme.sceneName);
    }
}
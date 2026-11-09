using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Put this on the button prefab. Used for both Methode (text) and Thema (image) buttons.
public class SelectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image background; // the frame / border colour
    [SerializeField] private Image icon;       // the picture (themes only)
    [SerializeField] private TMP_Text label;   // the text (methods, optional for themes)

    public void Setup(string text, Sprite sprite, Action onClick)
    {
        // Text
        label.gameObject.SetActive(!string.IsNullOrEmpty(text));
        label.text = text;

        // Image
        icon.gameObject.SetActive(sprite != null);
        icon.sprite = sprite;

        button.onClick.AddListener(() => onClick());
    }

    public void SetSelected(bool selected, Color normalColor, Color selectedColor)
    {
        background.color = selected ? selectedColor : normalColor;
    }
}
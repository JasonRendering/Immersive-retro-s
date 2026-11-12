using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Put this on the button prefab. Used for both Methode (text) and Thema (image) buttons.
public class SelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image background; // the frame / border colour
    [SerializeField] private Image icon;       // the picture (themes only)
    [SerializeField] private TMP_Text label;   // the text (methods, optional for themes)

    private Action onHoverEnter;
    private Action onHoverExit;

    public void Setup(string text, Sprite sprite, Action onClick,
                      Action onHoverEnter = null, Action onHoverExit = null)
    {
        // Text
        label.gameObject.SetActive(!string.IsNullOrEmpty(text));
        label.text = text;

        // Image
        icon.gameObject.SetActive(sprite != null);
        icon.sprite = sprite;

        button.onClick.AddListener(() => onClick());

        this.onHoverEnter = onHoverEnter;
        this.onHoverExit = onHoverExit;
    }

    public void SetSelected(bool selected, Color normalColor, Color selectedColor)
    {
        background.color = selected ? selectedColor : normalColor;
    }

    // Called by Unity's EventSystem when the mouse enters / leaves this button
    public void OnPointerEnter(PointerEventData eventData) => onHoverEnter?.Invoke();
    public void OnPointerExit(PointerEventData eventData) => onHoverExit?.Invoke();
}
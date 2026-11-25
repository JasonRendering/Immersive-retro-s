using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectionScale : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField, Min(1f)] private float selectedScale = 1.1f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = originalScale * selectedScale;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;
    }
}

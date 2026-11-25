using UnityEngine;
using UnityEngine.EventSystems;

public static class NavigationController
{
    public static void SelectButton(GameObject button)
    {
        if (button == null)
            return;

        if (EventSystem.current == null)
        {
            Debug.LogError("Controller UI navigation requires an EventSystem in the scene.");
            return;
        }

        EventSystem.current.SetSelectedGameObject(button);
    }
}

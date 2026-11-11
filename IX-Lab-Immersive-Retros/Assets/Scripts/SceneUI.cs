using UnityEngine;

public class SceneUI : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;

    private void Start()
    {
        if (firstButton == null)
        {
            Debug.LogError("Assign the first UI button in the Inspector.", this);
            return;
        }

        NavigationController.SelectButton(firstButton);
    }
}

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CaveCameraManager : MonoBehaviour
{
    [SerializeField] private CaveData caveData;
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private CaveCamera[] cameras;

    private sbyte firstDisplaySwap = -1;

    private void Start()
    {
#if !UNITY_EDITOR
        for (int i = 0; i < 5; i++)
        {
            Display.displays[i].Activate();
        }
#endif
        if (cameras == null || cameras.Length != 4)
            cameras = GetComponentsInChildren<CaveCamera>();

        SetCaveDataObject(CaveDataLoader.LoadCaveData(caveData));
        ReorderCameraOutputs(caveData.CameraDisplayOutputs);
        ResizeCameraArea();

        uiDocument.rootVisualElement.Q<Button>("RecalculateCamera").clicked += ResizeCameraArea;
        uiDocument.rootVisualElement.Q<Button>("SaveCaveData").clicked += SaveCurrentCaveData;
        //uiDocument.rootVisualElement.Q<Vector3Field>("").RegisterValueChangedCallback((v) => { caveData.CaveCenter = v.newValue; });

        var caveSize = uiDocument.rootVisualElement.Q<Vector3Field>("CaveSize");
        caveSize.Q<FloatField>("unity-x-input").label = "Right to Left wall";
        caveSize.Q<FloatField>("unity-y-input").label = "Ceiling to Ground";
        caveSize.Q<FloatField>("unity-z-input").label = "Front to Back wall";

        uiDocument.rootVisualElement.Q<Button>("DisplayForward").clicked  += () => SwapDisplay(0);
        uiDocument.rootVisualElement.Q<Button>("DisplayRight").clicked    += () => SwapDisplay(1);
        uiDocument.rootVisualElement.Q<Button>("DisplayBackward").clicked += () => SwapDisplay(2);
        uiDocument.rootVisualElement.Q<Button>("DisplayLeft").clicked     += () => SwapDisplay(3);

    }

    private void OnValidate()
    {
        if(cameras == null || cameras.Length != 4)
            cameras = GetComponentsInChildren<CaveCamera>();
    }

    public void SetUIDocument(UIDocument document)
    {
        uiDocument = document;
    }

    public void SetCaveDataObject(CaveData caveData)
    {
        this.caveData = caveData;
    }

    public void SaveCurrentCaveData()
    {
        Debug.Log("Saving");
        CaveDataLoader.SaveCaveData(caveData);
    }

    public void SwapDisplay(sbyte displayButton)
    {
        if (firstDisplaySwap == -1)
        {
            firstDisplaySwap = displayButton;
            return;
        }

        cameras[firstDisplaySwap].SwapDisplay(cameras[displayButton]);
        firstDisplaySwap = -1;
    }

    public void ReorderCameraOutputs(int[] displays)
    {
        if (displays.Length != 4)
            throw new ArgumentException($"There must be 4 displays given for a camera re-order. Got: {displays.Length}", nameof(displays));

        for (int i = 0; i < 4; i++)
        {
            cameras[i].SetDisplayOutput(displays[i]);
        }
    }

    /// <summary>
    /// Resizes the full camera area and recalculate their viewing planes using the current connected Cavedata object.
    /// </summary>
    /// <remarks>Will always make a square box of which the distance from the center is <see cref="caveData"/>.CaveSize.z</remarks>
    public void ResizeCameraArea()
    {
        Debug.Log("ResizeCamera");
        ResizeCameraArea(caveData.CaveCenter, Quaternion.identity, caveData.CaveSize, caveData.EyeHeight);
    }

    /// <summary>
    /// Resizes the full camera area and recalculate their viewing planes.
    /// </summary>
    /// <remarks>Will always make a square box of which the distance from the center is <paramref name="Size"/>.z</remarks>
    /// <param name="Center">The Gameobject to center around, using its location and rotation.</param>
    /// <param name="Size">The size of the Cave in world space from the center to the nearest wall</param>
    public void ResizeCameraArea(GameObject Center, Vector3 Size, float EyeHeight)
    {
        ResizeCameraArea(Center.transform.position, Center.transform.rotation, Size, EyeHeight);
    }

    /// <summary>
    /// Resizes the full camera area and recalculate their viewing planes.
    /// </summary>
    /// <remarks>Will always make a square box of which the distance from the center is <paramref name="Size"/>.z</remarks>
    /// <param name="Center">The Middle of the Cave in world space.</param>
    /// <param name="Rotation">The Rotation of the Cave in degrees. (DOES NOTHING RIGHT NOW)</param>
    /// <param name="Size">The size of the Cave in world space from the center to the nearest wall</param>
    public void ResizeCameraArea(Vector3 Center, Quaternion Rotation, Vector3 Size, float EyeHeight)
    {
        Size.x /= 2;
        Size.z /= 2;

        Vector3 CameraPos = new Vector3(0, EyeHeight, 0);
        transform.SetLocalPositionAndRotation((Center + CameraPos), Rotation);

        Vector3 forward = new Vector3(Size.x, Size.y, Size.z); // Backward is the exact same proportions since it's changes are just an extra rotation
        Vector3 right = new Vector3(Size.z, Size.y, Size.x); // same as the comment above for Left.

        for (int i = 0; i < cameras.Length; i++)
        {
            Vector3 plane = (i % 2) == 0 ? forward : right;

            Vector3 CenterCameraPlane = new Vector3(0, 0, plane.z);

            Vector3 LeftBottom = CenterCameraPlane - new Vector3(plane.x, 0, 0);
            Vector3 RightBottom = CenterCameraPlane + new Vector3(plane.x, 0, 0);
            Vector3 LeftTop = LeftBottom + new Vector3(0, plane.y, 0);
            float nearClipPlane = Mathf.Max(0.0001f, (CenterCameraPlane - CameraPos).magnitude);

            Debug.Log("Center: " + Center);
            Debug.Log("CenterCameraPlane: " + CenterCameraPlane);
            Debug.Log("LeftBottom: " + LeftBottom);
            Debug.Log("RightBottom: " + RightBottom);
            Debug.Log("LeftTop: " + LeftTop);
            Debug.Log("NearClipPlane: " + nearClipPlane);

            cameras[i].SetCorners(LeftBottom, RightBottom, LeftTop, nearClipPlane, CameraPos);
        }
    }
}

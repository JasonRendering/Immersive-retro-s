using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CaveCameraManager))]
public class CaveCameraEditor : Editor
{
    private CaveCameraManager manager;
    private Vector3 Rotation = Vector3.zero;
    private Vector3 Center = Vector3.zero;
    private Vector3 Size = Vector3.one;
    private float EyeHeight = 1.8f;

    [MenuItem("GameObject/Cave/Insert Camera Setup")]
    static void CreateCaveCameras(MenuCommand command)
    {
        GameObject root = new GameObject("Cave Cameras");
        GameObjectUtility.SetParentAndAlign(root, command.context as GameObject);
        CaveCameraManager manager = root.AddComponent<CaveCameraManager>();

        for (int i = 0; i < 4; i++)
        {
            GameObject cameraObject = new GameObject("Cave Camera " + (i+1));
            GameObjectUtility.SetParentAndAlign(cameraObject, root);
            cameraObject.transform.Rotate(Vector3.up, 90 * i);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.targetDisplay = i + 1;

            cameraObject.AddComponent<CaveCamera>();
        }

        SetCaveDataObject(manager);

        Undo.RegisterCreatedObjectUndo(root, "Created " + root.name);
        Selection.activeObject = root;

        UpdateMainCamera(manager);
    }

    private static void SetCaveDataObject(CaveCameraManager manager)
    {
        CaveData caveData = AssetDatabase.LoadAssetAtPath<CaveData>(
                "Packages/com.hu.cavecameras/Runtime/ScriptableObjects/CaveData.asset"
            );

        manager.SetCaveDataObject(caveData);
    }

    private static void UpdateMainCamera(CaveCameraManager manager)
    {
#if UNITY_6000_3_OR_NEWER
        bool userResponse = EditorDialog.DisplayDecisionDialog("Cave cameras - Main camera", "The main camera should also be changed to better suit the cave setup, Do you wish to continue?", null, null);
#else
        bool userResponse = EditorUtility.DisplayDialog("Cave cameras - Main camera", "The main camera should also be changed to better suit the cave setup, Do you wish to continue?", "Yes", "No");
#endif
        if (!userResponse)
            return;

        Camera main = Camera.main;
        Undo.RecordObject(main, "Updating " + main.name);
        main.clearFlags = CameraClearFlags.SolidColor;
        main.backgroundColor = Color.black;
        main.cullingMask = 1 << 5;
        main.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        SetupManualCalibrationUI(main.gameObject, manager);
    }

    private static void SetupManualCalibrationUI(GameObject parent, CaveCameraManager manager)
    {
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(
                "Packages/com.hu.cavecameras/Runtime/UI/CavePanelSettings.asset"
            );

        VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(   
                "Packages/com.hu.cavecameras/Runtime/UI/ManualCalibrationUI.uxml"
            );

        GameObject UIGO = new GameObject("Manual Calibration UI");
        GameObjectUtility.SetParentAndAlign(UIGO, parent);
        UIDocument UIDoc = UIGO.AddComponent<UIDocument>();
        UIDoc.panelSettings = panelSettings;
        UIDoc.visualTreeAsset = visualTreeAsset;

        manager.SetUIDocument(UIDoc);
        Undo.RegisterCreatedObjectUndo(UIGO, "Created " + UIGO.name);
    }

    private void OnEnable()
    {
        if (target)
            manager = target as CaveCameraManager;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        Center = EditorGUILayout.Vector3Field("Center", Center);
        Rotation = EditorGUILayout.Vector3Field("Rotation in degrees", Rotation);
        Size = EditorGUILayout.Vector3Field("Size", Size);
        EyeHeight = EditorGUILayout.FloatField("Eye Height", EyeHeight);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Resize"))
            manager.ResizeCameraArea(Center, Quaternion.Euler(Rotation), Size, EyeHeight);
        if (GUILayout.Button("Resize as Root"))
            manager.ResizeCameraArea(manager.gameObject, Size, EyeHeight);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}

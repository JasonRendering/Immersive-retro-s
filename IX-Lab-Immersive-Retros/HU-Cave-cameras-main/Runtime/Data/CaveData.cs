using Unity.Properties;
using UnityEngine;

[CreateAssetMenu]
public class CaveData : ScriptableObject
{
    [Header("Cave Setup")]
    [SerializeField]
    [CreateProperty]
    public Vector3 CaveCenter = Vector3.zero;

    [SerializeField]
    [CreateProperty]
    public Vector3 CaveSize = Vector3.one;

    [SerializeField]
    [CreateProperty]
    public float EyeHeight = 1.8f;

    [Header("Camera Outputs")]
    [SerializeField]
    [CreateProperty]
    public int[] CameraDisplayOutputs = new int[4] { 1, 2, 3, 4 };
}
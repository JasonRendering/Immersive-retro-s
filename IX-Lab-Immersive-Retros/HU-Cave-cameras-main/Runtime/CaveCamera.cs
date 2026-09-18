using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CaveCamera : MonoBehaviour
{
    Vector3 LeftBottom, RightBottom, LeftTop;
    Vector3 CenterCameraArea;
    new Camera camera;

    private void Awake()
    {
        if (!camera)
            camera = GetComponent<Camera>();
    }

    private void OnValidate()
    {
        if(!camera)
            camera = GetComponent<Camera>();
    }

    internal void SetCorners(Vector3 pa, Vector3 pb, Vector3 pc, float near, Vector3 CenterCameraArea)
    {
        this.CenterCameraArea = CenterCameraArea;
        LeftBottom = pa;
        RightBottom = pb;
        LeftTop = pc;
        camera.nearClipPlane = near;
    }

    private void FixedUpdate()
    {
        camera.projectionMatrix = GeneralizedPerspectiveProjection(LeftBottom, RightBottom, LeftTop, CenterCameraArea, camera.nearClipPlane, camera.farClipPlane);
    }

    private static Matrix4x4 GeneralizedPerspectiveProjection(Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pe, float near, float far)
    {
        Vector3 va, vb, vc;
        Vector3 vr, vu, vn;

        float left, right, bottom, top, eyedistance;

        Matrix4x4 transformMatrix;
        Matrix4x4 projectionM;
        Matrix4x4 eyeTranslateM;
        Matrix4x4 finalProjection;

        ///Calculate the orthonormal for the screen (the screen coordinate system
        vr = pb - pa;
        vr.Normalize();
        vu = pc - pa;
        vu.Normalize();
        vn = -Vector3.Cross(vr, vu);
        vn.Normalize();

        //Calculate the vector from eye (pe) to screen corners (pa, pb, pc)
        va = pa - pe;
        vb = pb - pe;
        vc = pc - pe;

        //Get the distance;; from the eye to the screen plane
        eyedistance = -Vector3.Dot(va, vn);

        //Get the varaibles for the off center projection
        left = (Vector3.Dot(vr, va) * near) / eyedistance;
        right = (Vector3.Dot(vr, vb) * near) / eyedistance;
        bottom = (Vector3.Dot(vu, va) * near) / eyedistance;
        top = (Vector3.Dot(vu, vc) * near) / eyedistance;

        //Get this projection
        projectionM = PerspectiveOffCenter(left, right, bottom, top, near, far);

        //Fill in the transform matrix
        transformMatrix = new Matrix4x4();
        transformMatrix[0, 0] = vr.x;
        transformMatrix[0, 1] = vr.y;
        transformMatrix[0, 2] = vr.z;
        transformMatrix[0, 3] = 0;
        transformMatrix[1, 0] = vu.x;
        transformMatrix[1, 1] = vu.y;
        transformMatrix[1, 2] = vu.z;
        transformMatrix[1, 3] = 0;
        transformMatrix[2, 0] = vn.x;
        transformMatrix[2, 1] = vn.y;
        transformMatrix[2, 2] = vn.z;
        transformMatrix[2, 3] = 0;
        transformMatrix[3, 0] = 0;
        transformMatrix[3, 1] = 0;
        transformMatrix[3, 2] = 0;
        transformMatrix[3, 3] = 1;

        //Now for the eye transform
        eyeTranslateM = new Matrix4x4();
        eyeTranslateM[0, 0] = 1;
        eyeTranslateM[0, 1] = 0;
        eyeTranslateM[0, 2] = 0;
        eyeTranslateM[0, 3] = -pe.x;
        eyeTranslateM[1, 0] = 0;
        eyeTranslateM[1, 1] = 1;
        eyeTranslateM[1, 2] = 0;
        eyeTranslateM[1, 3] = -pe.y;
        eyeTranslateM[2, 0] = 0;
        eyeTranslateM[2, 1] = 0;
        eyeTranslateM[2, 2] = SystemInfo.usesReversedZBuffer ? -1 : 1;
        eyeTranslateM[2, 3] = -pe.z;
        eyeTranslateM[3, 0] = 0;
        eyeTranslateM[3, 1] = 0;
        eyeTranslateM[3, 2] = 0;
        eyeTranslateM[3, 3] = 1f;

        //Multiply all together
        finalProjection = Matrix4x4.identity * projectionM * transformMatrix * eyeTranslateM;

        finalProjection[1, 2] *= SystemInfo.usesReversedZBuffer ? 1 : -1;

        //finally return
        return finalProjection;
    }

    private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }

    public void SetDisplayOutput(int output)
    {
        camera.targetDisplay = output;
    }

    public void SwapDisplay(CaveCamera other)
    {
        (other.camera.targetDisplay, camera.targetDisplay) = (camera.targetDisplay, other.camera.targetDisplay);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(CenterCameraArea, .25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(LeftBottom, .25f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(LeftTop, .25f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(RightBottom, .25f);
    }
}

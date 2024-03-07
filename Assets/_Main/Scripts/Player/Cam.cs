using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Cam : MonoBehaviour
{
    // references
    Camera cameraRef;
    Camera isometricCamera;
    [SerializeField] RenderTexture result;

    Matrix4x4 ogProjection;
    public float left = -0.2F;
    public float right = 0.2F;
    public float top = 0.2F;
    public float bottom = -0.2F;

    [SerializeField] Material material;

    public Vector3 offset = new Vector3(0, 1, 0);

    public void Init()
    {
        // get references
        cameraRef = GetComponent<Camera>();
        isometricCamera = transform.Find("Isometric Camera").GetComponent<Camera>();

        ogProjection = Matrix4x4.identity;
    }

    void LateUpdate()
    {
        material.SetVector("_CamPosition", cameraRef.transform.position);
        material.SetVector("_CamRotation", cameraRef.transform.rotation.eulerAngles);
        material.SetVector("_CamScale", cameraRef.transform.localScale);
        //Camera cam = Camera.main;
        //Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
        //cam.projectionMatrix = m;

        //Vector3 camoffset = new Vector3(-offset.x, -offset.y, offset.z);
        //Matrix4x4 m = Matrix4x4.TRS(camoffset, Quaternion.identity, new Vector3(1, 1, -1));
        //cameraRef.worldToCameraMatrix = m * transform.worldToLocalMatrix;
        material.SetMatrix("_CameraMatrix", transform.worldToLocalMatrix.inverse);


    }

    private void OnPostRender()
    {
        //Graphics.Blit(isometricCamera.targetTexture, cameraRef.targetTexture);
    }

    // from docs
    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
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
}

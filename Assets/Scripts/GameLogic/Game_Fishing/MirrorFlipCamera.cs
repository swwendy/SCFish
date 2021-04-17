using UnityEngine;
[RequireComponent(typeof(Camera))]
public class MirrorFlipCamera : MonoBehaviour
{
    new Camera camera;
    public Vector3 scale = new Vector3(1,1,1);

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void OnPreCull()
    {
        camera.ResetWorldToCameraMatrix();
        camera.ResetProjectionMatrix();
        camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(scale);
    }

    void OnPreRender()
    {
        GL.invertCulling = (scale != new Vector3(1,1,1));
    }

    void OnPostRender()
    {
        GL.invertCulling = false;
    }
}
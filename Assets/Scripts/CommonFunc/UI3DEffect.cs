using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UI3DEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject effectPrefab;
    [SerializeField]
    private bool orthographic = true;
    [SerializeField]
    private float orthographicSize = 1f;
    [SerializeField]
    private float far = 50f;
    [SerializeField]
    private Vector3 cameraPos = Vector3.zero;
    [SerializeField]
    private Vector3 Pos = Vector3.forward;
    [SerializeField]
    private Vector3 Rot = Vector3.zero;

    public LayerMask layer = 1 << 14;

    private GameObject effectGO;
    private RenderTexture renderTexture;
    private Camera rtCamera;
    private RawImage rawImage;

    void Awake()
    {
        //RawImage可以手动添加，设置no alpha材质，以显示带透明的粒子
        rawImage = gameObject.GetComponent<RawImage>();
        if (rawImage == null)
        {
            rawImage = gameObject.AddComponent<RawImage>();
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            return transform as RectTransform;
        }
    }

    void OnEnable()
    {
        if (effectPrefab != null)
        {
            effectGO = Instantiate(effectPrefab);

            GameObject cameraObj = new GameObject("UIEffectCamera");
            rtCamera = cameraObj.AddComponent<Camera>();
            renderTexture = new RenderTexture((int)rectTransform.sizeDelta.x, (int)rectTransform.sizeDelta.y, 24);
            renderTexture.antiAliasing = 4;
            rtCamera.clearFlags = CameraClearFlags.SolidColor;
            rtCamera.backgroundColor = new Color();
            rtCamera.cullingMask = layer;
            rtCamera.targetTexture = renderTexture;
            rtCamera.orthographic = orthographic;
            rtCamera.orthographicSize = orthographicSize;
            rtCamera.farClipPlane = far;

            effectGO.transform.SetParent(cameraObj.transform, false);
            cameraObj.transform.position = cameraPos;
            effectGO.transform.localPosition = Pos;
            effectGO.transform.localRotation = Quaternion.Euler(Rot);

            rawImage.enabled = true;
            rawImage.texture = renderTexture;
        }
        else
        {
            rawImage.texture = null;
            Debug.LogError("EffectPrefab can't be null");
        }
    }

    void OnDisable()
    {
        if (effectGO != null)
        {
            Destroy(effectGO);
            effectGO = null;
        }
        if (rtCamera != null)
        {
            Destroy(rtCamera.gameObject);
            rtCamera = null;
        }
        if (renderTexture != null)
        {
            Destroy(renderTexture);
            renderTexture = null;
        }
        rawImage.enabled = false;
    }
}
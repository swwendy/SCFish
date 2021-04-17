using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChildSceneObj : MonoBehaviour
{
    public string ObjPath;
    GameObject child;

    public string ModelParentPath = "3d_point/Model";
    Transform model;
    bool m_bDrag = false;

    public float RotationSpeed = 30f;
    /// <summary>
    /// 如果初始模型Y轴有旋转，需要把模型旋转值赋值到这里
    /// </summary>
    private float mYRotation = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_bDrag && model != null)
        {
            mYRotation -= Input.GetAxis("Mouse X") * RotationSpeed;

            Quaternion rotation = Quaternion.Euler(model.eulerAngles.x, mYRotation, 0);
            // 插值旋转
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, rotation, Time.deltaTime * RotationSpeed);
        }

    }

    private void OnEnable()
    {
        GameMain.SC(EnumEnable(true));
    }

    private void OnDisable()
    {
        GameMain.SC(EnumEnable(false));
    }

    IEnumerator EnumEnable(bool enable)
    {
        yield return null;

        if(enable)
        {
            child = GameObject.Find(ObjPath);
            if (child == null)
            {
                yield return null;
                child = GameObject.Find(ObjPath);
            }

            if(child != null)
            {
                child.transform.SetParent(transform, false);
                XPointEvent.AutoAddListener(gameObject, OnClickChild, null);
                Transform tfm = GameObject.Find(ModelParentPath).transform;
                if (tfm.childCount > 0)
                {
                    model = tfm.GetChild(0);
                    model.eulerAngles = new Vector3(0f, 180f, 0f);
                    mYRotation = model.eulerAngles.y;
                }
            }
        }
        else
        {
            if (child != null)
            {
                child.transform.SetParent(GameObject.Find("Canvas/Root").transform, false);
                child.transform.SetAsFirstSibling();
            }
            child = null;
            model = null;
        }
    }

    void OnClickChild(EventTriggerType eventtype, object message, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerDown)        {            m_bDrag = true;        }        if (eventtype == EventTriggerType.PointerUp)        {            m_bDrag = false;        }    }
}

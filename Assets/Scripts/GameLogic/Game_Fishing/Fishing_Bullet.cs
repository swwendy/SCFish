using UnityEngine;
using System.Collections;
using XLua;
using USocket.Messages;

[Hotfix]
public class Fishing_Bullet : MonoBehaviour {

    //移动速度
    float m_fMoveSpeed = 5f;
    byte m_nBounceTimes = 0;
    string m_szNet;
    GameObject m_Target;
    uint m_Id;

    public Fishing_Cannon OwnerCannon { get; set; }

    public static Transform ParentTfm { get; set; }
    //创建子弹
    public static Fishing_Bullet Create(AssetBundle ab, Vector3 pos, Quaternion q, FishingCannonData fcd, GameObject target, uint bulletId)
    {
        GameObject prefab = (GameObject)ab.LoadAsset(fcd.m_szBullet);
        GameObject fireSprite = (GameObject)Instantiate(prefab, pos, q);
        Fishing_Bullet f = fireSprite.AddComponent<Fishing_Bullet>();
        fireSprite.transform.SetParent(ParentTfm, false);
        f.m_fMoveSpeed = fcd.m_fBulletSpeed;
        f.m_nBounceTimes = fcd.m_nBounceTimes;
        f.m_szNet = fcd.m_szNet;
        f.m_Target = target;
        f.m_Id = bulletId;
        if (target != null)
        {
            f.m_nBounceTimes = 1;
        }
        Destroy(fireSprite, fcd.m_fBulletLifeTime);

        return f;
    }

	void Start ()
    {
        if (OwnerCannon != null)
            OwnerCannon.BelongRole.GameBase.m_AddItems.Add(gameObject);
    }

    private void OnDestroy()
    {
        if (OwnerCannon != null)
            OwnerCannon.BelongRole.GameBase.m_AddItems.Remove(gameObject);
    }

    void Update ()
    {
        Canvas cv = OwnerCannon.BelongRole.GameBase.GameCanvas;
        int layerMask = CGame_Fishing.FishLayer;
        int oldLayer = 0;

        if (m_Target != null)
        {
            Vector3 targetPos = RectTransformUtility.WorldToScreenPoint(Camera.main, m_Target.transform.position);
            Vector3 pos = RectTransformUtility.WorldToScreenPoint(cv.worldCamera, transform.position);
            Vector3 dir = targetPos - pos;
            float dis = dir.magnitude;

            if(dis < 50f)
            {
                OnHit(m_Target, m_Target.transform.position, cv);
                return;
            }

            float angle = Vector2.Angle(dir, Vector3.up);
            if (dir.x > 0f)
                angle = -angle;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            float speed = 50f;
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, speed / dis);

            oldLayer = m_Target.layer;
            m_Target.layer = 22;
            layerMask = 1 << m_Target.layer;

        }

        transform.Translate(new Vector3(0f, Time.deltaTime * m_fMoveSpeed, 0f));

        Vector3 uiPos = RectTransformUtility.WorldToScreenPoint(cv.worldCamera, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(uiPos);
        //Debug.DrawLine(ray.origin, ray.GetPoint(500f), Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 500f, layerMask))
        {
            GameObject gameObj = hitInfo.collider.gameObject;

            //DebugLog.Log("click object name is " + gameObj.name + " layer:" + gameObj.layer);

            OnHit(gameObj, hitInfo.point, cv);
        }
        else
            CheckBounce(uiPos);

        if (m_Target != null)
            m_Target.layer = oldLayer;
    }

    void CheckBounce(Vector3 uiPos)
    {
        byte oldTimes = m_nBounceTimes;

        Vector3 vVelocity = transform.TransformDirection(new Vector3(0f, 1f, 0f));

        if ((uiPos.x <= 0) && vVelocity.x < 0f)
        {
            vVelocity = new Vector3(-vVelocity.x, vVelocity.y, 0);
            m_nBounceTimes--;
        }

        if ((uiPos.x >= Screen.width) && vVelocity.x > 0)
        {
            vVelocity = new Vector3(-vVelocity.x, vVelocity.y, 0);
            m_nBounceTimes--;
        }

        if ((uiPos.y <= 0) && vVelocity.y < 0)
        {
            vVelocity = new Vector3(vVelocity.x, -vVelocity.y, 0);
            m_nBounceTimes--;
        }

        if ((uiPos.y >= Screen.height) && vVelocity.y > 0)
        {
            vVelocity = new Vector3(vVelocity.x, -vVelocity.y, 0);
            m_nBounceTimes--;
        }

        if (m_nBounceTimes == 0)
        {
            Destroy(gameObject);
        }
        else if (oldTimes != m_nBounceTimes)
        {
            float angle = Vector2.Angle(vVelocity, Vector3.up);

            if (vVelocity.x > 0f)
                angle = -angle;

            this.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    void OnHit(GameObject obj, Vector3 hitpoint, Canvas cv)
    {
        Fishing_Fish fish = obj.GetComponent<Fishing_Fish>();
        if (fish == null)
            return;

        Destroy(gameObject);

        GameObject prefab = (GameObject)OwnerCannon.BelongRole.GameBase.FishingAssetBundle.LoadAsset(m_szNet);

        Vector3 pos = GameFunction.WorldToLocalPointInRectangle(hitpoint, Camera.main, cv, cv.worldCamera);
        Transform root = cv.transform.Find("Root");

        GameObject explosion = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        explosion.transform.SetParent(root, false);
        OwnerCannon.BelongRole.GameBase.m_AddItems.Add(explosion);

        prefab = (GameObject)OwnerCannon.BelongRole.GameBase.FishingAssetBundle.LoadAsset("FishHit");
        GameObject bubble = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        bubble.transform.SetParent(root, false);
        OwnerCannon.BelongRole.GameBase.m_AddItems.Add(bubble);

        GameMain.WaitForCall(1f, () =>
        {
            OwnerCannon.BelongRole.GameBase.m_AddItems.Remove(explosion);
            Destroy(explosion);

            OwnerCannon.BelongRole.GameBase.m_AddItems.Remove(bubble);
            Destroy(bubble);
        });

        bool bLocal = m_Id != 0;
        fish.OnHit(bLocal);

        if (bLocal)//local player send
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_FIRERESULT);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(fish.m_nOnlyId);
            msg.Add(m_Id);
            HallMain.SendMsgToRoomSer(msg);
        }
    }
}

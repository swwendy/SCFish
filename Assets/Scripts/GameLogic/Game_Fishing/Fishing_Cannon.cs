using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using XLua;
using UnityEngine.UI;
using USocket.Messages;

[Hotfix]
public class Fishing_Cannon : MonoBehaviour {
    
    //声音源
    protected AudioSource m_FireSound;

    public Fishing_Role BelongRole { get; set; }
    public bool AutoFire { get; set; }

    Image m_LineImg;

    //射击计时器
    float m_shootTimer = 0;
    bool m_bFire = false;
    FishingCannonData m_Data;

    public byte m_nTypeId;
    DragonBones.UnityArmatureComponent m_AnimCtrl;

    uint m_nBulletId = 0;
    uint m_nBulletCost = 0;

    public static Fishing_Cannon Create(AssetBundle ab, Transform parent, byte id, byte level, uint bulletCost)
    {
        FishingCannonData fcd = Fishing_Data.GetInstance().m_CannonData[id];
        GameObject obj = (GameObject)ab.LoadAsset(fcd.m_szCannon);
        obj = ((GameObject)GameMain.instantiate(obj));
        obj.transform.SetParent(parent, false);
        obj.transform.Find(fcd.m_szLvPoint).GetComponent<UnityEngine.UI.Text>().text = level.ToString();
        Fishing_Cannon fc = obj.AddComponent<Fishing_Cannon>();
        fc.m_Data = fcd;
        fc.m_nTypeId = id;
        fc.m_nBulletCost = bulletCost * (uint)fcd.m_szBulletPoint.Length;

        if(fcd.m_nAudio != 0)
        {
            AudioSource audioSrc = obj.AddComponent<AudioSource>();
            audioSrc.clip = ab.LoadAsset<AudioClip>(CustomAudioDataManager.GetInstance().GetSoundName(fcd.m_nAudio));
        }
        return fc;
    }

    // Use this for initialization
    void Start ()
    {
     //获取声音
        m_FireSound = GetComponent<AudioSource>();
        m_FireSound.Stop();

        m_AnimCtrl = transform.Find("Anime_01").GetComponent<DragonBones.UnityArmatureComponent>();
        m_AnimCtrl.AddEventListener(DragonBones.EventObject.COMPLETE, AnimationEventHandler);

        m_LineImg = transform.Find(m_Data.m_szBulletPoint[0]).GetComponentInChildren<Image>();

        PlayAnim("up");

        CustomAudioDataManager.GetInstance().PlayAudio(1005);
    }

    void AnimationEventHandler(string _type, DragonBones.EventObject eventObject)
    {
        if(_type == DragonBones.EventObject.COMPLETE)
        {
            PlayAnim("idle");
        }
    }

    // Update is called once per frame
    void Update () {
        if (BelongRole.IsLocal())
        {
            m_shootTimer -= Time.deltaTime;

            if (!BelongRole.IsLockFish())
                UpdateInput();
            else
                UpdateLocked();
        }
        else
            UpdateLockLine();
    }

    void UpdateFire(Vector3 ms, GameObject target = null)
    {
        if (!m_bFire)
            return;

        if(ms != Vector3.zero)
        {
            Vector3 mypos = this.transform.position;
            mypos = RectTransformUtility.WorldToScreenPoint(BelongRole.GameBase.GameCanvas.worldCamera, mypos);
            //计算鼠标位置与大炮位置之间的角度
            Vector2 targetDir = ms - mypos;
            float angle = Vector2.Angle(targetDir, Vector3.up);
            if (ms.x > mypos.x)
                angle = -angle;

            this.transform.eulerAngles = new Vector3(0, 0, angle);
        }

        if (m_shootTimer <= 0)
        {
            m_shootTimer = m_Data.m_fBulletCD;

            LocalFire(target);
        }
    }

    void UpdateInput()
    {
        if (Input.GetMouseButtonUp(0))
            m_bFire = false;

        if(AutoFire)
        {
            Vector3 pos = Vector3.zero;
            if (Input.GetMouseButtonDown(0) && !GameFunction.IsPointerOnUI())
                pos = Input.mousePosition;

            m_bFire = true;

            UpdateFire(pos);
        }
        else if (Input.GetMouseButton(0))
        {
            if(!m_bFire)//非连击的情况下是否在UI上
            {
                if (GameFunction.IsPointerOnUI())
                    return;
            }

            m_bFire = true;

            UpdateFire(Input.mousePosition);
        }

        m_LineImg.fillAmount = 0f;
    }

    void UpdateLocked()
    {
        if (BelongRole.LockFish != null && !BelongRole.LockFish.Caught())
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayhit;
                if (Physics.Raycast(ray, out rayhit, 500f, CGame_Fishing.FishLayer))
                {
                    BelongRole.ChangeLockFish(rayhit.collider.gameObject.GetComponent<Fishing_Fish>());
                }
            }

            m_bFire = true;
            Canvas cv = BelongRole.GameBase.GameCanvas;
            Vector3 ms = RectTransformUtility.WorldToScreenPoint(cv.worldCamera, BelongRole.LockUI.transform.position);
            UpdateFire(ms, BelongRole.LockFish.gameObject);
            UpdateLockLine();
        }
        else
        {
            m_bFire = false;
            m_LineImg.fillAmount = 0f;
        }
    }

    void UpdateLockLine()
    {
        if (!BelongRole.IsLockFish() || BelongRole.LockFish == null || BelongRole.LockFish.Caught())
        {
            m_LineImg.fillAmount = 0f;
            return;
        }

        Canvas cv = BelongRole.GameBase.GameCanvas;
        Vector3 ms = RectTransformUtility.WorldToScreenPoint(cv.worldCamera, BelongRole.LockUI.transform.position);
        Vector3 linePos = RectTransformUtility.WorldToScreenPoint(cv.worldCamera, m_LineImg.transform.position);

        Vector2 targetDir = ms - linePos;
        float len = targetDir.magnitude;
        float uiLen = m_LineImg.rectTransform.rect.width;
        m_LineImg.fillAmount = len / uiLen;
    }

    void PlayAnim(string animName)
    {
        if(m_AnimCtrl != null)
            m_AnimCtrl.animation.Play(animName);
    }

    public void LocalFireFail()
    {
        AutoFire = false;
        CRollTextUI.Instance.AddVerticalRollText(2503);
    }

    void LocalFire(GameObject target)
    {
        if (BelongRole == null || BelongRole.m_nTotalCoin < m_nBulletCost)
        {
            LocalFireFail();
            return;
        }

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_APPLYFIRE);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(transform.eulerAngles.z);
        msg.Add((byte)m_Data.m_szBulletPoint.Length);
        msg.Add(++m_nBulletId);
        HallMain.SendMsgToRoomSer(msg);

        OnFire(target, m_nBulletId);

        if(m_FireSound != null)
        {
            m_FireSound.volume = AudioManager.Instance.SoundVolume;
            m_FireSound.loop = false;
            m_FireSound.Play();
        }

        BelongRole.UpdateInfoUI(BelongRole.m_nTotalCoin - m_nBulletCost);
    }

    public void OtherFire(float angle, byte num, GameObject lockObj)
    {
        if (BelongRole.IsLocal())
            return;

        transform.eulerAngles = new Vector3(0f, 0f, angle);
        OnFire(lockObj, 0);
    }

    void OnFire(GameObject target, uint bulletId)
    {
        PlayAnim("fashe");

        foreach (string point in m_Data.m_szBulletPoint)
        {
            Transform tfm = transform.Find(point);
            Canvas cv = BelongRole.GameBase.GameCanvas;
            Vector3 uiPos = GameFunction.WorldToLocalPointInRectangle(tfm.position, cv.worldCamera, cv, cv.worldCamera);
            Fishing_Bullet fb = Fishing_Bullet.Create(BelongRole.GameBase.FishingAssetBundle, uiPos, tfm.rotation, m_Data, target, bulletId);
            fb.OwnerCannon = this;
        }
    }

    public uint GetBulletCost()
    {
        return m_nBulletCost;
    }

    public void ChangeLevel(bool bLocal, byte level, uint bulletCost)
    {
        transform.Find(m_Data.m_szLvPoint).GetComponent<UnityEngine.UI.Text>().text = level.ToString();

        if (bLocal)
            m_nBulletCost = bulletCost * (uint)m_Data.m_szBulletPoint.Length;
    }
}

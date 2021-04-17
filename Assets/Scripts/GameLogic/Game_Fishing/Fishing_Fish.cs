using UnityEngine;
using System.Collections;
using SWS;
using DG.Tweening;
using XLua;
using System.Collections.Generic;
using UnityEngine.UI;

[Hotfix]public class Fishing_Fish : MonoBehaviour
{
    public delegate void VoidDelegate(Fishing_Fish fish);
    public VoidDelegate OnDeath;

    public byte m_nTypeId;
    public uint m_nOnlyId;
    public splineMove m_SplineMove;
    Animator m_Animator;
    Fishing_Role m_CatchRole = null;
    bool m_bDeathEnd = false;
    List<byte> m_LockSitList = new List<byte>();
    Renderer[] m_Renderers;
    Transform m_LockPointTfm;
    GameObject m_TalkObj;

    AudioSource m_HitSound;
    AudioSource m_DeadSound;

    public void Init(CGame_Fishing game, splineMove move, uint index, byte typeId)
    {
        AssetBundle ab = game.FishingAssetBundle;
        m_Animator = GetComponent<Animator>();        m_Animator.speed = 0f;        m_SplineMove = move;
        m_nOnlyId = index;
        m_nTypeId = typeId;

        m_Renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in m_Renderers)
            child.enabled = false;

        if (m_SplineMove != null)
        {
            if (m_SplineMove.loopType != splineMove.LoopType.loop)
                m_SplineMove.tween.OnComplete(OnComplete);
            m_SplineMove.tween.OnPlay(OnPlay);
        }
        else
            OnPlay();

        m_LockSitList.Clear();
        m_LockPointTfm = transform.parent.Find("Point_LockTarget");

        FishingFishData fd = Fishing_Data.GetInstance().m_FishData[m_nTypeId];
        if(fd.m_nHitAudio != 0)
        {
            m_HitSound = gameObject.AddComponent<AudioSource>();
            m_HitSound.clip = ab.LoadAsset<AudioClip>(CustomAudioDataManager.GetInstance().GetSoundName(fd.m_nHitAudio));
        }
        if (fd.m_nDeadAudio != 0)
        {
            m_DeadSound = gameObject.AddComponent<AudioSource>();
            m_DeadSound.clip = ab.LoadAsset<AudioClip>(CustomAudioDataManager.GetInstance().GetSoundName(fd.m_nDeadAudio));
        }
        if(fd.m_szTalk.Length > 0)
        {
            m_TalkObj = (GameObject)ab.LoadAsset("talk_Image_BG");
            m_TalkObj = (GameObject)Instantiate(m_TalkObj);
            m_TalkObj.GetComponentInChildren<Text>().text = fd.m_szTalk;
            m_TalkObj.transform.SetParent(game.MainUITfm.Find("point_talk"), false);
            Invoke("ShowTalk", fd.m_nTalkInterval);
            Invoke("HideTalk", fd.m_nTalkInterval + fd.m_nTalkShowTime);
            UpdateTalk();
            m_TalkObj.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (m_TalkObj != null)
            Destroy(m_TalkObj);
    }

    public void OnFreeze(AssetBundle ab, bool bFreeze)
    {
        if (bFreeze)
        {
            Vector3 postion = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 scale = transform.localScale;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;

            Vector3 center = Vector3.zero;
            foreach (Renderer child in m_Renderers)
            {
                center += child.bounds.center;
            }
            center /= m_Renderers.Length;
            Bounds bounds = new Bounds(center, Vector3.zero);
            foreach (Renderer child in m_Renderers)
            {
                bounds.Encapsulate(child.bounds);
            }

            GameObject obj = (GameObject)ab.LoadAsset("Flshing_Bingdong");
            obj = Instantiate(obj);
            obj.transform.SetParent(transform, false);
            obj.transform.localPosition = bounds.center - transform.position;
            obj.transform.localScale = bounds.size;
            obj.name = "Bing";
            obj.layer = gameObject.layer;

            transform.position = postion;
            transform.rotation = rotation;
            transform.localScale = scale;

            if(m_SplineMove != null)
                m_SplineMove.Pause();
        }
        else
        {
            Transform tfm = transform.Find("Bing");
            if (tfm != null)
                Destroy(tfm.gameObject);

            if (m_SplineMove != null)
                m_SplineMove.Resume();
        }
    }

    void ShowTalk()
    {
        if(!Caught())
            m_TalkObj.SetActive(true);
    }

    void HideTalk()
    {
        m_TalkObj.SetActive(false);
    }

    void OnComplete()
    {
        OnDeath(this);
    }

    void OnPlay()
    {
        m_Animator.speed = 1.0f;

        foreach (Renderer child in m_Renderers)
            child.enabled = true;
    }


    private void Update()
    {
        UpdateTalk();
    }

    void UpdateTalk()
    {
        if(m_TalkObj != null && m_TalkObj.activeSelf)
        {
            Canvas cv = m_TalkObj.GetComponentInParent<Canvas>();
            Vector3 uiPos = GameFunction.WorldToLocalPointInRectangle(m_LockPointTfm.position, Camera.main, cv, cv.worldCamera);
            m_TalkObj.transform.localPosition = uiPos;
            float dis = Vector3.Distance(m_LockPointTfm.position, Camera.main.transform.position);
            float scaleFactor = Mathf.Clamp(80f / dis, 0.1f, 1f);
            m_TalkObj.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            //DebugLog.Log("fish scale:" + scaleFactor + " dis:" + dis);
        }
    } 

    public void OnHit(bool bLocal)
    {
        if (m_Renderers != null)
        {
            foreach (Renderer child in m_Renderers)
                foreach (Material mat in child.materials)
                    mat.SetFloat("_Emission", 1f);
        }

        GameMain.WaitForCall(0.1f, () =>
        {
            if (m_Renderers != null)
            {
                foreach (Renderer child in m_Renderers)
                {
                    if (child != null)
                    {
                        foreach (Material mat in child.materials)
                            mat.SetFloat("_Emission", 0f);
                    }
                }
            }
        });

        if(bLocal && m_HitSound != null && Random.Range(0f, 100f) < 30f)
        {
            m_HitSound.volume = AudioManager.Instance.SoundVolume;
            m_HitSound.Play();
        }
    }

    public void OnCatch(Fishing_Cannon cannon, bool bLocal, long reward, FishType_Enum fishType, byte getSkillId, int specialId)
    {
        if (m_CatchRole != null)
            return;

        m_CatchRole = cannon.BelongRole;
        m_Animator.SetTrigger("die");
        gameObject.layer = 0;//no collide
        m_SplineMove.Stop();
        m_CatchRole.GameBase.OnLockFishLost(new List<byte>(m_LockSitList));

        if(bLocal && fishType > FishType_Enum.FishType_Boss)
            m_CatchRole.GameBase.StartLottery(fishType, specialId);
        else
        {
            Canvas cv = m_CatchRole.GameBase.GameCanvas;
            Vector3 pos = GameFunction.WorldToLocalPointInRectangle(transform.position, Camera.main, cv, cv.worldCamera);
            if (fishType == FishType_Enum.FishType_Lottery)
                getSkillId = RoomInfo.NoSit;
            GameMain.SC(PopCoin(this, Fishing_Data.GetInstance().m_FishData[m_nTypeId],
                bLocal, reward, pos, m_CatchRole.GetCoinPos(), getSkillId));
        }

        float deadTime = 3f;
        m_CatchRole.GameBase.m_AddItems.Add(gameObject);
        if (bLocal && m_DeadSound != null)
        {
            m_DeadSound.volume = AudioManager.Instance.SoundVolume;
            m_DeadSound.Play();
            deadTime = m_DeadSound.clip.length;
        }
        GameMain.WaitForCall(deadTime, () =>
            {
                m_CatchRole.GameBase.m_AddItems.Remove(gameObject);
                OnDeath(this);
            });
    }

    void DieEnd()
    {
        m_bDeathEnd = true;
    }

    IEnumerator PopCoin(Fishing_Fish fish, FishingFishData fd, bool bLocal, long getCoin, Vector3 center, Vector3 targetPos, byte getSkillId)
    {
        //DebugLog.Log("num:" + num + " center:" + center + " target:" + targetPos);

        if (getCoin <= 0)
            yield break;

        CGame_Fishing game = fish.m_CatchRole.GameBase;
        AssetBundle ab = game.FishingAssetBundle;
        Canvas cv = game.GameCanvas;
        Transform root = cv.transform.Find("Root");
        GameObject prefab;

        GameObject big = null;
        if (fd.m_nMultiple >= 20)
        {
            prefab = (GameObject)ab.LoadAsset("FishCoin_big");
            big = ((GameObject)GameMain.instantiate(prefab));
            big.transform.SetParent(root, false);
            big.transform.localPosition = center;
            game.m_AddItems.Add(big);
            Camera.main.DOShakePosition(1f, 1f).OnComplete(()=> 
            {
                Camera.main.transform.position = game.CameraSourcePos;
            });

            CustomAudioDataManager.GetInstance().PlayAudio(1004);        }
        else
            CustomAudioDataManager.GetInstance().PlayAudio(1003);

        yield return new WaitUntil(() => m_bDeathEnd);

        Transform effectTfm = fish.transform.parent.Find("Effect");
        if(effectTfm != null)
            effectTfm.DOScale(Vector3.zero, 1f);
        fish.transform.parent.DOScale(Vector3.zero, 1f);

        prefab = (GameObject)ab.LoadAsset(bLocal ? "Text_HurtNum" : "Text_HurtNum_other");
        GameObject textObj = ((GameObject)GameMain.instantiate(prefab));
        textObj.transform.SetParent(root.Find("HurtNum"), false);
        textObj.transform.localPosition = center;
        textObj.transform.localScale = Vector3.zero;
        textObj.transform.GetComponent<Text>().text = GameFunction.FormatCoinText(getCoin, true, false);
        game.m_AddItems.Add(textObj);
        textObj.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).OnComplete(() =>
        {
            textObj.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).OnComplete(()=>
            {
                game.m_AddItems.Remove(textObj);
                Destroy(textObj);
            });
        });

        bool bCoin = true;

        if(getSkillId > 0 && bLocal)
        {
            if(getSkillId == RoomInfo.NoSit)//lottery
            {
                prefab = (GameObject)ab.LoadAsset("Skill_Ticket");
                bCoin = false;
            }
            else
                prefab = (GameObject)ab.LoadAsset("Skill_" + getSkillId);
            if(prefab != null)
            {
                GameObject skillObj = Instantiate(prefab);
                skillObj.transform.SetParent(root, false);
                skillObj.transform.localPosition = center;
                game.m_AddItems.Add(skillObj);
                skillObj.transform.DOLocalMove(center + new Vector3(0, 100f, 0f), 1f).OnComplete(() =>
                {
                    game.m_AddItems.Remove(skillObj);
                    Destroy(skillObj);
                });
            }
        }

        if(bCoin)
        {
            int num = Mathf.CeilToInt((float)fd.m_nMultiple / 5) + 2;
            float radius = (num - 1) * 10f;
            float halfPi = Mathf.PI * 0.5f;
            float angle;
            prefab = (GameObject)ab.LoadAsset("FishCoin");
            GameObject coin;
            Vector3 pos;

            List<GameObject> coinList = new List<GameObject>();

            for (int i = 0; i < num; i++)
            {
                angle = Random.Range(0f, halfPi) * 4f;
                coin = ((GameObject)GameMain.instantiate(prefab));
                coin.transform.SetParent(root, false);
                coin.transform.localPosition = center;
                pos = center + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * Random.Range(0f, radius);
                coin.transform.DOLocalMove(pos, 0.5f);
                //DebugLog.Log(i + ": angle:" + angle + " pos:" + pos);

                coinList.Add(coin);
                game.m_AddItems.Add(coin);

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSecondsRealtime(1f);

            CustomAudioDataManager.GetInstance().PlayAudio(1002);

            float flyTime = 0.5f;

            foreach (GameObject obj in coinList)
            {
                if (obj == null)
                    continue;

                Vector3 dir = targetPos - obj.transform.localPosition;
                Vector3 speed = new Vector3(dir.x / flyTime, 0f);
                float a = dir.y * 2f / (flyTime * flyTime);

                DOTween.To(() => obj.transform.localPosition,
                    r =>
                    {
                        obj.transform.localPosition += (speed * Time.unscaledDeltaTime);
                        speed.y = speed.y + a * Time.unscaledDeltaTime;
                    },
                    targetPos, flyTime);
            }

            yield return new WaitForSecondsRealtime(flyTime);

            foreach (GameObject obj in coinList)
            {
                if (obj == null)
                    continue;

                game.m_AddItems.Remove(obj);
                Destroy(obj);
            }
        }

        if (big != null)
        {
            game.m_AddItems.Remove(big);
            Destroy(big);
        }
    }

    public void Lock(byte sit)
    {
        m_LockSitList.Add(sit);
    }

    public void Unlock(byte sit)
    {
        if (sit == RoomInfo.NoSit)
            m_LockSitList.Clear();
        else
            m_LockSitList.Remove(sit);
    }

    public bool CanLock(byte sit, float offset)
    {
        if (m_LockSitList.Contains(sit))
            return false;

        if (Caught())
            return false;

        return InBound(offset);
    }

    public bool IsLockBySit(byte sit, float offset)
    {
        if (!m_LockSitList.Contains(sit))
            return false;

        if (Caught())
            return false;

        return InBound(offset);
    }

    bool InBound(float offset)
    {
        Vector3 uiPos = RectTransformUtility.WorldToScreenPoint(Camera.main, m_LockPointTfm.position);
        return uiPos.x > offset && uiPos.y > offset && (uiPos.x < Screen.width - offset) && (uiPos.y < Screen.height - offset);
    }

    public Transform GetLockPoint()
    {
        return m_LockPointTfm;
    }

    public bool Caught()
    {
        return gameObject.layer == 0;
    }
}

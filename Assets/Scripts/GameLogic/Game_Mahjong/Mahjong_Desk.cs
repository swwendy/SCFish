using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mahjong_Desk : MonoBehaviour
{
    Animator Anime { get; set; }
    Animator m_DieAnim;    public CGame_Mahjong GameBase { get; set; }
    Coroutine m_FlashCoroutin = null;
    Coroutine m_TimeCoroutin = null;
    Vector3 m_SrcEuler;
    GameObject m_TimeObj;

    // Use this for initialization
    void Awake ()
    {
        Anime = GetComponent<Animator>();
        Anime.Rebind();
        GameObject obj = GameObject.Find("Game_Model/Zhuozi/mesh_shaizi");
        m_DieAnim = obj.GetComponent<Animator>();

        Transform tfm = transform.Find("A6_MJ_Zhuo_Pian_G");
        m_SrcEuler = tfm.localEulerAngles;

        m_TimeObj = GameObject.Find("Game_Model/Zhuozi/mesh_time");
        m_TimeObj.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public void BeginGame()
    {
        GameBase.OnTileNumChanged(0);
        Anime.SetBool("xiajiang", true);
    }

    public void OnEnd()
    {
        m_TimeObj.SetActive(false);
        m_DieAnim.gameObject.SetActive(false);
        Transform tfm = transform.Find("A6_MJ_Zhuo_Pian_E");
        tfm.gameObject.SetActive(true);

        Anime.Rebind();
    }

    void jiangEnd()
    {
        GameBase.ShowWallTiles();

        Anime.SetBool("xiajiang", false);
    }

    public void OnRotDir(byte localSeverSit)
    {
        Transform tfm = transform.Find("A6_MJ_Zhuo_Pian_G");
        Vector3 rot = m_SrcEuler;
        rot.y += 90f * localSeverSit;
        tfm.localEulerAngles = rot;
    }

    public void OnPlayDie(byte[] points, bool midEnt)
    {
        if (m_DieAnim == null)
            return;

        m_DieAnim.runtimeAnimatorController = GameBase.MahjongAssetBundle
            .LoadAsset<RuntimeAnimatorController>("mj_shaiziRun_" + Random.Range(1, 4));
        m_DieAnim.gameObject.SetActive(true);
        Transform tfm = transform.Find("A6_MJ_Zhuo_Pian_E");
        tfm.gameObject.SetActive(false);

        for (int i = 0; i < points.Length; i++)
        {
            tfm = m_DieAnim.transform.GetChild(i);
            if (tfm == null)
                break;

            Vector2 uv = new Vector2((points[i] + 1) % 3 / 3f, points[i] > 3 ? 0f : 0.5f);
            tfm.GetComponent<MeshRenderer>().material.mainTextureOffset = uv;
        }

        if(!midEnt)
        {
            CustomAudioDataManager.GetInstance().PlayAudio(3001);
            m_DieAnim.SetBool("shaiziRun", true);
            GameMain.WaitForCall(2f, () => m_DieAnim.SetBool("shaiziRun", false));
        }
    }

    public void ShowTurn(byte serverSit)
    {
        Transform tfm = transform.Find("A6_MJ_Zhuo_Pian_G");
        Material[] mats = tfm.GetComponent<MeshRenderer>().materials;
        for(int i = 1; i <= 4; i++)
            mats[i].SetFloat("_Emission", 0f);
        if(m_FlashCoroutin != null)
            StopCoroutine(m_FlashCoroutin);
        if(serverSit != RoomInfo.NoSit)
            m_FlashCoroutin = StartCoroutine(FlashMat(mats[serverSit + 1]));
        else
            tfm.localEulerAngles = m_SrcEuler;
    }

    IEnumerator FlashMat(Material mat)
    {
        if (mat == null)
            yield break;

        while(true)
        {
            mat.SetFloat("_Emission", 1f);
            yield return new WaitForSecondsRealtime(0.5f);
            mat.SetFloat("_Emission", 0.3f);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void StartCountdown(float time, bool pause = false, bool warn = false)
    {
        if (m_TimeCoroutin != null)
            StopCoroutine(m_TimeCoroutin);

        if(time < 0f)
        {
            m_TimeObj.SetActive(false);
            m_DieAnim.gameObject.SetActive(true);
            return;
        }

        if (!m_TimeObj.activeSelf)
        {
            m_TimeObj.SetActive(true);
            m_DieAnim.gameObject.SetActive(false);
        }

        m_TimeCoroutin = StartCoroutine(ShowCountDown(time, pause, warn));
    }

    IEnumerator ShowCountDown(float secondLeft, bool pause, bool warn, UnityAction action = null)
    {
        MeshRenderer mr = m_TimeObj.GetComponent<MeshRenderer>();
        Material[] mats = mr.materials;
        if (mats == null || mats.Length < 2)
            yield break;

        int nTime = (int)secondLeft;

        while (nTime >= 0)
        {
            int digit = nTime % 100 / 10;
            Vector2 uv = new Vector2((digit % 5) * 0.2f, digit >= 5 ? 0.5f : 0f);
            mats[0].mainTextureOffset = uv;

            digit = nTime % 10;
            uv = new Vector2((digit % 5) * 0.2f, digit >= 5 ? 0.5f : 0f);
            mats[1].mainTextureOffset = uv;

            mr.materials = mats;

            if (pause)
                yield break;

            if(warn && nTime <= 5)
                CustomAudioDataManager.GetInstance().PlayAudio(3007);

            yield return new WaitForSecondsRealtime(1f);

            nTime -= 1;
        }

        yield return null;

        if (action != null)
            action();
    }
}

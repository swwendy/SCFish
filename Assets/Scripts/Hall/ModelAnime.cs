using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MatChangeInfo
{
    public GameObject changeNode;
    public byte index;
    public float interval;
    public float changeInterval;
    public List<Material> mats;
}

public class ModelAnime : MonoBehaviour
{
    public float interval = 5f;
    public List<string> param = new List<string>();

    public List<MatChangeInfo> MatChange = new List<MatChangeInfo>();

    Animator m_Animator;

    // Use this for initialization
    void Start ()
    {
        m_Animator = GetComponent<Animator>();

        if(m_Animator != null && param.Count > 0)
            InvokeRepeating("RandomStartAnime", interval, interval);

        foreach(MatChangeInfo info in MatChange)
        {
            StartCoroutine(ChangeMat(info));
        }
    }

    // Update is called once per frame
    void Update ()
    {
	}

    void RandomStartAnime()
    {
        string rand = param[Random.Range(0, param.Count)];
        m_Animator.SetTrigger(rand);
    }

    IEnumerator ChangeMat(MatChangeInfo info)
    {
        if (info == null || info.changeNode == null || info.mats.Count < 2)
            yield break;

        SkinnedMeshRenderer smr = info.changeNode.GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
            yield break;

        UnityEngine.Material[] mats = smr.materials;

        List<Material> list = new List<Material>(info.mats);
        List<Material> listAdd = new List<Material>(info.mats);
        listAdd.Reverse();
        list.AddRange(listAdd);

        while (true)
        {
            yield return new WaitForSecondsRealtime(info.interval);

            for(int i = 0; i < list.Count; i++)
            {
                mats[info.index] = list[i];
                smr.materials = mats;
                yield return new WaitForSecondsRealtime(info.changeInterval);
            }
        }
    }
}

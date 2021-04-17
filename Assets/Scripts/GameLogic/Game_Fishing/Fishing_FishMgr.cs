using UnityEngine;
using System.Collections;
using SWS;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Events;
using XLua;

using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;

using PathFishList = System.Collections.Generic.Dictionary<SWS.PathManager, PathInfo>;
using System.Linq;

[LuaCallCSharp]public class PathInfo
{
    public float speed;
    public float time;
    public List<Fishing_Fish> fishList;
}

[Hotfix]public class Fishing_FishMgr
{
    CGame_Fishing m_GameBase;
    WaypointManager m_WaypointMgr;
    PathFishList m_PathFishList = new PathFishList();
    Dictionary<uint, Fishing_Fish> m_FishDict = new Dictionary<uint, Fishing_Fish>();
    Dictionary<ushort, PathManager> m_PathDict = new Dictionary<ushort, PathManager>();
    public bool FishPause { get; private set; }

    public Fishing_FishMgr(CGame_Fishing game)
    {
        m_GameBase = game;
        FishPause = false;
        m_WaypointMgr = GameObject.Find("Waypoint Manager").GetComponent<WaypointManager>();
    }

    public void CreatePathFish(ushort pathId, uint fishId, byte typeId, float pathPassTime, byte group, byte index)
    {
        if (pathPassTime < 0f)
            return;

        if(!Fishing_Data.GetInstance().m_PathData.ContainsKey(pathId))
        {
            DebugLog.LogWarning("Error: reate path id(" + pathId + ") no path data!");
            return;
        }
        FishingPathData pd = Fishing_Data.GetInstance().m_PathData[pathId];

        PathManager pm;
        PathInfo pi = null;
        List<Fishing_Fish> fishList = null;

        if(m_PathDict.ContainsKey(pathId))
        {
            pm = m_PathDict[pathId];
            if (m_PathFishList.ContainsKey(pm))
            {
                pi = m_PathFishList[pm];
                fishList = pi.fishList;
            }
            else
                DebugLog.LogError("fishMgr pathFishList not contain dict's pm!");
        }
        else
        {
            //create path
            GameObject prefab = (GameObject)m_GameBase.FishingAssetBundle.LoadAsset(pd.m_szPath);
            if(prefab == null)
            {
                DebugLog.LogWarning("Fail to preload path:" + pd.m_szPath);
                return;
            }
            GameObject path = (GameObject)GameMain.instantiate(prefab);
            path.transform.SetParent(m_WaypointMgr.transform);

            pm = path.GetComponent<PathManager>();
            fishList = new List<Fishing_Fish>();
            pi = new PathInfo();
            pi.speed = pd.m_fSpeed;
            pi.time = pd.m_fTime;
            m_PathDict[pathId] = pm;
        }

        Fishing_Fish[] fish = CreatFish(typeId, pd, pm.transform, pm, fishId, 1, pathPassTime, group, index);
        fishList.AddRange(fish);
        pi.fishList = fishList;
        m_PathFishList[pm] = pi;

        if (pd.m_fSpeed != 0f)
        {
            float endX = pm.transform.localPosition.x + pd.m_fSpeed * pd.m_fTime;

            pm.transform.Translate(pd.m_fSpeed * pathPassTime, 0f, 0f);

            Tweener t = pm.transform.DOLocalMoveX(endX, pd.m_fTime - pathPassTime);
            if (pd.m_bLoop)
                t.OnComplete(() => RemovePath(pm));
        }
    }

    float GetPathPassTime(float passTime, splineMove sm, FishingPathData pd)
    {
        if (pd.m_ChangePoints.Count == 0)
            return passTime;

        float value;
        float speed = sm.speed;
        float countTime = passTime;
        List<byte> changeList = new List<byte>(pd.m_ChangePoints.Keys);

        if(passTime > 0f)
        {
            sm.tween.ForceInit();

            TweenerCore<Vector3, Path, PathOptions> tweenPath = sm.tween as TweenerCore<Vector3, Path, PathOptions>;
            float time = 0f;
            countTime = 0f;
            float curSpeed = speed;
            for (byte i = 0; i < sm.pathContainer.GetWaypointCount(); i++)
            {
                value = tweenPath.changeValue.wpLengths[i] / curSpeed;
                time += value;
                countTime += value;

                if (passTime <= time)
                {
                    countTime -= (time - passTime);
                    break;
                }

                if (pd.m_ChangePoints.ContainsKey(i))
                {
                    changeList.Remove(i);

                    if (i >= sm.events.Count)
                    {
                        DebugLog.LogWarning("Error: fish point count(" + sm.events.Count + ") out path(" + pd.m_szPath + ") change config!!");
                        break;
                    }

                    value = pd.m_ChangePoints[i];
                    if (value > 0f)
                    {
                        sm.events[i].AddListener(() => sm.ChangeSpeed(value * speed));
                        curSpeed = speed * value;
                    }
                    else
                    {
                        time -= value;
                        if (passTime <= time)
                        {
                            sm.events[i].AddListener(() => sm.Pause(time - passTime));
                            break;
                        }
                    }
                }
            }
        }

        foreach (byte i in changeList)
        {
            if(i >= sm.events.Count)
            {
                DebugLog.LogWarning("Error: fish point count(" + sm.events.Count + ") out path(" + pd.m_szPath +") change config!!");
                break;
            }
            value = pd.m_ChangePoints[i];
            if (value > 0f)
                sm.events[i].AddListener(() => sm.ChangeSpeed(value * speed));
            else
                sm.events[i].AddListener(() => sm.Pause(-value));
        }

        return countTime;
    }

    Fishing_Fish[] CreatFish(byte fishTypeId, FishingPathData pd, Transform parent, PathManager pm, uint fishId, int count, float passTime, byte group, byte index)
    {
        if(!Fishing_Data.GetInstance().m_FishData.ContainsKey(fishTypeId))
        {
            DebugLog.LogError("fish:" + fishTypeId + " data is error!!");
            return null;
        }

        FishingFishData fd = Fishing_Data.GetInstance().m_FishData[fishTypeId];
        GameObject prefab = (GameObject)m_GameBase.FishingAssetBundle.LoadAsset(fd.m_szFish);

        int arrayCount = pd.m_Offsets.Count > 0 ? pd.m_Offsets.Count : 1;
        Debug.Assert(count != 0 && count <= arrayCount, "create fish count wrong!!");

        Fishing_Fish[] fishs = new Fishing_Fish[count];
        for(uint j = 0; j < count; j++)
        {
            GameObject go = (GameObject)GameMain.instantiate(prefab);
            go.transform.SetParent(parent, false);
            splineMove sm = go.AddComponent<splineMove>();
            sm.local = true;
            sm.lookAhead = 0.01f;
            sm.forwardDir = Vector3.right;
            sm.pathMode = PathMode.Ignore;
            sm.offset = index < pd.m_Offsets.Count ? pd.m_Offsets[index] : Vector3.zero;
            sm.waypointRotation = (fd.m_nRotType == 1) ? splineMove.RotationType.all : splineMove.RotationType.none;
            if (m_GameBase.IsMirror())
                sm.eulerAngles = (fd.m_nRotType == 1) ? new Vector3(180f, 0f, 0f) : new Vector3(0f, 0f, 180f);
            else
                sm.eulerAngles = Vector3.zero;
            sm.speed = fd.m_GroupSpeed[group];
            sm.loopType = pd.m_bLoop ? splineMove.LoopType.loop : splineMove.LoopType.none;
            sm.SetPath(pm);
            sm.GoTo(GetPathPassTime(passTime, sm, pd));

            if (FishPause)
                sm.Pause();

            Fishing_Fish fish = go.transform.Find("skin").gameObject.AddComponent<Fishing_Fish>();
            fish.Init(m_GameBase, sm, fishId + j, fishTypeId);
            fish.OnDeath += RemoveFish;
            fishs[j] = fish;
            m_FishDict.Add(fish.m_nOnlyId, fish);
        }
        return fishs;
    }

    int SortByMultiple(Fishing_Fish a, Fishing_Fish b)//按倍率从大到小排列
    {
        FishingFishData dataA = Fishing_Data.GetInstance().m_FishData[a.m_nTypeId];
        FishingFishData dataB = Fishing_Data.GetInstance().m_FishData[b.m_nTypeId];
        return dataB.m_nMultiple - dataA.m_nMultiple;
    }

    void RemoveFish(Fishing_Fish fish)
    {
        if (!m_PathFishList.ContainsKey(fish.m_SplineMove.pathContainer))
            return;

        PathManager pm = fish.m_SplineMove.pathContainer;
        PathInfo pi = m_PathFishList[pm];
        if (!pi.fishList.Contains(fish))
            return;

        m_FishDict.Remove(fish.m_nOnlyId);

        pi.fishList.Remove(fish);
        GameObject.Destroy(fish.transform.parent.gameObject);

        if(pi.fishList.Count == 0)
        {
            RemovePath(pm);
        }
    }

    public void RemoveFish(uint id)
    {
        if (!m_FishDict.ContainsKey(id))
            return;

        RemoveFish(m_FishDict[id]);
    }

    void RemovePath(PathManager pm)
    {
        if (!m_PathFishList.ContainsKey(pm))
            return;

        foreach(Fishing_Fish fish in m_PathFishList[pm].fishList)
        {
            m_FishDict.Remove(fish.m_nOnlyId);
        }

        ushort key = m_PathDict.FirstOrDefault(q => q.Value == pm).Key;
        m_PathDict.Remove(key);

        m_PathFishList.Remove(pm);
        GameObject.Destroy(pm.gameObject);
    }

    public void ClearScene()
    {
        DebugLog.Log("Begin clear...............");

        foreach (Fishing_Fish fish in m_FishDict.Values)
        {
            fish.m_SplineMove.ChangeSpeed(fish.m_SplineMove.speed * 10f);
            foreach (UnityEvent evt in fish.m_SplineMove.events)
            {
                evt.RemoveAllListeners();
            }
        }
    }

    public Fishing_Fish LockNextFish(byte sit)
    {
        List<Fishing_Fish> fishList = new List<Fishing_Fish>(m_FishDict.Values);
        fishList.Sort(SortByMultiple);

        foreach (Fishing_Fish fish in fishList)
        {
            if (fish.CanLock(sit, 0f))
            {
                return fish;
            }
        }

        return null;
    }

    public Fishing_Fish GetFishById(uint onlyId)
    {
        if (m_FishDict.ContainsKey(onlyId))
            return m_FishDict[onlyId];

        return null;
    }

    public void OnEnd()
    {
        foreach(PathManager pm in m_PathFishList.Keys)
        {
            GameObject.Destroy(pm.gameObject);
        }
        m_PathFishList.Clear();
        m_FishDict.Clear();
        m_PathDict.Clear();
        FishPause = false;
    }

    public void PauseScene(bool pause)
    {
        FishPause = pause;

        foreach (Fishing_Fish fish in m_FishDict.Values)
        {
            fish.OnFreeze(m_GameBase.FishingAssetBundle, pause);
        }
    }

}

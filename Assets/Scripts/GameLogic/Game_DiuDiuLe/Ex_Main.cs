using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;
using DragonBones;
using XLua;

[Hotfix]
public class CGame_DiuDiuLe : CGameBase
{
    static GameObject mUICanvas;
    static Ex_IBaseUI curUI;
    static AssetBundle bundle;
    static List<GameObject> UIArr;

    public CGame_DiuDiuLe() : base(GameKind_Enum.GameKind_DiuDiuLe)
    {
        mUICanvas = null;
        curUI = null;
        bundle = null;
        UIArr = new List<GameObject>();
    }

    public void InitDDLMsg()
    { 
}

    // Use this for initialization
    public override void Initialization()
    {
        base.Initialization();

        mUICanvas = GameObject.Find("Canvas/Root");
        bundle = BundleIns();
        RemoveChildrens();
        //设置屏幕自动旋转， 并置支持的方向
        //Screen.orientation = ScreenOrientation.PortraitUpsideDown;
        //Screen.autorotateToLandscapeLeft = false;
        //Screen.autorotateToLandscapeRight = false;
        //Screen.autorotateToPortrait = true;
        //Screen.autorotateToPortraitUpsideDown = true;
        
        if (Ex_GameData.isReconect)
        {
            Ex_GameData.isReconect = false;
            GameObject tableUI = LoadPrefab("DiuDiuLe_Game");
            Ex_Table ex_table = new Ex_Table();
            ex_table.InitUI(tableUI, ex_table, true, Ex_GameData.SMGameScene.areaId);
        }
        else
        {
            GameObject homeUI = LoadPrefab("DiuDiuLe_Lobby");
            Ex_Home ex_home = new Ex_Home();
            ex_home.InitUI(homeUI, ex_home);
        }
    }

    // Update is called once per frame
    public override void ProcessTick ()
    {
        base.ProcessTick();

        if (curUI != null)
        {
            curUI.Update();
        }
	}

    public override void RefreshGamePlayerCoin(uint AddMoney)
    {
        base.RefreshGamePlayerCoin(AddMoney);
    }


    public static void SetCurUI(Ex_IBaseUI curClass)
    {
        curUI = curClass;
    }

    public static GameObject LoadPrefab(string resUrl)
    {
        GameObject go_rt = null;
        Object obj0 = bundle.LoadAsset(resUrl);
        go_rt = (GameObject)GameMain.instantiate(obj0);
        go_rt.transform.SetParent(mUICanvas.transform,false);
        UIArr.Add(go_rt);
        return go_rt;
    }

    

    public static GameObject InsPrefab(string resUrl, GameObject go_parent)
    {
        GameObject go_rt = null;
        Object obj0 = bundle.LoadAsset(resUrl) as Object;
        go_rt = (GameObject)GameMain.instantiate(obj0);
        go_rt.transform.SetParent(go_parent.transform,false);
        return go_rt;
    }

    public static void RemoveChildrens()
    {
        //for (int i = 0; i < mUICanvas.transform.childCount; i++)
        //{
        //    GameObject _go = mUICanvas.transform.GetChild(i).gameObject;
        //    Object.Destroy(_go);
        //}
        for (int i = 0; i < UIArr.Count; i++)
        {
            GameObject _go = UIArr[i];
            Object.Destroy(_go);
        }
    }

    public static AssetBundle BundleIns()
    {
        if (bundle == null)
        {
            //CResVersionCompareUpdate.CompareABVersionAndUpdate("diu.resource");
            //HttpDownload.DownFile(GameDefine.LuancherURL, GameDefine.AssetBundleSavePath, "diu.resource");
            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_DiuDiuLe);
            if (gamedata != null)
            {
                //AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, gamedata.ResourceABName);

                bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            }
        }
        return bundle;
    }

    public static void ExitToHall()
    {
        //UnityFactory.factory.Clear();
        curUI = null;
        GameMain.hall_.SwitchToHallScene();
       
    }
}

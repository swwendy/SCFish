using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XLua;
using USocket.Messages;

[Hotfix]
public class testExample : MonoBehaviour
{
    LuaEnv luaenv = new LuaEnv();
    public static readonly string m_PathURL = "http://192.166.0.172/Luancher/test.lua.txt";
    public static readonly string m_PathURL1 = "http://192.166.0.172/Luancher/abscene";
    // Use this for initialization
    void Start ()
    {
       // GameObject.DontDestroyOnLoad(this);
        //NetWorkClient.GetInstance().InitNetWork("192.166.0.172", 16201);
//         GameObject game = GameObject.Find("Game_Icon3");
//         Button btn = game.GetComponent<Button>();
//         btn.onClick.AddListener(
//         delegate ()
//         {
//             change2GameScene();
//         });

        //StartCoroutine(LoadGameObjectPackedByThemselves(m_PathURL));

        //CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.EMSG_U3D_CLIENT_LOGIN, RoleLoginMsg);

        //设置屏幕自动旋转， 并置支持的方向
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;

        //获取手机标识码
        string uid = SystemInfo.deviceUniqueIdentifier;        //网络状态        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)        {            Debug.Log("当前用的是wifi");        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log("update in C#");
        //NetWorkClient.GetInstance().Update();
	}

    public bool RoleLoginMsg(uint _msgType, UMessage _msg)
    {
       int  MainPlayerId = _msg.ReadInt();
        string playerName = _msg.ReadString();
        Debug.Log("玩家ID:"+ MainPlayerId+playerName);
        return true;
    }

    private IEnumerator LoadGameObjectPackedByThemselves(string path)
    {
        WWW bundle = new WWW(path);
        yield return bundle;

        //加载  
        /* Object obj0 = bundle.assetBundle.LoadAsset("aaaaa");
         yield return Instantiate(obj0);*/
        //bundle.assetBundle.Unload(false);
        luaenv.DoString(bundle.text);
        bundle.Dispose();
        // 
    }

    private IEnumerator LoadAssetBundleScene(string path)
    {
        WWW bundle = WWW.LoadFromCacheOrDownload(path,1);
        yield return bundle;

        //加载  
        /* Object obj0 = bundle.assetBundle.LoadAsset("aaaaa");
         yield return Instantiate(obj0);*/
        //bundle.assetBundle.Unload(false);
        AsyncOperation async_operation;
        async_operation = SceneManager.LoadSceneAsync("Scene");
        yield return async_operation;
        //luaenv.DoString(bundle.text);
        bundle.Dispose();
        // 
    }

    private void OnDestroy()
    {
        NetWorkClient.GetInstance().CloseNetwork();
    }

    public void testadd(int a,int b)
    {
        int c = a+b ;
        Debug.Log("a+b =" + c);
    }

    void change2GameScene()
    {
        //SceneManager.LoadScene(1);

        /*UMessage msgEnter = new UMessage((uint)GameCity.EMSG_ENUM.EMSG_U3D_CLIENT_LOGIN);
        msgEnter.Add(1025411);
        msgEnter.Add(756231);
        msgEnter.Add("aaaaaa");
        msgEnter.Add("bbbbb");
        msgEnter.Add("ccccc");
        msgEnter.Add("dddddd");
        NetWorkClient.GetInstance().SendMsg(msgEnter);

        StartCoroutine(LoadAssetBundleScene(m_PathURL1));
        Debug.Log("button click aaaaa");*/

       

        testadd(3,5);
    }

    void LoadAssetbundleExample()
    {
        //string bundlepath = Application.dataPath + "/AssetBundles/IOS/test";
        //AssetBundle bundle = AssetBundle.LoadFromFile(bundlepath);

        //Sprite obj = bundle.LoadAsset("sadfaf") as Sprite;
    }
}


public class testgameclass:CGameBase
{


    public override void ProcessTick()
    {
        base.ProcessTick();
        Debug.Log("childclass update....");
    }
}
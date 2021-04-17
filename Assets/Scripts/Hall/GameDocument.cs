using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//游戏文档界面
public class GameDocumentPanel
{
    GameObject RootGameObject = null;
    //游戏文档界面对象
    GameObject DocumentPanelGameObject = null;
    //游戏规则介绍界面
    GameObject RightGameRule = null;
    //游戏规则数据
    Dictionary<byte, GameObject> GameDocumentDictionary;

    /// <summary>
    /// 初始化游戏文档界面
    /// </summary>
    public void InitGameDocumentPanelResource()
    {
        if(RootGameObject == null)
        {
            RootGameObject = GameObject.Find("Canvas/Root");
        }

        if (GameDocumentDictionary == null)
        {
            GameDocumentDictionary = new Dictionary<byte, GameObject>();
        }

        if (DocumentPanelGameObject ==  null)
        {
            GameDocumentDictionary.Clear();
            AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (assetBundle)
            {
                Object LobbyGameRule = assetBundle.LoadAsset("Lobby_Game_Rule");
                DocumentPanelGameObject = (GameObject)GameMain.instantiate(LobbyGameRule);
                DocumentPanelGameObject.SetActive(false);
                DocumentPanelGameObject.transform.SetParent(RootGameObject.transform, false);

                //收集游戏规则介绍列表对象
                string childUiName;
                UnityEngine.Transform gameTransform = null;
                for (GameKind_Enum gameKind = GameKind_Enum.GameKind_CarPort; gameKind < GameKind_Enum.GameKind_Max; ++gameKind)
                {
                    childUiName = "Left/Toggle_" + (byte)gameKind;
                    gameTransform = DocumentPanelGameObject.transform.Find(childUiName);
                    if (gameTransform)
                    {
                        gameTransform.gameObject.SetActive(false);
                        GameDocumentDictionary.Add((byte)gameKind, gameTransform.gameObject);
                    }
                }

                RightGameRule = DocumentPanelGameObject.transform.Find("Right/Game_Rule").gameObject;
            }
            InitGameDocumentPanelUIEvents();
        }
    }

    /// <summary>
    /// 初始化游戏规则界面事件
    /// </summary>
    private void InitGameDocumentPanelUIEvents()
    {
        if(DocumentPanelGameObject == null || GameDocumentDictionary == null)
        {
            return;
        }

        Transform returnButton =  DocumentPanelGameObject.transform.Find("Top/ButtonReturn");
        XPointEvent.AutoAddListener(returnButton.gameObject, OnClickButtonReturnDocumentPanel, null);
    }

    /// <summary>
    /// 返回游戏大厅界面（禁用本界面）
    /// </summary>
    void OnClickButtonReturnDocumentPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if(eventtype == EventTriggerType.PointerClick)
        {
            SetGameDocumentActivePanel(false);
        }
    }

    /// <summary>
    /// 激活或禁用游戏规则界面
    /// </summary>
    /// <param name="ShowState">true: 激活，false :禁用</param>
    public void SetGameDocumentActivePanel(bool ShowState, Dictionary<byte, GameObject> GameListData = null)
    {
        if(null == DocumentPanelGameObject)
        {
            return;
        }
        //游戏左边游戏列表
        bool GameDocumentActive = true;
        if(GameListData != null && GameDocumentDictionary != null)
        {
            GameObject OutGameObject = null;
            foreach(var gameData in GameListData)
            {
                if(!GameDocumentDictionary.TryGetValue(gameData.Key, out OutGameObject))
                {
                    continue;
                }
                OutGameObject.SetActive(true);
            }
        }else
        {
            GameDocumentActive = false;
        }
        //游戏右边规则介绍
        if (RightGameRule)
        {
            RightGameRule.SetActive(GameDocumentActive);
        }
        DocumentPanelGameObject.SetActive(ShowState);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

/// <summary>
/// 象棋主控玩家对象
/// </summary>
[Hotfix]
public class CCHess_RoleLocal : CChess_Role 
{
    /// <summary>
    /// 当前鼠标选中的棋子对象数据
    /// </summary>
    int m_CurSelectPiecePostion = 0;

    /// <summary>
    /// 警告音效时间
    /// </summary>
    float WarningTime = 1;

    /// <summary>
    /// 警告图片对象
    /// </summary>
    Image CountDownWarningTimeImage = null;

    /// <summary>
    /// 警告图片对象缩放
    /// </summary>
    Vector3 ImageScale = Vector3.zero;

    public CCHess_RoleLocal(CGame_Chess gameBase, byte cSit):base(gameBase,cSit)
    {
        CountDownWarningTimeImage = m_RoleInfoTranform.parent.Find("TimeTips").GetComponent<Image>();
    }

    /// <summary>
    /// 玩家游戏逻辑推进
    /// </summary>
    public override void OnTick()    {
        base.OnTick();
        UpdateChessPlayerWarningTime();
    }

    /// <summary>
    /// 更新象棋警告时间
    /// </summary>
    private void UpdateChessPlayerWarningTime()
    {
        if (m_GameBase == null  || CountDownWarningTimeImage == null)
        {
            return;
        }

        if (PlayChessTime  > 10 || WarningAudioCount < 0)
        {
            if(CountDownWarningTimeImage.gameObject.activeSelf)
            {
                WarningTime = 1;
                CountDownWarningTimeImage.gameObject.SetActive(false);
            }
            return;
        }

        WarningTime += Time.deltaTime;
        if (CountDownWarningTimeImage.gameObject.activeSelf)
        {
            ImageScale.x = WarningTime + 0.5f;
            ImageScale.y = ImageScale.x;
            if (ImageScale.x >= 1.5f)
            {
                --WarningAudioCount;
                CountDownWarningTimeImage.gameObject.SetActive(false);
                if (WarningAudioCount < 0)
                {
                    WarningTime = 1;
                    return;
                }
            }
        }
        if (WarningTime >= 1)
        {
            if (WarningAudioCount < 10 && PlayChessTime < 9.5f)
            {
                if (m_GameBase.m_ChessAssetBundle)
                {
                    CountDownWarningTimeImage.sprite = m_GameBase.m_ChessAssetBundle.LoadAsset<Sprite>(string.Format("xq_word_sz_{0}", WarningAudioCount));
                }
                ImageScale.x = ImageScale.y = 0.5f;
                CountDownWarningTimeImage.gameObject.SetActive(true);
            }
            CustomAudioDataManager.GetInstance().PlayAudio(1006);
            WarningTime -= 1.0f;
        }
        CountDownWarningTimeImage.transform.localScale = ImageScale;
    }

    /// <summary>
    /// 判断两条直线之间是否有棋子存在
    /// </summary>
    /// <param name="PostionValue"></param>
    /// <param name="StartValue"></param>
    /// <param name="EndValue"></param>
    /// <param name="PieceNum"></param>
    /// <param name="YState"></param>
    /// <returns></returns>
    bool CheckPiecePostionPiecesType(int PostionValue,int StartValue,int EndValue, EChessPiecesType PieceType ,out int PieceNum,bool YState = true)
    {
        PieceNum = 0;
        if (m_GameBase == null)
        {
            return true;
        }
        bool State = false;
        int SymbolVlaue = EndValue - StartValue > 0 ? 1 : -1;
        int indexY = StartValue + SymbolVlaue;
        while (indexY != EndValue)
        {
            int NewPostion = YState ? PostionValue * 100 + indexY : PostionValue + indexY * 100;
            if (m_GameBase.IsCheckerboardPostionPieceType(NewPostion))
            {
                PieceNum++;
                if(PieceType == EChessPiecesType.CChessType_Rook_1 ||
                   PieceType == EChessPiecesType.CChessType_Rook_2)
                {
                    State = true;
                    break;
                }else if(PieceType == EChessPiecesType.CChessType_Cannon_1 ||
                         PieceType == EChessPiecesType.CChessType_Cannon_2)
                {
                    if(PieceNum >= 2)
                    {
                        State = true;
                        break;
                    }
                }
            }
            indexY += SymbolVlaue;
        }
        return State;
    }


    /// <summary>
    /// 检测棋子行走位置是否非法
    /// </summary>
    /// <param name="PieceType"></param>
    /// <param name="EndPostion"></param>
    /// <returns></returns>
    bool CheckMovePiecePostionState(EChessPiecesType PieceType,int EndPostion)
    {
        if(m_GameBase == null)
        {
            return true;
        }
        #region "第一个检测版本"
        //if (!m_GameBase.IsCheckerboardPostionPieceType(EndPostion))
        //{
        //    Transform PieceTransform = m_GameBase.GetPieceTransform(EndPostion);
        //    if(PieceTransform)
        //    {
        //        return !PieceTransform.FindChild("Image_point (1)").gameObject.activeSelf;
        //    }
        //}
        //return false;
        #endregion
        #region "第二个检测版本"
        int StartX = m_CurSelectPiecePostion / 100;
        int StartY = m_CurSelectPiecePostion % 100;
        int EndX = EndPostion / 100;
        int EndY = EndPostion % 100;
        int XValue = Mathf.Abs(EndX - StartX);
        int YValue = Mathf.Abs(EndY - StartY);
        DebugLog.Log("StartX = "+ StartX + " StartY = "+ StartY + " EndX = "+ EndX + " EndY = "+ EndY);
        switch (PieceType)
        {
            case EChessPiecesType.CChessType_General://将
                {
                    if (EndX <3 || EndX > 5 || EndY < 7 ||(XValue != 0 && YValue != 0)||
                      (XValue == 0 && YValue > 1) || (YValue == 0 && XValue > 1))
                    {
                        return true;
                    }
                }
                break;
            case EChessPiecesType.CChessType_Guard_1://士
            case EChessPiecesType.CChessType_Guard_2:
                {
                    if (EndX < 3 || EndX > 5 || EndY < 7 || (XValue != 1 || YValue != 1))
                    {
                        return true;
                    }
                }
                break;
            case EChessPiecesType.CChessType_Elephant_1://象
            case EChessPiecesType.CChessType_Elephant_2:
                {
                    int NewPostion = (StartX +((EndX - StartX) / 2)) * 100 + StartY+(EndY - StartY)/2;
                    if (EndY < 5 || (XValue != 2 || YValue != 2) ||
                       (XValue == 2 && YValue == 2 && EChessPiecesType.CChessType_None != GetChessPieceTypeByPostion(NewPostion)))
                    {
                        return true;
                    }
                }
                break;
            case EChessPiecesType.CChessType_Cannon_1://炮
            case EChessPiecesType.CChessType_Cannon_2:
            case EChessPiecesType.CChessType_Rook_1://车
            case EChessPiecesType.CChessType_Rook_2:
                {
                    int OutPieceNum = 0;
                    bool returnPeiceNumState = false;
                    if (XValue != 0 && YValue != 0)
                    {
                        return true;
                    }
                    else if (XValue == 0 && YValue != 0)
                    {
                        returnPeiceNumState = CheckPiecePostionPiecesType(StartX, StartY, EndY, PieceType,out OutPieceNum);
                    }
                    else if (YValue == 0 && XValue != 0)
                    {
                        returnPeiceNumState = CheckPiecePostionPiecesType(StartY, StartX, EndX, PieceType,out OutPieceNum, false);
                    }

                    if((PieceType == EChessPiecesType.CChessType_Cannon_1 || PieceType == EChessPiecesType.CChessType_Cannon_2)&& 
                       !returnPeiceNumState && OutPieceNum == 1 && !m_GameBase.IsCheckerboardPostionPieceType(EndPostion))
                    {
                        returnPeiceNumState = true;
                    }

                    if (returnPeiceNumState)
                    {
                        return true;
                    }
                }
                break;
            case EChessPiecesType.CChessType_Horse_1://马
            case EChessPiecesType.CChessType_Horse_2:
                {
                    if((XValue == 2 && YValue == 1) || (XValue == 1 && YValue == 2))
                    {
                        int NewPostion = 0;
                        if (XValue == 2)
                        {
                            NewPostion = (StartX + ((EndX - StartX) / 2)) * 100 + StartY;
                    
                        }
                        else
                        {
                            NewPostion = StartX * 100 + StartY + (EndY - StartY)/2;
                        }
                        if (m_GameBase.IsCheckerboardPostionPieceType(NewPostion))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                break;
            case EChessPiecesType.CChessType_Soldier_1://卒
            case EChessPiecesType.CChessType_Soldier_2:
            case EChessPiecesType.CChessType_Soldier_3:
            case EChessPiecesType.CChessType_Soldier_4:
            case EChessPiecesType.CChessType_Soldier_5:
                {
                    if(EndY > StartY)
                    {
                        return true;
                    }

                    if(StartY < 5)
                    {
                        if (XValue > 1 || YValue > 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if(XValue >= 1 || YValue > 1)
                        {
                            return true;
                        }
                    }
                }
                break;
        }
        return false;
        #endregion
    }

    /// <summary>
    /// 棋盘鼠标点击事件
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="message"></param>
    /// <param name="eventData"></param>
    public void OnChessCheckerboardClickEvent(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if(m_GameBase == null)
        {
            return;
        }

        if (m_GameBase.AskPlayerServerSit != m_nSSit ||
            !m_GameBase.PieceMoveComplete)
        {
            return;
        }

        int PiecePostion = (int)message;
        switch (eventtype)
        {
            case EventTriggerType.PointerClick:
                {
                    EChessPiecesType LastPieceType = GetChessPieceTypeByPostion(m_CurSelectPiecePostion);
                    EChessPiecesType PieceType = GetChessPieceTypeByPostion(PiecePostion);
                    if (PieceType == EChessPiecesType.CChessType_None)
                    {
                       if(LastPieceType != EChessPiecesType.CChessType_None)
                        {
                            if(CheckMovePiecePostionState(LastPieceType, PiecePostion))
                            {
                                CRollTextUI.Instance.AddVerticalRollText(26026);
                                return;
                            }
                            int PieceTyped = (m_GameBase.RedCampPlayerSSit != m_nSSit ? -1 : 1) * (int)LastPieceType;
                            m_GameBase.AnswerServerAskQuestionToServerMsg((sbyte)PieceTyped, PiecePostion);
                        }
                        return;
                    }

                    bool SelectPieceState = m_CurSelectPiecePostion != PiecePostion;
                    CustomAudioDataManager.GetInstance().PlayAudio(SelectPieceState ? 1002 : 1003);

                    if (m_CurSelectPiecePostion != PiecePostion && LastPieceType != EChessPiecesType.CChessType_None)
                    {
                        RefreshPlayerPiece(LastPieceType,true,false,true);
                    }
                    RefreshPlayerPiece(PieceType, true, SelectPieceState, true);
                    if (SelectPieceState)
                    {
                        m_GameBase.RefreshPieceTransportableSigns(PieceType, PiecePostion);
                    }
                    m_CurSelectPiecePostion = m_CurSelectPiecePostion != PiecePostion ? PiecePostion : 0;
                }
                break;
            default:
                break;
        }
    }
}

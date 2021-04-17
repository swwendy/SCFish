using UnityEngine;using System.Collections;using System.Collections.Generic;public class SingleTimeBase{    public delegate void TimeCallBack(params object[] args);    public float TimeCount;    public TimeCallBack CallBack;    public bool bFlagDelete;    private object[] ArgsObj;    public SingleTimeBase(float t, TimeCallBack callb, params object[] args)    {        TimeCount = t;        CallBack = callb;        ArgsObj = args;        bFlagDelete = false;    }    private void CallBackFunction()    {        if (CallBack != null)            CallBack(ArgsObj);    }    public void SetDeleteFlag(bool deleteFlag)
    {
        bFlagDelete = deleteFlag;
    }    public virtual bool UpdateTimer(float timedelta)
    {
        if (bFlagDelete)
            return true;

        TimeCount -= timedelta;
        if (TimeCount <= 0f)
        {
            CallBackFunction();
            return true;
        }
        return false;
    }}//倒计时类型(每秒调用一次回调函数)public class CTimerPersecondCall : SingleTimeBase
{
    public CTimerPersecondCall(float t, TimeCallBack callb) 
        : base(t,callb,null)
    {

    }

    public override bool UpdateTimer(float timedelta)
    {
        if (bFlagDelete)
            return true;

        TimeCount -= timedelta;
        CallBack((object)TimeCount);

        if (TimeCount <= 0f)
        {
            return true;
        }
        return false;
    }
}


//循环类型(时间结束后从头开始)
public class CTimerCirculateCall : SingleTimeBase
{
    private float circulatetime;
    public CTimerCirculateCall(float t, TimeCallBack callb)
        : base(t, callb, null)
    {
        circulatetime = t;
    }

    public override bool UpdateTimer(float timedelta)
    {
        if (bFlagDelete)
            return true;

        TimeCount -= timedelta;
        if (TimeCount <= 0f)
        {           
            CallBack((object)TimeCount);
            TimeCount = circulatetime;
        }
        return false;
    }
}public class xTimeManger{    public List<SingleTimeBase> listSingleTimer = new List<SingleTimeBase>();    private static xTimeManger instance = new xTimeManger();    private float secondtime = 0f;    public static xTimeManger Instance    {        get        {            return instance;        }    }    List<SingleTimeBase> cacheSingleTimer__ = new List<SingleTimeBase>();    public void RegisterTimer(SingleTimeBase s)    {        if (listSingleTimer.Count > 500)        {            Debug.LogWarning(listSingleTimer.Count);            cacheSingleTimer__.Add(s);        }        else        {            listSingleTimer.AddRange(cacheSingleTimer__);            listSingleTimer.Add(s);            cacheSingleTimer__.Clear();        }    }    public void Update()    {        //if (xClientNetData.Instance.CurrentSceneType != xEnumDefine.SceneType.FightScene)        //    return;        secondtime += Time.deltaTime;        //一秒钟update一次        if (secondtime < 1.0f)            return;        //Debug.Log("secondtime:" + secondtime);

        for (int i = listSingleTimer.Count - 1; i >= 0; i--)
        {
            if (listSingleTimer[i].UpdateTimer(secondtime))
            {
                listSingleTimer[i] = null;
                listSingleTimer.RemoveAt(i);
            }        }        secondtime = 0f;    }    public void Clear()    {        listSingleTimer.Clear();    }}
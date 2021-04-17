﻿using UnityEngine;
    {
        bFlagDelete = deleteFlag;
    }
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
    }
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
}

        for (int i = listSingleTimer.Count - 1; i >= 0; i--)
        {
            if (listSingleTimer[i].UpdateTimer(secondtime))
            {
                listSingleTimer[i] = null;
                listSingleTimer.RemoveAt(i);
            }
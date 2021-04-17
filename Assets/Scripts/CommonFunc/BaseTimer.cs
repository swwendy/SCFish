using UnityEngine;
using System.Collections;

public class BaseTimer
{
    public delegate void TimerCallBack(Object _message);
    private Object mMessage;
    private TimerCallBack callBackFunc;
    public float mElapsedTime { get; protected set; }
    float mTimeLimit;
    public float timeLimit
    {
        get { return mTimeLimit; }
        set
        {
            mTimeLimit = value;
            if (HasReachedLimit() && callBackFunc != null)
                callBackFunc(mMessage);
        }
    }

    public BaseTimer(float _timeLimit, TimerCallBack _callback = null, Object _message = null)
    {
        timeLimit = _timeLimit;
        callBackFunc = _callback;
        mMessage = _message;
    }

    public void FrameMove(float _elapsedTime)
    {
        mElapsedTime += _elapsedTime;
        if (HasReachedLimit() && callBackFunc != null)
            callBackFunc(mMessage);
    }
    public bool HasReachedLimit(float _elapsedTime)
    {
        return mElapsedTime + _elapsedTime >= timeLimit;
    }
    public bool HasReachedLimit() { return mElapsedTime >= timeLimit; }
}

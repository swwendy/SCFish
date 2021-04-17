using System;

//  1.lazy模式 2.无double check  3.模板单例模式
//  2.使用例子 CSingletonTemplate<ClassName>.getMe ();
public class Singleton<T> where T : class
{
    private static readonly Type tp = typeof(T);
    private static readonly T m_staticMe = (T)Activator.CreateInstance(tp, true);


    private Singleton() { }
    public static T get() { return m_staticMe; }
}
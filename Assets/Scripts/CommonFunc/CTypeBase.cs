using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


public class CMyFiledInfo
{
    public FieldInfo parent;
    public FieldInfo currifno;

    public CMyFiledInfo()
    {
        parent = null;
        currifno = null;
    }
}
public class CTypeBase
{

    private CTypeBase() { }

    private const char splitone = '|';
    private const char splittwo = '^';


    public static Type tpbool = typeof(bool);
    public static Type tpbyte = typeof(byte);
    public static Type tpint = typeof(int);
    public static Type tpfloat = typeof(float);
    public static Type tpstring = typeof(string);
    public static Type tplstint = typeof(List<int>);                //格式如下  value|value
    public static Type tplststr = typeof(List<string>);             //格式如下  value|value
    public static Type tpsetint = typeof(HashSet<int>);              //格式如下  value|value
    public static Type tpmpint = typeof(Dictionary<int, int>);      //格式如下  key^value|key^value

    public static Type tpobj = typeof(object);

    public const BindingFlags BindOnlySefltPublic = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    //递归注意
    private static void CalcAllFields(FieldInfo[] fields, List<CMyFiledInfo> ls, FieldInfo parent)
    {
        foreach (FieldInfo fieldInfo in fields) {
            //不是原生类型 并且不属于系统类型
            if (!fieldInfo.FieldType.IsPrimitive && !fieldInfo.FieldType.FullName.Contains("System")) {
                //Type tp = fieldInfo.GetType();
                FieldInfo[] infos = fieldInfo.FieldType.GetFields(BindOnlySefltPublic);
                CalcAllFields(infos, ls, fieldInfo);
            }
            else {
                CMyFiledInfo myFiledInfo = new CMyFiledInfo();
                myFiledInfo.currifno = fieldInfo;
                myFiledInfo.parent = parent;
                ls.Add(myFiledInfo);
            }
        }
    }

    public static List<CMyFiledInfo> getAllFilelds(object obj)
    {
        List<CMyFiledInfo> ls = new List<CMyFiledInfo>();
        FieldInfo[] info = null;

        Type basetp = obj.GetType().BaseType;
        //先查基类
        while (basetp != tpobj) {
            info = basetp.GetFields(BindOnlySefltPublic);
            CalcAllFields(info, ls, null);
            basetp = basetp.BaseType;
        }

        info = obj.GetType().GetFields(BindOnlySefltPublic);

        CalcAllFields(info, ls, null);

        return ls;
    }

    public static void AutoPushvalue(string[] strcolvalues, ref object dstvalue, List<CMyFiledInfo> ls)
    {
        int i = 0;
        int lstcount = ls.Count - 1;
        foreach (string str in strcolvalues) {
            if (i > lstcount) {
                break;
            }
            CMyFiledInfo myFiledInfo = ls[i];
            if (myFiledInfo.parent == null) {
                pushOneColumn(str, dstvalue, myFiledInfo.currifno, i);
            }
            else {
                object parentdstvalue = myFiledInfo.parent.GetValue(dstvalue);
                pushOneColumn(str, parentdstvalue, myFiledInfo.currifno, i);
            }

            i++;
        }
    }

    private static void pushOneColumn(string strsrc, object dstvalue, FieldInfo fileInfo, int numType)
    {
        //if (strsrc.Length <= 0) {
        //    return;
        //}

        string[] arary1 = null;
        string[] arary2 = null;

        try {
            if (fileInfo.FieldType == tpbool) {
                fileInfo.SetValue(dstvalue, bool.Parse(strsrc));
            }
            else if (fileInfo.FieldType == tpbyte) {
                fileInfo.SetValue(dstvalue, byte.Parse(strsrc));
            }
            else if (fileInfo.FieldType == tpint) {
                fileInfo.SetValue(dstvalue, int.Parse(strsrc));
            }
            else if (fileInfo.FieldType == tpstring) {
                fileInfo.SetValue(dstvalue, strsrc);
            }
            else if (fileInfo.FieldType == tpfloat) {
                fileInfo.SetValue(dstvalue, float.Parse(strsrc));
            }
            else if (fileInfo.FieldType == tplstint) {
                arary1 = strsrc.Split(splitone);
                List<int> dst = new List<int>();
                fileInfo.SetValue(dstvalue, dst);
                for (int i = 0; i < arary1.Length; i++) {
                    if (arary1[i] != "")
                        dst.Add(int.Parse(arary1[i]));
                }
            }
            else if (fileInfo.FieldType == tplststr) {
                arary1 = strsrc.Split(splitone);
                List<string> dst = new List<string>();
                fileInfo.SetValue(dstvalue, dst);
                for (int i = 0; i < arary1.Length; i++) {
                    dst.Add(arary1[i]);
                }
            }
            else if (fileInfo.FieldType == tpsetint) {
                arary1 = strsrc.Split(splitone);
                HashSet<int> dst = new HashSet<int>();
                fileInfo.SetValue(dstvalue, dst);
                for (int i = 0; i < arary1.Length; i++) {
                    if (arary1[i] != "")
                        dst.Add(int.Parse(arary1[i]));
                }
            }
            else if (fileInfo.FieldType == tpmpint) {
                arary1 = strsrc.Split(splitone);
                Dictionary<int, int> mpint = new Dictionary<int, int>();
                fileInfo.SetValue(dstvalue, mpint);
                for (int i = 0; i < arary1.Length; i++) {
                    arary2 = arary1[i].Split(splittwo);
                    if (arary2.Length == 2) {
                        mpint.Add(int.Parse(arary2[0]), int.Parse(arary2[1]));
                    }
                }
            }
        }
        catch {
#if UNITY_EDITOR
            Debug.Log(dstvalue + "类里的第" + numType + "个字段的类型和对应配表中该字段的类型不一致！");
#endif
        }
    }
}

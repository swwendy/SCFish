﻿using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;using XLua;[Hotfix]public class CH_OverList{    int selfmoney_;    int sort_;	// Use this for initialization	void Start () {			}		// Update is called once per frame	void Update () {			}    void InitInfo(int money, int sort)    {        selfmoney_ = money;        sort_ = sort;    }    public void ShowInfo()    {        GameObject obj = GameObject.FindGameObjectWithTag("jiesuan");        Text selfmoney = GetChildTextByName(obj, "zijijinbishu");        selfmoney.text = selfmoney_.ToString();        Text sort = GetChildTextByName(obj, "paiming");        sort.text = sort_.ToString();        Text bossname = GetChildTextByName(obj, "name");        bossname.text = BossInfo.currentboss.name_;        Text bossmoney = GetChildTextByName(obj, "jinbishu");        bossmoney.text = BossInfo.currentboss.money_.ToString();        GameObject sortobj = GetChildNode(obj, "di1");        for (int index = 0; index < 5; index++)        {            GameObject tempobj = GetChildNode(sortobj, index.ToString());            Text name = GetChildTextByName(tempobj, "name");            name.text = "";            Text money = GetChildTextByName(tempobj, "jinbishu");            money.text = "";        }    }         Text GetChildTextByName( GameObject obj, string name )    {        GameObject resultobj = GetChildNode(obj, name);        return resultobj.GetComponent<Text>();    }    GameObject GetChildNode(GameObject obj, string name)    {        Transform transform = obj.transform.Find(name);        return transform.gameObject;    }}
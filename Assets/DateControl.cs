using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DateControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ItemType { _year, _month, _day, _hour, _minute }

    public ItemType _itemtype;

    RectTransform conentRect;

    RectTransform targetRec;

    Vector3 oldDragPos;

    Vector3 newDragPos;

    public AnimationCurve curve_scale;//改变大小曲线
    public AnimationCurve curve_color;//渐变效果曲线

    public Color textc_color;           //字体颜色

    List<Text> textList = new List<Text>();

    Button testBtn;

    float
        itemHeight,             //子项item的高
        contentParentHeight,    //Content爸爸的高
        itemNum,                //子项数量
        itemHeight_min,         //子项最小发生改变位置
        itemHeight_max,         //子项最大发生改变位置
        conentLimit,            //Conent纠正位置
        conentSpacing;          //子项间隔大小

    float deltaX, deltaY;

    [HideInInspector]
    public static int _year, _month, _day, _hour, _minute;

    [HideInInspector]
    int dateItemNum;

    public Color itemColor_hig;


    List<int> HourDataList = new List<int>();
    List<int> MinuteDataList = new List<int>();

    void Awake()
    {
        conentRect = transform.Find("Content").GetComponent<RectTransform>();
        targetRec = transform.parent.Find("HighlightTarget").GetComponent<RectTransform>();
        for(int index = 0; index < 60; ++index)
        {
            if(index < 24)
            {
                HourDataList.Add(index);
            }
            MinuteDataList.Add(index);
        }
    }

    void OnEnable()
    {
        RefreshDataControl();
        InitDateControlData();
        Invoke("ItemList", 0.05f);
    }

    void Start()
    {
        switch (_itemtype)
        {
            case ItemType._year: InstantiateData(15, 2018); break;
            case ItemType._month: InstantiateData(12, 12); break;
            case ItemType._day: InstantiateData(31, 31); break;
            case ItemType._hour: InstantiateData(24, 23); break;
            case ItemType._minute: InstantiateData(60, 59); break;
        }
        InitDateControlData();
        Invoke("ItemList", 0.05f);
    }

    void InitDateControlData()
    {
        if(textList.Count == 0)
        {
            return;
        }
        itemNum = textList.Count;

        contentParentHeight = conentRect.parent.GetComponent<RectTransform>().sizeDelta.y;

        conentSpacing = conentRect.GetComponent<VerticalLayoutGroup>().spacing / 2;

        itemHeight = textList[0].rectTransform.sizeDelta.y + conentSpacing;

        if (itemNum % 2 == 0) conentLimit = (itemHeight + 5) / 2;

        else conentLimit = 0;

        conentRect.anchoredPosition = new Vector2(conentRect.anchoredPosition.x, conentLimit);

        deltaX = textList[0].GetComponent<RectTransform>().sizeDelta.x;
        deltaY = textList[0].GetComponent<RectTransform>().sizeDelta.y;
    }

    void InstantiateData(int itemNum, int dat)
    {
        GameObject go;
        Text testObj = conentRect.Find("Text").GetComponent<Text>();
        for (int i = 0; i < 5; ++i)
        {
            go = Instantiate(testObj.gameObject, conentRect);
            go.GetComponent<Text>().text = i.ToString();
            go.name = i.ToString();
            textList.Add(go.GetComponent<Text>());
        }
        Destroy(conentRect.Find("Text").gameObject);
        RefreshDataControl();
        ShowItem(true);
    }

    void RefreshDataControl()
    {
        int now = 0;
        int index = 0;
        List<int> TimeDataList = null;
        if (_itemtype == ItemType._hour)
        {
            now = System.DateTime.Now.Hour - 2;
            if (System.DateTime.Now.Minute + 3 > 59)
            {
                now += 1;
            }
            if (now < 0)
            {
                now += 24;
            }
            TimeDataList = HourDataList;
            index = HourDataList.FindIndex(value => value == now);
        }

        if (_itemtype == ItemType._minute)
        {
            now = System.DateTime.Now.Minute + 1;
            if (now > 59)
            {
                now -= 60;
            }
            TimeDataList = MinuteDataList;
            index = MinuteDataList.FindIndex(value => value == now);
        }

        for (int i = 0; i < textList.Count; ++i)
        {
            if(index >= TimeDataList.Count)
            {
                index = 0;
            }
            textList[i].text = string.Format("{0:00}", TimeDataList[index]);
            textList[i].transform.SetSiblingIndex(i);
            ++index;
        }
    }

    void ShowItem(bool isIncreaseOrdecrease)
    {
        itemHeight_min = -itemHeight;

        if (_itemtype == ItemType._day) itemHeight_max = -itemHeight * itemNum - 95;
        else if (_itemtype == ItemType._hour) itemHeight_max = -itemHeight * itemNum + 60;
        else if (_itemtype == ItemType._minute) itemHeight_max = -itemHeight * itemNum + 60;
        else itemHeight_max = -itemHeight * itemNum;

        int unit = 5;
        int max = 0;
        if (_itemtype == ItemType._hour)
            max = 24;
        if (_itemtype == ItemType._minute)
            max = 60;

        if (isIncreaseOrdecrease)
        {
            foreach (Text rectItem in textList)
            {
                if (rectItem.GetComponent<RectTransform>().anchoredPosition.y > itemHeight_min)
                {
                    //print("+");
                    int lastvalue = int.Parse(rectItem.text);

                    if (lastvalue >= max)
                        lastvalue = 0;
                    else
                        lastvalue += unit;

                    if (lastvalue >= max)
                        lastvalue -= max;

                    rectItem.text = string.Format("{0:00}", lastvalue);

                    rectItem.transform.SetSiblingIndex((int)itemNum);
                }
            }
            //print(itemHeight_min);
        }
        else
        {
            foreach (Text rectItem in textList)
            {
                if (rectItem.GetComponent<RectTransform>().anchoredPosition.y < itemHeight_max)
                {
                    //print("-");

                    int lastvalue = int.Parse(rectItem.text);

                    if (lastvalue < max && lastvalue >= unit)
                        lastvalue -= unit;
                    else if (lastvalue < unit)
                        lastvalue = max - (unit - lastvalue);

                    rectItem.text = string.Format("{0:00}", lastvalue);

                    rectItem.transform.SetSiblingIndex(0);
                }
            }
            //print(itemHeight_max);
        }
    }

    void ItemList()
    {
        foreach (Text item in textList)
        {
            float indexA = Mathf.Abs(item.GetComponent<RectTransform>().position.y - targetRec.position.y);
            float indexSc_scale = Mathf.Abs(curve_scale.Evaluate(indexA / contentParentHeight));
            float indexSc_color = Mathf.Abs(curve_color.Evaluate(indexA / contentParentHeight));

            if (indexA < 15.0f)
            {
                item.color = itemColor_hig;
                switch (_itemtype)
                {
                    case ItemType._year : _year = int.Parse(item.text); break;
                    case ItemType._month : _month = int.Parse(item.text); break;
                    case ItemType._day : _day = int.Parse(item.text); break;
                    case ItemType._hour : _hour = int.Parse(item.text); break;
                    case ItemType._minute : _minute = int.Parse(item.text); break;
                }
            }
            else item.color = new Color(textc_color.r, textc_color.g, textc_color.b, 1.0f - indexSc_color);

            item.GetComponent<RectTransform>().localScale = new Vector3(1 - indexSc_scale, 1 - indexSc_scale * 3, 1 - indexSc_scale);
        }

        Text datetx = transform.parent.parent.Find("Text_D").gameObject.GetComponent<Text>();
        long currentseconds = ConvertDataTimeToLong(System.DateTime.Now);
        long targetsecondes = ConvertDataTimeToLong(System.DateTime.Now.Date.AddSeconds(_hour * 3600 + _minute * 60));
        if (currentseconds > targetsecondes)
            datetx.text = "明天";
        else
            datetx.text = "今天";
    }

    public static string GetDateInfo()
    {
        //return _year + "-" + _month + "-" + _day;
        return _hour + ":" + _minute;
    }

    long ConvertDataTimeToLong(DateTime dt)    {        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));        TimeSpan toNow = dt.Subtract(dtStart);        return (long)toNow.TotalSeconds;
    }

    void UpdateEx()
    {
        if (conentRect.anchoredPosition.y > conentLimit)
        {
            ShowItem(true);
            conentRect.anchoredPosition = new Vector2(conentRect.anchoredPosition.x, conentRect.anchoredPosition.y - itemHeight);
        }
        if (conentRect.anchoredPosition.y < conentLimit)
        {
            ShowItem(false);
            conentRect.anchoredPosition = new Vector2(conentRect.anchoredPosition.x, conentRect.anchoredPosition.y + itemHeight);
        }
    }

    void SetDraggedPosition(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(conentRect, eventData.position, eventData.pressEventCamera, out newDragPos))
        {
            newDragPos = eventData.position;
            Debug.Log("new : " + newDragPos.y + " old:" + oldDragPos.y);
            if (Mathf.Abs(newDragPos.y - oldDragPos.y) >= itemHeight / 4)
            {
                if (newDragPos.y > oldDragPos.y)
                {
                    conentRect.anchoredPosition = new Vector2(conentRect.anchoredPosition.x, conentRect.anchoredPosition.y + itemHeight);
                    oldDragPos += new Vector3(0, itemHeight / 4, 0);
                    ItemList();
                }
                else
                {
                    conentRect.anchoredPosition = new Vector2(conentRect.anchoredPosition.x, conentRect.anchoredPosition.y - itemHeight);
                    oldDragPos -= new Vector3(0, itemHeight / 4, 0);
                    ItemList();
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        oldDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
        UpdateEx();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
        UpdateEx();
    }
}
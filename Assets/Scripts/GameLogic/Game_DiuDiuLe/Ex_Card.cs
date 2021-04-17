using UnityEngine;
using UnityEngine.UI;
using XLua;[Hotfix]
public class Ex_Card
{

    public byte cardCode = (byte)0;//牌 16
    public int cardNum = 0;//牌值 10
    public int cardColor = 0;//花色
    GameObject di;//底图背景
    GameObject zi;//数字
    GameObject hua;//花色
    public static int _width = 86;
    public static int _height = 113;
    public bool isSelect = false;//是否弹起(是否选取) true 选取
    GameObject card;
    GameObject go_huangbian;

    public void InitUI(GameObject _card, byte _cardCode)
    {
        card = _card;
        di = card;
        cardCode = _cardCode;
        zi = card.transform.Find("CardShuZi").gameObject;
        hua = card.transform.Find("CardHuaSe").gameObject;

        SetCardType(cardCode);
    }

    public void SetCardType(byte num, bool isFan = false)
    {
        cardCode = num;
        cardNum = GameCommon.GetCardValue(num);
        cardColor = GameCommon.GetCardColor(num);
        if (cardCode == 0)
        {
            di.GetComponent<Image>().sprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("ex_cardbei");
            zi.SetActive(false);
            hua.SetActive(false);
        }
        else
        {
            zi.SetActive(true);
            hua.SetActive(true);
            di.GetComponent<Image>().sprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("ex_carddi");
            if (cardColor == 4)
            {
                if (cardNum == -1)
                {
                    hua.GetComponent<Image>().sprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("ex_small");
                    zi.SetActive(false);
                }
                if (cardNum == 0)
                {
                    hua.GetComponent<Image>().sprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("ex_big");
                    zi.SetActive(false);
                }
                if (isFan)
                {
                    hua.transform.eulerAngles = new Vector3(0f,0f,180f);
                }
                return;
            }

            hua.GetComponent<Image>().sprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("ex_color" + cardColor);
            if (cardColor == 3 || cardColor == 0)//黑
            {
                zi.GetComponent<Image>().sprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("ex_b" + cardNum);
            }
            if (cardColor == 2 || cardColor == 1)//红
            {
                zi.GetComponent<Image>().sprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("ex_r" + cardNum);
            }
        }
    }

    /***
     *自己手牌状态
     */
    public void setState(bool _isSelect)
    {
        isSelect = _isSelect;
        if (isSelect)
        {
            card.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (go_huangbian == null)
            {
                go_huangbian = new GameObject();
                Image img_huangbian = go_huangbian.GetComponent<Image>();
                if (img_huangbian == null)
                {
                    img_huangbian = go_huangbian.AddComponent<Image>();
                }
                Sprite sprite_huangbian;// = new Sprite();
                sprite_huangbian = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("Main_BG_09");
                img_huangbian.sprite = sprite_huangbian;
                img_huangbian.SetNativeSize();
            }
            go_huangbian.transform.SetParent(card.transform,false);
        }
        else
        {
            if (go_huangbian != null)
            {
                Object.Destroy(go_huangbian);
            }
            card.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void SetAlpha(float a)
    {
        Color c = card.GetComponent<Image>().color;
        card.GetComponent<Image>().color = new Color(c.r, c.g, c.b, a);
        Color c_zi = zi.GetComponent<Image>().color;
        zi.GetComponent<Image>().color = new Color(c_zi.r, c_zi.g, c_zi.b, a);
        Color c_hua = hua.GetComponent<Image>().color;
        hua.GetComponent<Image>().color = new Color(c_hua.r, c_hua.g, c_hua.b, a);
    }
}

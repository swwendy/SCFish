﻿using UnityEngine;
public class Ex_Tips
{
    private string str;
    public delegate void OkFunc();
    public Ex_Tips(GameObject _tipsUI, string _str, OkFunc _okFunc = null, CancleFunc _cancleFunc = null)
    {
        tipsUI = _tipsUI;
        XPointEvent.AutoAddListener(go_cancelBtn, TapFunc, go_cancelBtn);
        go_text.GetComponent<Text>().text = str;
        if (cancleFunc == null)
        {
            go_cancelBtn.SetActive(false);
        }
        go_showGroup.transform.localScale = new Vector3(0f, 0f, 1f);
        DOTween.To(() => go_showGroup.transform.localScale, r => go_showGroup.transform.localScale = r, new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.InOutBack);
    }

            {
                AudioManager.Instance.PlaySound("diu.resource", "ex_sBtn");
            }
                    Object.Destroy(tipsUI);
                    if (okFunc != null)
                    {
                        okFunc();
                    }
                });
                Tweener t = DOTween.To(() => go_showGroup.transform.localScale, r => go_showGroup.transform.localScale = r, new Vector3(0f, 0f, 1f), 0.3f).SetEase(Ease.InOutBack);
                    Object.Destroy(tipsUI);
                    if (cancleFunc != null)
                    {
                        cancleFunc();
                    }
                });
}
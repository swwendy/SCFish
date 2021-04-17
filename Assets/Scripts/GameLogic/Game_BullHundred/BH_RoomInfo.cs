﻿using DG.Tweening;
            tfm.Find("HeadMask/ImageHead").GetComponent<Image>().sprite= GameMain.hall_.GetIcon(url, userid, (int)faceId);
            {
                go = tfm.gameObject;

                if (index < CardMax && count > 0)
                {
                    go.SetActive(true);
                else if (index <= CardMax)
                    go.SetActive(false); 
                else
                    GameObject.Destroy(go);
                index++;
            }

        for (int i = 0; i < CardMax; i++)
                    go = m_PokerTfm[j].GetChild(i).gameObject;
                }
                    continue;
                }
                go.GetComponent<Image>().sprite = spriteBack;
                target = go.transform.localPosition;
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_PokerTfm[j], false);
                tempObj.transform.position = m_PokerSource.position;
                tempObj.transform.DOLocalMove(target, flyTime);

                yield return new WaitForSecondsRealtime(flyTime);

                go.SetActive(true);
                tempObj.SetActive(false);

                CustomAudioDataManager.GetInstance().PlayAudio(1003);

                if (m_vecPoker[j].Count > i)
                {
                    go.transform.DOScaleX(0.1f, scaleTime);

                    yield return new WaitForSecondsRealtime(scaleTime);

                    sprite = m_BHGameBase.BullHundredAssetBundle.LoadAsset<Sprite>(GameCommon.GetPokerMat(m_vecPoker[j][i]));
                    go.GetComponent<Image>().sprite = sprite;
                    go.transform.DOScaleX(1f, scaleTime);

                    yield return new WaitForSecondsRealtime(scaleTime);
                }
            }

        GameObject.DestroyImmediate(tempObj);
        m_PokerSource.gameObject.SetActive(false);
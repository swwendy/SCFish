﻿using System.Collections.Generic;
{
    eFT_None,
    eFT_Normal,
    eFT_Special,

    eFT_Num
}

[LuaCallCSharp]

        m_RoomData.Clear();

        byte levelNum = msg.ReadByte();
            //float.TryParse(strList[i][j++], out gamedata.m_fExplosinRange);
            j++;
            //int.TryParse(strList[i][j++], out gamedata.m_nBulletCost);
            j++;
            j++;
            //gamedata.m_szIcon = strList[i][j++];
            j++;
                string[] offsets = str.Split('@');
                string[] point;
                foreach (string oft in offsets)
                {
                    point = oft.Split('|');
                    Debug.Assert(point.Length == 3, "offset point coordinate wrong(not 3)!!");
                    Vector3 pos = new Vector3();
                    float.TryParse(point[0], out pos.x);
                    float.TryParse(point[1], out pos.y);
                    float.TryParse(point[2], out pos.z);
                    gamedata.m_Offsets.Add(pos);
                }
            }
            str = strList[i][j++];
            if (!string.IsNullOrEmpty(str))
                string[] speedChange = str.Split('@');
                string[] info;
                foreach (string sc in speedChange)
                {
                    info = sc.Split('|');
                    Debug.Assert(info.Length == 3, "point change speed info is wrong(not 3)!!");
                    byte pointIndex, endIndex;
                    float speed, time;
                    byte.TryParse(info[0], out pointIndex);
                    float.TryParse(info[1], out speed);
                    if (speed == 0f)//pause
                    {
                        gamedata.m_ChangePoints[pointIndex] = -time;
                    }
                        byte.TryParse(info[2], out endIndex);
                        gamedata.m_ChangePoints[endIndex] = 1f;//change back
                    }
            }

            string[] groupspeed = strList[i][j++].Split('@');
            string[] speedStr;
            foreach (string speed in groupspeed)
            {
                speedStr = speed.Split('|');
                Debug.Assert(speedStr.Length == 2, "group speed data is wrong(not 2)!!");
                gamedata.m_GroupSpeed[byte.Parse(speedStr[0])] = float.Parse(speedStr[1]);
            }

            //float.TryParse(strList[i][j++], out gamedata.m_fHitRate);
            j++;
                byte.TryParse(strs[0], out gamedata.m_nTalkInterval);
    }
}
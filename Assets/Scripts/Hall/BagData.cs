using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;

public enum ItemUseType
{
    Type1 = 1,
    Type2,
    Type3,
    Type4,
    Type5,
}

public class Item
{
    public ushort itemid;
    public ushort itemNumber;
}

public class BagItemData
{
    public uint itemid;
    public string itemName;
    public string itemIcon;
    public string itemInfo;
    public uint repeatNumber;
    public bool isgiven;
    public ItemUseType usetype;
    public uint itemScriptid;
    public uint itemWorkTime;
    public string workEndTime;
}

public class BagDataManager
{
    static BagDataManager instance_;
    public Dictionary<uint, BagItemData> bagitemsdata_;
    public Dictionary<ushort, Item> currentItems_;
    public bool isUseItem;
    public uint scriptid;

    public static BagDataManager GetBagDataInstance()
    {
        if (instance_ == null)
            instance_ = new BagDataManager();

        return instance_;
    }

    BagDataManager()
    {
        bagitemsdata_ = new Dictionary<uint, BagItemData>();
        currentItems_ = new Dictionary<ushort, Item>();

        isUseItem = false;

        ReadItemConfig("ItemCsv");
        InitBagBackMsg();
    }

    void InitBagBackMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SENDPLAYERPACKETINFO, BackBagData);                     //背包数据
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_UPDATEPLAYERPACKETINFO, BackBagUpdateData);                     //背包数据
    }

    bool BackBagUpdateData(uint _msgType, UMessage msg)
    {
        byte state = msg.ReadByte();
        ushort itemid = msg.ReaduShort();
        byte changeNumber = msg.ReadByte();
        ushort itemNumber = msg.ReaduShort();

        Debug.Log(itemid.ToString() + "  " + changeNumber.ToString() + " " + itemNumber.ToString());

        if(currentItems_.ContainsKey(itemid))
        {
            if (itemNumber == 0)
            {
                currentItems_.Remove(itemid);
                return true;
            }

            currentItems_[itemid].itemNumber = itemNumber;
        }
        else
        {
            Item item = new Item();
            item.itemid = itemid;
            item.itemNumber = itemNumber;
            currentItems_.Add(itemid, item);
        }

        Bag.GetBagInstance().LoadBagItemResource();

        return true;
    }

    bool BackBagData(uint _msgType, UMessage msg)
    {
        currentItems_.Clear();

        byte length = msg.ReadByte();

        Debug.Log("bag data accessed length:" + length.ToString());

        for (int index = 0; index < length; index++)
        {
            Item item = new Item();

            item.itemid = msg.ReaduShort();
            item.itemNumber = msg.ReaduShort();

            currentItems_.Add(item.itemid, item);
        }

        return true;
    }

    void ReadItemConfig(string csvname)
    {
        bagitemsdata_.Clear();

        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, csvname, out strList);

        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)
        {
            BagItemData data = new BagItemData();

            uint.TryParse(strList[i][0], out data.itemid);
            data.itemName = strList[i][1];
            data.itemIcon = strList[i][2];
            data.itemInfo = strList[i][3];
            uint.TryParse(strList[i][4], out data.repeatNumber);
            int isgiven = 0;
            int.TryParse(strList[i][5], out isgiven);
            data.isgiven = isgiven == 1;
            int usetype = 0;
            int.TryParse(strList[i][6], out usetype);
            data.usetype = (ItemUseType)usetype;
            uint.TryParse(strList[i][7], out data.itemScriptid);
            uint.TryParse(strList[i][8], out data.itemWorkTime);
            data.workEndTime = strList[i][9];

            bagitemsdata_.Add(data.itemid, data);
        }
    }

    public BagItemData GetItemData(uint itemid)
    {
        if(!bagitemsdata_.ContainsKey(itemid))
        {
            Debug.Log("背包数据id错误！！！" + itemid);
            return new BagItemData();
        }
        return bagitemsdata_[itemid];
    }
}

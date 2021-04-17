using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

public static class EnumExtend
{
    public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo _fieldInfo)
    {
        if (_fieldInfo != null) {
            return (DescriptionAttribute[])_fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        }
        return null;
    }

    public static string GetDescription(this Enum _enumValue)
    {
        string tDesc = string.Empty;
        FieldInfo tFieldInfo = _enumValue.GetType().GetField(_enumValue.ToString());
        DescriptionAttribute[] tAttributes = tFieldInfo.GetDescriptAttr();
        if (tAttributes != null && tAttributes.Length > 0)
            tDesc = tAttributes[0].Description;
        else
            tDesc = _enumValue.ToString();
        return tDesc;
    }

    public static List<string> GetDescriptionList<T>()
    {
        List<string> descList = new List<string>();
        Type tType = typeof(T);
        if (tType.IsEnum) {
            var tFields = tType.GetFields();
            // Enum的第一个Field是System.Int32类型的Value__变量，并不是枚举之一
            for (int i = 1; i < tFields.Length; i++) {
                FieldInfo tFieldInfo = tFields[i];
                DescriptionAttribute[] tAttributes = tFieldInfo.GetDescriptAttr();
                if (tAttributes != null && tAttributes.Length > 0)
                    descList.Add(tAttributes[0].Description);
                else
                    descList.Add(tFieldInfo.Name);
            }
        }
        return descList;
    }

public static T GetEnumName<T>(string _description)
    {
        Type tType = typeof(T);
        var tFields = tType.GetFields();
        for (int i = 0; i < tFields.Length; i++) {
            var currField = tFields[i];
                DescriptionAttribute[] tCurDesc = currField.GetDescriptAttr();
            if (tCurDesc != null && tCurDesc.Length > 0) {
                if (tCurDesc[0].Description == _description)
                    return (T)currField.GetValue(null);
            }
            else {
                if (currField.Name == _description)
                    return (T)currField.GetValue(null);
            }
        }
        throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", _description), "Description");
    }

    public static bool IsFlagEnum(Type _type)
    {
        if (_type.IsEnum) {
            var attrL = (FlagsAttribute[])_type.GetCustomAttributes(typeof(FlagsAttribute), false);
            return attrL != null && attrL.Length > 0;
        }
        return false;
    }

    public static List<T> GetEnumList<T>()
    {
        Type tType = typeof(T);
        List<T> valueList = new List<T>();
        if (tType.IsEnum) {
            var iter = Enum.GetValues(tType).GetEnumerator();
            while (iter.MoveNext())
                valueList.Add((T)iter.Current);
        }
        return valueList;
    }

    public static List<int> GetEnumIntList<T>()
    {
        Type tType = typeof(T);
        List<int> intList = new List<int>();
        if (tType.IsEnum) {
            var iter = Enum.GetValues(tType).GetEnumerator();
            while (iter.MoveNext())
                intList.Add((int)iter.Current);
        }
        return intList;
    }

    public static int GetFlagSum(this Enum _enum)
    {
        Type tType = _enum.GetType();
        int sum = 0;
        if (IsFlagEnum(tType)) {
            var iter = Enum.GetValues(tType).GetEnumerator();
            while (iter.MoveNext())
                sum += (int)iter.Current;
        }

        return sum;
    }
}

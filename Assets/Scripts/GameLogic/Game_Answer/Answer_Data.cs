using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;
using XLua;

/// <summary>
/// 问题数据
/// </summary>
[LuaCallCSharp]
public class Answer_QuestionData
{
    /// <summary>
    /// 问题ID
    /// </summary>
    public uint m_nID;
    /// <summary>
    /// 问题描述
    /// </summary>
    public string m_sQuestion;
    /// <summary>
    /// 问题答案选项
    /// </summary>
    public string [] m_sOption;
    /// <summary>
    /// 问题解析
    /// </summary>
    public string m_sAnswer;

    public Answer_QuestionData()
    {
        m_nID = 0;
        m_sQuestion = string.Empty;
        m_sOption = new string[4];
        for(int index = 0; index < m_sOption.Rank; ++index)
        {
            m_sOption[index] = string.Empty;
        }
        m_sAnswer = string.Empty;
    }
}

/// <summary>
/// 答题赛数据
/// </summary>
[Hotfix]
public class Answer_Data
{
    static Answer_Data m_AnswerInstance = null;

    Dictionary<uint, Answer_QuestionData> m_AnswerDataDictionary = null;
    Answer_Data()
    {
        m_AnswerInstance = null;
        m_AnswerDataDictionary = new Dictionary<uint, Answer_QuestionData>();
        ReadAnswerData();
    }

    public static Answer_Data GetInstance()
    {
        if(m_AnswerInstance == null)
        {
            m_AnswerInstance = new Answer_Data();
        }
        return m_AnswerInstance;
    }

    /// <summary>
    /// 解析答题数据
    /// </summary>
    /// <param name="msg"></param>
    public void ReadData(UMessage msg)
    {
    }

    /// <summary>
    /// 读取答题题库数据
    /// </summary>
    private void ReadAnswerData()
    {
        List<string []> strDataList = null;
        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "Question.txt", out strDataList);

        if(strDataList.Count == 0)
        {
            return;
        }

        Answer_QuestionData questionData = null;
        for (int rawIndex = 2; rawIndex < strDataList.Count; ++rawIndex)
        {
            questionData = new Answer_QuestionData();
            uint.TryParse(strDataList[rawIndex][0],out questionData.m_nID);
            questionData.m_sQuestion = strDataList[rawIndex][4].Replace("\\n", "\n");
            for (int optionIndex = 0; optionIndex < 4; ++ optionIndex)
            {
                questionData.m_sOption[optionIndex] = strDataList[rawIndex][5 + optionIndex];
            }
            questionData.m_sAnswer = strDataList[rawIndex][9];

            m_AnswerDataDictionary.Add(questionData.m_nID, questionData);
        }
    }

    /// <summary>
    /// 根据ID获得答题数据
    /// </summary>
    /// <param name="answerID">ID</param>
    /// <param name="answerData">答题数据</param>
    /// <returns>true(找到当前问题)false(当前问题非法)</returns>
    public bool GetAnswerData(uint answerID,ref Answer_QuestionData answerData)
    {
        answerData = null;
        if (m_AnswerDataDictionary.ContainsKey(answerID))
        {
            answerData = m_AnswerDataDictionary[answerID];
        }
        return answerData != null;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class DataSave : MonoBehaviour
{
    //public UnityEngine.UI.Text text;
    public int exp;
    public int[] guitar = new int[9];
    public int level;
    public int[] songCombo = new int[2];
    public int[] songRank = new int[2];
    public int[] songScore = new int[2];
    public int ticket;
    public string exitTime;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        else if(Input.GetKeyDown(KeyCode.G))
        {
            Get();
        }
    }

    public static void Write(List<UserData> saveList, string path)
    {
        XmlDocument document = new XmlDocument();
        XmlElement saveListElement = document.CreateElement("saveList");
        document.AppendChild(saveListElement);

        foreach (UserData Save in saveList)
        {
            XmlElement saveElement = document.CreateElement("Save");

            // EXP 저장
            saveElement.SetAttribute("exp", Save.exp.ToString());

            // GUITAR 정보 저장
            for (int i = 0; i < 9; i ++)
                saveElement.SetAttribute("guitar_" + i, Save.guitar[i].ToString());
            
            // 레벨 저장
            saveElement.SetAttribute("level", Save.level.ToString());

            // 노래 최고 콤보 저장
            for (int i = 0; i < 2; i++)
                saveElement.SetAttribute("songCombo_" + i, Save.songCombo[i].ToString());

            // 노래 최고 랭크 저장
            for (int i = 0; i < 2; i++)
                saveElement.SetAttribute("songRank_" + i, Save.songRank[i].ToString());

            // 노래 최고 점수 저장
            for (int i = 0; i < 2; i++)
                saveElement.SetAttribute("songScore_" + i, Save.songScore[i].ToString());

            // 티겟 개수 저장
            saveElement.SetAttribute("ticket", Save.ticket.ToString());

            // 마지막 종료 시간
            saveElement.SetAttribute("exitTime", Save.exitTime.ToString());


            // 저장
            saveListElement.AppendChild(saveElement);
        }

        document.Save(path);
    }

    public static List<UserData> Read(string path)
    {
        XmlDocument document = new XmlDocument();
        document.Load(path);

        XmlElement saveListElement = document["saveList"];

        List<UserData> saveList = new List<UserData>();

        foreach(XmlElement saveElemnet in saveListElement.ChildNodes)
        {
            UserData info = new UserData();

            info.exp = System.Convert.ToInt32(saveElemnet.GetAttribute("exp"));

            for (int i = 0; i < 9; i ++)
                info.guitar[i] = System.Convert.ToInt32(saveElemnet.GetAttribute("guitar_" + i));
            
            info.level = System.Convert.ToInt32(saveElemnet.GetAttribute("level"));

            for (int i = 0; i < 2; i++)
                info.songCombo[i] = System.Convert.ToInt32(saveElemnet.GetAttribute("songCombo_" + i));

            for (int i = 0; i < 2; i++)
                info.songRank[i] = System.Convert.ToInt32(saveElemnet.GetAttribute("songRank_" + i));

            for (int i = 0; i < 2; i++)
                info.songScore[i] = System.Convert.ToInt32(saveElemnet.GetAttribute("songScore_" + i));

            info.ticket = System.Convert.ToInt32(saveElemnet.GetAttribute("ticket"));

            info.exitTime = System.Convert.ToString(saveElemnet.GetAttribute("exitTime"));

            saveList.Add(info);
        }

        return saveList;
    }

    public void Save()
    {
        List<UserData> saveList = new List<UserData>();

        UserData info = new UserData();
        info.exp = this.exp;
        info.guitar = this.guitar;
        info.level = this.level;
        info.songCombo = this.songCombo;
        info.songRank = this.songRank;
        info.songScore = this.songScore;
        info.ticket = this.ticket;
        info.exitTime = this.exitTime;

        saveList.Add(info);
        Write(saveList, pathForDocuments("saveDataList.xml"));
    }

    public void Get()
    {
        List<UserData> saveInfoList = Read(pathForDocuments("saveDataList.xml"));

        for (int i =0; i < saveInfoList.Count; i ++)
        {
            UserData info = saveInfoList[i];
            this.exp = info.exp;
            this.guitar = info.guitar;
            this.level = info.level;
            this.songCombo = info.songCombo;
            this.songRank = info.songRank;
            this.songScore = info.songScore;
            this.ticket = info.ticket;
            this.exitTime = info.exitTime;
        }
    }

    string pathForDocuments(string fileName)
    {
        string path = Application.streamingAssetsPath + "/" +  fileName;
        return path;

        //text.text = "YES";
//#if UNITY_EDITOR
//        string path = Application.dataPath + "/StreamingAssets/" +  fileName;
//        //text.text = path;
//        return path;
//#elif UNITY_ANDROID
//        string path = "jar:file://" + Application.dataPath + "/StreamingAssets/" + fileName;
//        //text.text = path;
//        return path;
//#endif
    }
}

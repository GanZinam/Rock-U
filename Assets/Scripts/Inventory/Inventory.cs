using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
//using UnityEditor;

public class Inventory : MonoBehaviour
{
    public GameObject sceneChange;
    public GameObject sceneOpen;

    public Image myUseGuitar;

    [SerializeField] DataSave userData;
    public Text levelText;
    public Text collectText;

    public Image[] Guitar;
    public Button[] GuitarBT;

    public Sprite[] change_guitar;
    public Image[] effect;
    
    public Sprite[] select_bg;  //0.초록배경 1.초록추가효과 2. 장착완료 / 3.노란배경 4.노란추가효과 5. 장착 / 6.초록 스킬배경 7.초록 스킬 / 8.노란 스킬배경 9.노란 스킬

    public Image bg;
    public Image Additional_effect;
    public Image Mounting;
    public Image skill_bg;
    public Image skill;

    public Image char_guitar;

    int Guitar_num = 0;
    int Guitar_select = 0;
    bool skill_push = false;
    bool skill_on = false;

    int moveHow = 0;

    int[] myGuitar = new int[5];

    IEnumerator sOpen()
    {
        yield return new WaitForSeconds(0.5f);
        sceneOpen.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(sOpen());
        // 데이터 초기화 할떄 =======================================
        // for (int i = 0; i < 5; i++)
        // {
        //     PlayerPrefs.SetInt("guitar_" + i, 0);
        // }
        // PlayerPrefs.SetInt("guitar_" + 0, 2);

        for (int i = 0; i < 5; i ++)
        {
            myGuitar[i] = PlayerPrefs.GetInt("guitar_" + i);
        }
        levelText.text = "" + PlayerPrefs.GetInt("level").ToString("D2");

        int cou = 0;
        for (int i = 0; i < 5; i ++)
        {
            if (PlayerPrefs.GetInt("guitar_" + i) > 0) cou++;
        }
        collectText.text = cou + " / " + "5";

        myUseGuitar.sprite = Guitar[Guitar_select].sprite;

        //text.text = Application.streamingAssetsPath + "/saveDataList.xml";

        //readAllMyGuitar("Data/UserData/Guitar");
        //userData.Get();
        //InstallBt();
        //pushGitar(Guitar_num);

        for (int i = 0; i < 5; i++)
        {
            if (myGuitar[i] == 2)
            {
                Guitar_select = i;
                Guitar_num = i;
                pushGitar(i);
                InstallBt();
            }
            else if (myGuitar[i] == 0)
            {
                Guitar[i].color = Color.red;
                GuitarBT[i].enabled = false;
            }
            //if (myGuitar[i] == 2)
            //{
            //    Guitar_select = i;
            //    Guitar_num = i;
            //    pushGitar(i);
            //    InstallBt();
            //}
            //else if(myGuitar[i] == 0)
            //{

            //}
        }
    }

    //void readAllMyGuitar(string fileName)
    //{
    //    FileStream f = new FileStream(pathForDocuments("Resources/Data/UserData/Guitar.txt"), FileMode.Open, FileAccess.Read);
    //    StreamReader writer = new StreamReader(f, System.Text.Encoding.Unicode);

    //    string str = writer.ReadToEnd();

    //    char sp = ',';
    //    string[] spString = str.Split(sp);

    //    for (int i = 0; i < spString.Length - 1; i++)
    //    {
    //        int numInterg = System.Convert.ToInt32(spString[i]);
    //        myGuitar[i] = numInterg;
    //    }
    //    writer.Close();
    //    f.Close();


    //    //TextAsset textAsset = Resources.Load(fileName) as TextAsset;
    //    //string str = textAsset.text;

    //    //char sp = ',';
    //    //string[] spString = str.Split(sp);

    //    //for (int i = 0; i < spString.Length - 1; i++)
    //    //{
    //    //    int numInterg = System.Convert.ToInt32(spString[i]);
    //    //    myGuitar[i] = numInterg;
    //    //}
    //}

    //메인 가는 함수
    public void GoMain()
    {
        //SceneManager.LoadScene("EqipScene", LoadSceneMode.Additive);
        sceneChange.SetActive(true);
        StartCoroutine(changeSce());
    }


    IEnumerator changeSce()
    {
        yield return new WaitForSeconds(0.5f);
        Application.LoadLevel("MainScene");
    }

    //장착 버튼 클릭함수
    public void InstallBt()
    {
        //Debug.Log(Guitar_num);
        for (int i = 0; i < 5; i++)
        {
            if (Guitar_num == i)
            {
                myGuitar[Guitar_select] = 1;
                Guitar_select = Guitar_num;
                myGuitar[Guitar_select] = 2;

                effect[i].gameObject.SetActive(true);
                myUseGuitar.sprite = Guitar[Guitar_select].sprite;
            }
            else
            {                
                effect[i].gameObject.SetActive(false);
            }
            if (Guitar_num == Guitar_select)
            {
                bg.sprite = select_bg[0];
                Additional_effect.sprite = select_bg[1];
                Mounting.sprite = select_bg[2];
                skill.sprite = select_bg[7];
                skill_bg.sprite = select_bg[6];
            }
            else
            {
                bg.sprite = select_bg[3];
                Additional_effect.sprite = select_bg[4];
                Mounting.sprite = select_bg[5];
                skill.sprite = select_bg[9];
                skill_bg.sprite = select_bg[8];
            }
        }

        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt("guitar_" + i, myGuitar[i]);
            //myGuitar[i] = PlayerPrefs.GetInt("guitar_" + i);
        }

        //FileStream f = new FileStream(pathForDocuments("Resources/Data/UserData/Guitar.txt"), FileMode.Create, FileAccess.Write);
        //StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Unicode);
        //
        //for (int i = 0; i < 9; i ++)
        //{
        //    writer.WriteLine(myGuitar[i] + ",");
        //}
        //writer.Close();
        //f.Close();

        //AssetDatabase.Refresh();
    }

    //기타 클릭시 눌린 기타 보여주는함수
    public void pushGitar(int gitar_n)
    {
        //Debug.Log(gitar_n);
        for(int i = 0; i< 5; i++)
        {
            if (gitar_n == i)
            {
                Guitar_num = i;
                char_guitar.sprite = change_guitar[i];
                if (Guitar_num == Guitar_select)
                {
                    bg.sprite = select_bg[0];
                    Additional_effect.sprite = select_bg[1];
                    Mounting.sprite = select_bg[2];
                    skill.sprite = select_bg[7];
                    skill_bg.sprite = select_bg[6];
                }
                else
                {
                    bg.sprite = select_bg[3];
                    Additional_effect.sprite = select_bg[4];
                    Mounting.sprite = select_bg[5];
                    skill.sprite = select_bg[9];
                    skill_bg.sprite = select_bg[8];
                }
            }            
        }
    }
    
    //스킬 버튼 클릭시 들어가는 함수
    public void pushSkill()
    {
       skill_push = true;
       if(skill_on)
       {
           skill_on = false;
       }
       else
       {
           skill_on = true;
       }
       Debug.Log(skill_bg.transform.position.x);
       // for (int i=0;i<2300 ;i++ )
       // {
       //     Vector2 skill_xPos = skill.transform.position;
       //     skill_xPos.x += 0.1f;
       //     skill.transform.position = skill_xPos;

        //} //145.9231,304.4905f
    } 
     
//    string pathForDocuments(string fileName)
//    {
//#if UNITY_EDITOR
//        string path = Application.dataPath;
//        path = path.Substring(0, path.LastIndexOf('/'));
//        path += "/assets/";
//        return Path.Combine(path, fileName);
//#elif UNITY_ANDROID
//        string path = Application.persistentDataPath;
//        path = path.Substring(0, path.LastIndexOf('/'));
//        path = "jar:file://" + Application.dataPath + "!/assets/";
//        return Path.Combine(path, fileName);
//#endif
//    } 
     
    void Update()
    {
        if (skill_push)
        {
            if (skill_on)
            {
                Vector2 skill_xPos = skill_bg.transform.localPosition;
                skill_xPos.x += 10f;
                moveHow += 10;

                skill_bg.transform.localPosition = skill_xPos;

                if (moveHow >= 210)
                {
                    moveHow = 0;
                    skill_push = false;
                }
            }
            else
            {
                Vector2 skill_xPos = skill_bg.transform.localPosition;
                skill_xPos.x -= 10f;
                moveHow += 10;
                skill_bg.transform.localPosition = skill_xPos;

                if (moveHow >= 210)
                {
                    moveHow = 0;
                    skill_push = false;
                }
            }
        }
    }
}

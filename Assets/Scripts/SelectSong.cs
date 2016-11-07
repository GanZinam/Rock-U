using UnityEngine;
using System.Collections;
using System;

public class SelectSong : MonoBehaviour
{
    // public GameObject[] songTitles;

    public GameObject sceneChange;
    public GameObject sceneOpen;

    AudioSource audio;
    public AudioClip[] clips;

    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text rankText;
    public GameObject[] songTitles;

    public GameObject scroll;
    public GameObject[] albums;
    int num = 0;
    SpringPanel spring;

    public GameObject[] moveButton;
    public GameObject[] unSelectBTs;
    public GameObject[] selectBTs;
    bool isbuttonClick = false;

    int[] scores = new int[8];
    string[] ranks = new string[8];

    void readAllSongsScore(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;
        
        char sp = ',';
        string[] spString = str.Split(sp);
        
        for (int i = 0; i < spString.Length - 1; i++)
        {
            int numInterg = System.Convert.ToInt32(spString[i]);
            scores[i] = numInterg;
        }
    }

    void readAllSongsRank(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            string rankStr = System.Convert.ToString(spString[i]);
            ranks[i] = rankStr;
        }
    }

    IEnumerator sOpen()
    {
        yield return new WaitForSeconds(0.5f);
        sceneOpen.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(sOpen());

        audio = GetComponent<AudioSource>();
        audio.clip = clips[0];
        audio.time = 31;

        System.DateTime.Now.ToString("yyyy");
        string ti =  DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        Debug.Log(ti);

        //readAllSongsScore("Data/UserData/SongScore");
        //readAllSongsRank("Data/UserData/SongRank");

        for (int i = 0; i < 8; i ++)
        {
            scores[i] = PlayerPrefs.GetInt("songScore_" + i);
            ranks[i] = PlayerPrefs.GetString("songRank_" + i);
        }

        scoreText.text = "" + scores[0].ToString("D8");
        rankText.text = ranks[0];

        //rank.text = "";

        moveButton[0].SetActive(false);
        moveButton[1].SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneChange.SetActive(true);
            StartCoroutine(changeSce2());
        }

        //Debug.Log((int)scroll.GetComponent<SpringPanel>().target.x);
        if (((int)scroll.GetComponent<SpringPanel>().target.x) % -410 == 0 && num != (int)scroll.GetComponent<SpringPanel>().target.x / -410)
        {
            songTitles[num].SetActive(false);
            num = (int)scroll.GetComponent<SpringPanel>().target.x / -410;
            Singleton.getInstance.songNum = num;
            songTitles[num].SetActive(true);

            scoreText.text = "" + scores[num].ToString("D8");
            rankText.text = ranks[num];

            audio.Stop();
            if (num < 2)
            {
                audio.clip = clips[num];
                if (num == 0)
                    audio.time = 31;
                else if (num == 1)
                    audio.time = 47;
            }
            audio.Play();

            // 좌 우 버튼 사라지게 하기
            if (num == 0)
                moveButton[0].SetActive(false);
            else
                moveButton[0].SetActive(true);

            if (num == albums.Length - 1)
                moveButton[1].SetActive(false);
            else
                moveButton[1].SetActive(true);


            Debug.Log("NUM : " + num);

            //songTitles[num].SetActive(true);

            for (int i = 0; i < albums.Length; i++)
            {
                albums[i].transform.GetChild(0).GetComponent<UISprite>().SetDimensions(200, 200);
                albums[i].transform.GetChild(1).GetComponent<UISprite>().SetDimensions(200, 200);
            }

            albums[num].transform.GetChild(0).GetComponent<UISprite>().SetDimensions(420, 420);
            albums[num].transform.GetChild(1).GetComponent<UISprite>().SetDimensions(420, 420);
        }
        //else if(!scroll.GetComponent<SpringPanel>().isActiveAndEnabled)
        //{
        //    hideOBJ.SetActive(false);
        //    rank.gameObject.SetActive(false);            
        //}
    }

    public void gameStart()
    {
        Debug.Log(Singleton.getInstance.level + " : " + Singleton.getInstance.songNum);
        DontDestroyOnLoad(Singleton.getInstance.gameObject);

        sceneChange.SetActive(true);
        StartCoroutine(changeSce());
    }

    IEnumerator changeSce()
    {
        yield return new WaitForSeconds(0.5f);
        Application.LoadLevel("InGameScene");
    }

    IEnumerator changeSce2()
    {
        yield return new WaitForSeconds(0.5f);
        Application.LoadLevel("MainScene");
    }

    public void level(int i)
    {
        isbuttonClick = true;
        unSelectBTs[Singleton.getInstance.level].SetActive(true);
        selectBTs[Singleton.getInstance.level].SetActive(false);

        Singleton.getInstance.level = i;

        unSelectBTs[i].SetActive(false);
        selectBTs[i].SetActive(true);
    }

    public void pointUp()
    {
        isbuttonClick = false;
    }
}

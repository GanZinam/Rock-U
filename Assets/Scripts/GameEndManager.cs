using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviour
{
    public GameObject showPresent;

    public UnityEngine.UI.Image cle;
    public Sprite[] clUn;       // clear | unClear

    public GameObject[] gameTitles;

    public SpriteRenderer rank;
    public GameObject expBar;
    public Sprite[] rankSpr;

    public Text levelText;
    public Text progressText;
    public Text missText;
    public Text maxComboText;
    public Text scoreText;
    public Text peopleText;
    public Text BonusEXPText;
    public Text beforeText;
    public Text beforeMaxComboText;
    public Text beforeRankText;

    public GameObject audio;

    int myExp;

    IEnumerator bomb()
    {
        yield return new WaitForSeconds(2.5f);

        audio.SetActive(true);
    }

    void Start()
    {
        if (Singleton.getInstance.miss >= 10)
            cle.sprite = clUn[1];
        else
            cle.sprite = clUn[0];

        gameTitles[Singleton.getInstance.songNum].SetActive(true);

        Debug.Log("Note : " + Singleton.getInstance.noteCount);
        Debug.Log("Mis : " + Singleton.getInstance.miss);

        string myRank = "F";
        int bN = 0, nN = 0;

        int progress;

        if (Singleton.getInstance.miss != 0)
            progress = (Singleton.getInstance.noteCount - Singleton.getInstance.miss) * 100 / Singleton.getInstance.noteCount;
        else
            progress = 100;

        if (Singleton.getInstance.miss >= 10)
            progress = 1;

        if (progress < 40)
        {
            nN = 0;
            myRank = "F";
        }
        else if (progress < 50)
        {
            nN = 1;
            myRank = "D";
        }
        else if (progress < 60)
        {
            nN = 2;
            myRank = "C";
        }
        else if (progress < 70)
        {
            nN = 3;
            myRank = "B";
        }
        else if (progress < 80)
        {
            nN = 4;
            myRank = "A";
        }
        else if (progress < 90)
        {
            nN = 5;
            myRank = "S";
        }
        else
        {
            nN = 6;
            myRank = "SS";
        }

        progressText.text = progress + "%";
        missText.text = Singleton.getInstance.miss + "";
        maxComboText.text = Singleton.getInstance.maxCombo + "";
        scoreText.text = Singleton.getInstance.score + "";
        peopleText.text = (int)Singleton.getInstance.maxCombo / 5 + "";
        BonusEXPText.text = ((int)Singleton.getInstance.maxCombo / 50) / 1 + "";

        if (PlayerPrefs.GetInt("songScore_" + Singleton.getInstance.songNum) < Singleton.getInstance.score)
            PlayerPrefs.SetInt("songScore_" + Singleton.getInstance.songNum, Singleton.getInstance.score);

        if (PlayerPrefs.GetInt("songCombo_" + Singleton.getInstance.songNum) < Singleton.getInstance.maxCombo)
            PlayerPrefs.SetInt("songCombo_" + Singleton.getInstance.songNum, Singleton.getInstance.maxCombo);

        string bS = PlayerPrefs.GetString("songRank_" + Singleton.getInstance.songNum);

        if (bS == "SS")
            bN = 6;
        else if (bS == "S")
            bN = 5;
        else if (bS == "A")
            bN = 4;
        else if (bS == "B")
            bN = 3;
        else if (bS == "C")
            bN = 2;
        else if (bS == "D")
            bN = 1;
        else if (bS == "F")
            bN = 0;

        if (bN < nN)
            PlayerPrefs.SetString("songRank_" + Singleton.getInstance.maxCombo, myRank);
        else
            nN = bN;

        beforeText.text = PlayerPrefs.GetInt("songScore_" + Singleton.getInstance.songNum) + "";
        beforeMaxComboText.text = PlayerPrefs.GetInt("songCombo_" + Singleton.getInstance.songNum) + "";
        beforeRankText.text = PlayerPrefs.GetString("songRank_" + Singleton.getInstance.maxCombo);
        

        //readExp("Data/UserData/Exp");
        if (Singleton.getInstance.miss >= 10)
            nN = 0;
        rank.sprite = rankSpr[nN];


        myExp = PlayerPrefs.GetInt("exp");
        myExp += 160;

        if (PlayerPrefs.GetInt("exp") + PlayerPrefs.GetInt("level") * (PlayerPrefs.GetInt("level") + 1) * 20 - 40 < myExp)
        {
            myExp -= PlayerPrefs.GetInt("level") * (PlayerPrefs.GetInt("level") + 1) * 20 - 40;
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        }

        PlayerPrefs.SetInt("exp", myExp);

        if (myExp != 0)
            expBar.transform.localScale = new Vector3(PlayerPrefs.GetInt("exp") / (PlayerPrefs.GetInt("level") * (PlayerPrefs.GetInt("level") + 1) * 20 - 40), 1, 0);

        if (expBar.transform.localScale.x > 1)
            expBar.transform.localScale = new Vector3(1, 1, 0);


        // 강제 수치 변화   ( 앱 출시 때엔 변경할 것 ) ======================
        //PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        levelText.text = PlayerPrefs.GetInt("level").ToString("D2");

        StartCoroutine(bomb());
    }

    void Update()
    {
        if (showPresent.active)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(Singleton.getInstance.gameObject);
                Application.LoadLevel("MainScene");


                Singleton.getInstance.miss = 0;
                Singleton.getInstance.maxCombo = 0;
                Singleton.getInstance.score = 0;
                Singleton.getInstance.noteCount = 0;

            }
        }
    }

    public void playAgain()
    {

    }

    public void nextButton()
    {
        if (Singleton.getInstance.miss >= 10)
        {
            Destroy(Singleton.getInstance.gameObject);
            Application.LoadLevel("MainScene");


            Singleton.getInstance.miss = 0;
            Singleton.getInstance.maxCombo = 0;
            Singleton.getInstance.score = 0;
            Singleton.getInstance.noteCount = 0;

        }
        else
        {
            showPresent.SetActive(true);
            PlayerPrefs.SetInt("guitar_3", 1);
        }
    }
    

    void readExp(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        myExp = System.Convert.ToInt32(str);
    }

    void readLevel(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        myExp = System.Convert.ToInt32(str);
    }
}
   
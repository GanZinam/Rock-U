// 이전 GameManager.cs 파일


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum TYPE : byte { NOTE, LONG_NOTE, LINK_NOTE }

public static class Define
{
    public const float PERFECT = 0.6f;
    public const float GREAT = 0.7f;
    public const float GOOD = 0.8f;
    public const float BAD = 0.9f;
}

#region TYPE_STRUCT

public struct NOTE
{
    public TYPE type;
    public int sTime;
    public Vector2 sPos;

    public NOTE(int time, Vector2 sPos)
    {
        this.type = TYPE.NOTE;
        this.sTime = time;
        this.sPos = sPos;
    }
}

public struct LONGNOTE
{
    public TYPE type;
    public int sTime;
    public Vector2 sPos;
    public int eTime;
    public int moveType;
    public int childs;

    public LONGNOTE(int sTime, Vector2 sPos, int eTime, int moveType, int childs)
    {
        this.type = TYPE.LONG_NOTE;
        this.sTime = sTime;
        this.sPos = sPos;
        this.eTime = eTime;
        this.moveType = moveType;
        this.childs = childs;
    }
}

public struct LINKNOTE
{
    public TYPE type;
    public int sTime;
    public Vector2 sPos;
    public int eTime;
    public float angle;

    public LINKNOTE(int sTime, Vector2 sPos, int eTime, float angle)
    {
        this.type = TYPE.LINK_NOTE;
        this.sTime = sTime;
        this.sPos = sPos;
        this.eTime = eTime;
        this.angle = angle;
    }
}

#endregion

public class GameManager : MonoBehaviour
{
    public GameObject sceneOpen;
    public GameObject sceneChange;

    // public Animator[] comboAnims;
    public GameObject hpBar;

    public Sprite[] guitaSpr;
    public SpriteRenderer charGuitar;
    public GameObject fireGuitar;

    public Sprite[] backSpr;
    public SpriteRenderer baGround;

    public SpriteRenderer[] comboSpr;
    public Sprite[] numSprite;
    public Sprite[] numSpriteOpa;
    public Sprite[] numSpriteFever;
    public Sprite[] numSpriteFeverOpa;

    [SerializeField]
    GameObject[] note;
    [SerializeField]
    GameObject pauseCanvas;

    [SerializeField]
    Text scoreText;

    AudioSource audio;
    public Animator[] charAnim;

    int count = 0;
    int nScore = 0;
    int nNowScore = 0;
    int nCombo = 0;

    List<NOTE> _noteList = new List<NOTE>();
    List<LONGNOTE> _longNoteList = new List<LONGNOTE>();
    List<LINKNOTE> _linkNoteList = new List<LINKNOTE>();

    int dataCountNum = 0;
    int dataCountNum_long = 0;
    int dataCountNum_link = 0;
    bool ready = false;
    bool pause = false;

    public AudioClip[] au;

    int noteCount = 0;

    void Start()
    {
        int i = 0;
        for (; i < 5; i++)
        {
            if (PlayerPrefs.GetInt("guitar_" + i) == 2)
                break;
        }
        if (PlayerPrefs.GetInt("guitar_4") == 2)
            fireGuitar.SetActive(true);

        if (i == 5)
            i = 4;
        charGuitar.sprite = guitaSpr[i];
        if (i == 4) fireGuitar.SetActive(true);

        baGround.sprite = backSpr[Singleton.getInstance.songNum];

        Singleton.getInstance.miss = 0;
        Singleton.getInstance.maxCombo = 0;
        Singleton.getInstance.score = 0;
        Singleton.getInstance.noteCount = 0;


        _noteList.Clear();
        _longNoteList.Clear();
        _linkNoteList.Clear();

        readStringFromFile("Data/Level" + Singleton.getInstance.level + "/Note/song" + Singleton.getInstance.songNum);
        readStringFromFile_long("Data/Level" + Singleton.getInstance.level + "/LongNote/song" + Singleton.getInstance.songNum);
        readStringFromFile_link("Data/Level" + Singleton.getInstance.level + "/LinkNote/song" + Singleton.getInstance.songNum);
        //Debug.Log(Singleton.getInstance.level + " : " + Singleton.getInstance.songNum);
        audio = GetComponent<AudioSource>();
        audio.clip = au[Singleton.getInstance.songNum];

        StartCoroutine(justWait());
    }

    IEnumerator sOpen()
    {
        yield return new WaitForSeconds(0.5f);
        sceneOpen.SetActive(false);
    }

    IEnumerator justWait()
    {
        yield return new WaitForSeconds(1.5f);
        
        StartCoroutine(countTime());
        StartCoroutine(playSong());
    }

    bool touchCheck(Vector2 touchPos)
    {
        bool che = false;
        RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("NOTE"))// && hit.collider.gameObject == gameObject)
            {
                hit.collider.transform.SendMessage("touchNote");
                che = true;
            }
        }
        return che;
    }

    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(pos.x + ", " + pos.y);

            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            touchCheck(touchPos);
        }

#elif UNITY_ANDROID
            int nbTouches = Input.touchCount;

            if (nbTouches > 0)
            {
                for (int i = 0; i < nbTouches; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(touch.position).x, Camera.main.ScreenToWorldPoint(touch.position).y);

                    if (touch.phase == TouchPhase.Began)
                    {
                        if (touchCheck(touchPos)) break;
                    }
                }
            }
#else
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(pos.x + ", " + pos.y);

            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            touchCheck(touchPos);
        }
#endif

        // Debug.Log(ready + ", " + audio.isPlaying);

        if (ready && !audio.isPlaying && !pause)
        {
            sceneChange.SetActive(true);
            StartCoroutine(goEndScene());
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneChange.SetActive(true);
            StartCoroutine(goMainScene());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Singleton.getInstance.cheat = true;

            int randNum = Random.RandomRange(1, 4);

            for (int i = 0; i < 3; i++)
                charAnim[i].SetInteger("type", randNum);

            StartCoroutine(hideComboText());
        }

        if (!pause)
        {
            if (dataCountNum < _noteList.Count && ready)
            {
                if (_noteList[dataCountNum].sTime <= count)
                {
                    GameObject gO = Instantiate(note[(byte)_noteList[dataCountNum].type], new Vector3(_noteList[dataCountNum].sPos.x, _noteList[dataCountNum].sPos.y, 0), Quaternion.identity) as GameObject;
                    dataCountNum++;
                }
            }
            if (dataCountNum_long < _longNoteList.Count && ready)
            {
                if (_longNoteList[dataCountNum_long].sTime <= count)
                {
                    GameObject gO = Instantiate(note[(byte)_longNoteList[dataCountNum_long].type], new Vector3(_longNoteList[dataCountNum_long].sPos.x, _longNoteList[dataCountNum_long].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _longNoteList[dataCountNum_long]);
                    dataCountNum_long++;
                }
            }
            if (dataCountNum_link < _linkNoteList.Count && ready)
            {
                if (_linkNoteList[dataCountNum_link].sTime <= count)
                {
                    GameObject gO = Instantiate(note[(byte)_linkNoteList[dataCountNum_link].type], new Vector3(_linkNoteList[dataCountNum_link].sPos.x, _linkNoteList[dataCountNum_link].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _linkNoteList[dataCountNum_link]);
                    dataCountNum_link++;
                }
            }

            if (nScore > nNowScore)
            {
                if (nScore - nNowScore < 100)
                    nNowScore++;
                else if (nScore - nNowScore < 1000)
                    nNowScore += 10;
                else if (nScore - nNowScore < 10000)
                    nNowScore += 100;
                else if (nScore - nNowScore < 100000)
                    nNowScore += 1000;
                else
                    nNowScore += 10000;

                scoreText.text = "" + nNowScore.ToString("D8");
            }
        }
    }

    IEnumerator goMainScene()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(Singleton.getInstance.gameObject);
        Application.LoadLevel("MainScene");
    }


    IEnumerator goEndScene()
    {
        yield return new WaitForSeconds(0.5f);

        DontDestroyOnLoad(Singleton.getInstance.gameObject);
        Application.LoadLevel("EndScene");
    }


    IEnumerator playSong()
    {
        yield return new WaitForSeconds(0.6f);

        ready = true;
        GetComponent<AudioSource>().Play();
    }

    IEnumerator comboSpriteChange(int cipher, int comNum)
    {
        yield return new WaitForSeconds(0.05f);

        comboSpr[cipher].sprite = numSpriteOpa[comNum];

        StartCoroutine(comboSpriteChangeAfter(cipher, comNum));
    }

    IEnumerator comboSpriteChangeAfter(int cipher, int comNum)
    {
        yield return new WaitForSeconds(0.05f);

        comboSpr[cipher].sprite = numSprite[comNum];
    }

    public void missNote()
    {
        Singleton.getInstance.miss++;
        nCombo = 0;
        hpBar.transform.localScale -= new Vector3(0.1f, 0);

        comboSpr[0].transform.localPosition = new Vector3(0, 4);
        comboSpr[1].transform.localPosition = new Vector3(0, 4);
        comboSpr[2].transform.localPosition = new Vector3(0, 4);

        if (Singleton.getInstance.miss >= 10)
        {
            DontDestroyOnLoad(Singleton.getInstance.gameObject);
            Application.LoadLevel("EndScene");
        }
    }

    // type
    // 0 : bad(-100)
    // 1 : good(60)
    // 2 : nice(80)
    // 3 : perfect(100)
    public void hitNote(int type)
    {
        nCombo++;

        if (Singleton.getInstance.maxCombo < nCombo)
            Singleton.getInstance.maxCombo = nCombo;


        if (nCombo < 10)
        {
            comboSpr[0].sprite = numSpriteOpa[nCombo - 1];

            StartCoroutine(comboSpriteChange(0, nCombo));

            comboSpr[0].transform.localPosition = Vector3.zero;
        }
        else if (nCombo < 100)
        {
            if (nCombo % 10 == 0)
            {
                comboSpr[0].sprite = numSpriteOpa[9];
                comboSpr[1].sprite = numSpriteOpa[nCombo % 10];

                StartCoroutine(comboSpriteChange(1, nCombo / 10));
            }
            else
            {
                comboSpr[0].sprite = numSpriteOpa[nCombo % 10 - 1];
            }

            StartCoroutine(comboSpriteChange(0, nCombo % 10));

            comboSpr[0].transform.localPosition = new Vector3(0.8f, 0);
            comboSpr[1].transform.localPosition = new Vector3(-0.8f, 0);
        }
        else if (nCombo < 1000)
        {
            // 100
            if (nCombo % 100 == 0)
            {
                comboSpr[0].sprite = numSpriteOpa[9];
                comboSpr[1].sprite = numSpriteOpa[9];
                comboSpr[2].sprite = numSpriteOpa[nCombo / 100 - 1];

                StartCoroutine(comboSpriteChange(1, (nCombo % 100) / 10));
                StartCoroutine(comboSpriteChange(2, nCombo / 100));
            }
            else if (nCombo % 10 == 0)
            {
                comboSpr[0].sprite = numSpriteOpa[9];
                comboSpr[1].sprite = numSpriteOpa[nCombo % 10];

                StartCoroutine(comboSpriteChange(1, (nCombo / 10) % 10));
            }
            else
            {
                comboSpr[0].sprite = numSpriteOpa[nCombo % 10 - 1];
            }
            StartCoroutine(comboSpriteChange(0, nCombo % 10));

            comboSpr[0].transform.localPosition = new Vector3(1.4f, 0);
            comboSpr[1].transform.localPosition = new Vector3(0, 0);
            comboSpr[2].transform.localPosition = new Vector3(-1.4f, 0);
        }
        else
        {
            comboSpr[0].transform.localPosition = new Vector3(0, 4);
            comboSpr[1].transform.localPosition = new Vector3(0, 4);
            comboSpr[2].transform.localPosition = new Vector3(0, 4);
        }


        //for (int i = 0; i < 3; i++)
        //    comboAnims[i].SetBool("show", true);

        if (nCombo < 25)
        {
            nScore += 40 + 20 * (type - 1);
        }
        else if (nCombo < 50)
        {
            nScore += 60 + 30 * (type - 1);
        }
        else if (nCombo < 100)
        {
            nScore += 80 + 40 * (type - 1);
        }
        else if (nCombo < 200)
        {
            nScore += 120 + 60 * (type - 1);
        }
        else
        {
            nScore += 180 + 90 * (type - 1);
        }

        if (nCombo % 5 == 0)
        {
            // 애니메이션 실행
            int randNum = Random.RandomRange(1, 4);
            for (int i = 0; i < 3; i++)
                charAnim[i].SetInteger("type", randNum);

            StartCoroutine(hideComboText());
        }
    }


    IEnumerator hideComboText()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 3; i++)
            charAnim[i].SetInteger("type", 0);
    }

    public void Pause()
    {
        if (!pause)
        {
            //Time.timeScale = 0;
            pause = true;
            pauseCanvas.SetActive(true);
            audio.Pause();
        }
    }

    IEnumerator countOne()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(countTwo());
    }

    IEnumerator countTwo()
    {
        yield return new WaitForSeconds(1);

        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
        pause = false;
        audio.Play();

        StartCoroutine(countTime());
    }

    public void Resume()
    {
        if (pause)
        {
            StartCoroutine(countOne());
        }
    }

    public void GoMain()
    {
        pauseCanvas.SetActive(false);
        sceneChange.SetActive(true);
        StartCoroutine(goMainScene());
    }


    public void Exit()
    {

    }

    #region Application_PAUSE
    //void OnApplicationFocus(bool focusStatus)
    //{
    //    if (focusStatus)
    //    {
    //        Pause();
    //    }
    //    else
    //    {
    //        Resume();
    //    }
    //}

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
    #endregion

    #region ReadFile
    /// <summary>
    ///  기본 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        NOTE note = new NOTE(0, Vector2.zero);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 3 == 0)
            {
                note.sTime = (int)numFloat;
            }
            else if (i % 3 == 1)
            {
                note.sPos.x = numFloat;
            }
            else if (i % 3 == 2)
            {
                note.sPos.y = numFloat;
                _noteList.Add(note);
                Singleton.getInstance.noteCount++;
            }
        }
        return str;
    }

    /// <summary>
    /// 롱 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile_long(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        LONGNOTE note = new LONGNOTE(0, Vector2.zero, 0, 0, 0);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 6 == 0)
            {
                note.sTime = (int)numFloat;
            }
            else if (i % 6 == 1)
            {
                note.sPos.x = numFloat;
            }
            else if (i % 6 == 2)
            {
                note.sPos.y = numFloat;
            }
            else if (i % 6 == 3)
            {
                note.eTime = (int)numFloat;
            }
            else if (i % 6 == 4)
            {
                note.moveType = (int)numFloat;
            }
            else if (i % 6 == 5)
            {
                note.childs = (int)numFloat;
                _longNoteList.Add(note);
                Singleton.getInstance.noteCount++;
            }
        }
        return str;
    }

    /// <summary>
    /// 연결 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile_link(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        LINKNOTE note = new LINKNOTE(0, Vector2.zero, 0, 0);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 5 == 0)
            {
                note.sTime = (int)numFloat;
            }
            else if (i % 5 == 1)
            {
                note.sPos.x = numFloat;
            }
            else if (i % 5 == 2)
            {
                note.sPos.y = numFloat;
            }
            else if (i % 5 == 3)
            {
                note.eTime = (int)numFloat;
            }
            else if (i % 5 == 4)
            {
                note.angle = numFloat;
                _linkNoteList.Add(note);
                Singleton.getInstance.noteCount++;
            }
        }
        return str;
    }

    string pathForDocuments(string fileName)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), fileName);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            //string path = "jar:file:///data/app/com.cid.SwagMon/SwagMon.apk!/";
            string path = Application.streamingAssetsPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, fileName);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, fileName);
        }
    }
    #endregion


    IEnumerator countTime()
    {
        yield return new WaitForSeconds(0.1f);

        if (!pause)
        {
            count++;
            StartCoroutine(countTime());
        }
    }
}
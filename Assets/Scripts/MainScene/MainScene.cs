using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    public GameObject sceneChange;
    public GameObject sceneOpen;

    public UnityEngine.UI.Text level;

    public GameObject fireGuitar;
    public Sprite[] guitarSpr;
    public SpriteRenderer guitar;
    public GameObject expBar;

    IEnumerator sOpen()
    {
        yield return new WaitForSeconds(0.5f);
        sceneOpen.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(sOpen());

        // 기존 초기 데이터 입력
        if (!PlayerPrefs.HasKey("tutorial"))
        {
            PlayerPrefs.SetInt("tutorial", 0);

            for (int i = 0; i < 5; i++)
            {
                PlayerPrefs.SetInt("guitar_" + i, 0);
            }
            PlayerPrefs.SetInt("guitar_" + 0, 2);
            PlayerPrefs.SetInt("guitar_" + 4, 1);

            for (int i = 0; i < 8; i++)
            {
                PlayerPrefs.SetInt("songScore_" + i, 0);
                PlayerPrefs.SetInt("songCombo_" + i, 0);
                PlayerPrefs.SetString("songRank_" + i, "F");
            }
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetInt("exp", 0);
        }

        int num = 0;
        for (int i= 0; i < 5; i ++)
        {
            if (PlayerPrefs.GetInt("guitar_" + i) == 2)
            {
                num = i;
                break;
            }
        }

        guitar.sprite = guitarSpr[num];
        if (num == 4)
            fireGuitar.SetActive(true);

        level.text = PlayerPrefs.GetInt("level").ToString("D2") + "";
        if (PlayerPrefs.GetInt("exp") == 0)
            expBar.transform.localScale = new Vector3(0, 1);
        else
            expBar.transform.localScale = new Vector3(PlayerPrefs.GetInt("exp") / (PlayerPrefs.GetInt("level") * (PlayerPrefs.GetInt("level") + 1) * 20 - 40), 1, 0);

        if (expBar.transform.localScale.x > 1)
            expBar.transform.localScale = new Vector3(1, 1);

    }

    public void GoEquip()
    {
        sceneChange.SetActive(true);
        StartCoroutine(waitTime2());
    }

    public void GoSelect()
    {
        sceneChange.SetActive(true);
        StartCoroutine(waitTime());
    }

    public void GoOption(GameObject Canvas)
    {
        Canvas.SetActive(true);
    }


    IEnumerator waitTime()
    {
        yield return new WaitForSeconds(0.5f);

        Application.LoadLevel("SelectScene");
    }

    IEnumerator waitTime2()
    {
        yield return new WaitForSeconds(0.5f);

        Application.LoadLevel("InventoryScene");
    }
}

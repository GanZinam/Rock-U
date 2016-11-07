using UnityEngine;
using System.Collections;

public class Notice : MonoBehaviour
{
    //public GameObject notice;
    public GameObject leaderboard;
    //public GameObject spr;

    string noticeUrl = "http://cid.dothome.co.kr/gimotti/bbs/se/upload/notice.jpg";

    void Start()
    {
        //notice.SetActive(true);
        StartCoroutine(loadImg());
    }

    IEnumerator loadImg()
    {
        yield return 0;
        WWW imgLink = new WWW(noticeUrl);

        yield return imgLink;

        //Sprite sprite = new Sprite();
        //sprite = Sprite.Create(imgLink.texture, new Rect(0, 0, 162, 135), Vector2.zero);
        //spr.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
    }

    public void seeNotice()
    {
        //notice.SetActive(false);
    }

    public void goStart()
    {
        Application.LoadLevel("InGameScene");
    }

    public void openLeaderboard()
    {
        leaderboard.SetActive(true);
    }

    public void closeLeaderboard()
    {
        leaderboard.SetActive(false);
    }
}

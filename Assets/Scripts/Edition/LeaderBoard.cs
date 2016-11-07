using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public GameObject _board;
    public Transform _scroll;

    string lank;

    void Start()
    {
        getRankingLeaderBoard();
    }

    public void getRankingLeaderBoard()
    {
        StartCoroutine(callGetLeaderBoard());

    }

    public void refreshScore()
    {
        // 기본 데이터를 전송
        WWWForm form = new WWWForm();
        form.AddField("albumName", "app_level_0");
        form.AddField("id", Singleton.getInstance.userId);
        form.AddField("score", 100);

        // php 내에서 처리
        WWW w = new WWW("http://cid.dothome.co.kr/swagmon/refreshScore.php", form);
    }

    IEnumerator callGetLeaderBoard()
    {
        WWWForm form = new WWWForm();
        form.AddField("albumName", "app_level_0");

        WWW w = new WWW("http://cid.dothome.co.kr/swagmon/getRanking.php", form);
        yield return w;

        string list = w.text;
        list += '!';

        int idx = 0;
        bool isId = true;
        bool isScore = false;

        List<char> temp = new List<char>();
        List<string> id = new List<string>();
        List<string> score = new List<string>();

        //@ 데이터를 읽음
        while (list[idx] != '!')
        {
            if (list[idx] == '\n')
            {
                string word = "";

                foreach (var it in temp)
                {
                    word += it;
                }
                score.Add(word);
                temp.Clear();

                isId = true;
                isScore = false;
            }
            else if (isId)
            {
                string word = "";

                if (list[idx] == '\t')
                {
                    foreach (var it in temp)
                    {
                        word += it;
                    }
                    id.Add(word);
                    temp.Clear();

                    isId = false;
                    isScore = true;
                }
                else
                {
                    temp.Add(list[idx]);
                }
            }
            else if (isScore)
            {
                temp.Add(list[idx]);
            }
            idx++;
        }

        for(int i = 0; i < id.Count; i++)
        {
            createBoard(id[i], score[i]);
        }
    }

    void createBoard(string id, string score)
    {
        GameObject myFriend = Instantiate(_board);
        myFriend.transform.localScale = Vector3.one;
        myFriend.transform.SetParent(_scroll);
        myFriend.GetComponentInChildren<UnityEngine.UI.Text>().text = score;

        FB.API(id + "/picture?width=50&height=50", HttpMethod.GET, delegate (IGraphResult result) {
            myFriend.GetComponentInChildren<Image>().sprite = Sprite.Create(result.Texture, new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
        });
    }
}

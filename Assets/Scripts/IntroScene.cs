using UnityEngine;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using System.IO;

public class IntroScene : MonoBehaviour
{
    public GameObject networkCheckCanvas;
    public GameObject failedCanvas;
    public UnityEngine.UI.Text because;

    void Awake()
    {
        checkNetwork();
        StartCoroutine(checkIsPause());
    }

    public void checkNetwork()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            // 연결 실패
            networkCheckCanvas.SetActive(true);
        }
        else if (!HtmlText.Contains("schema.org/WebPage"))
        {
            // 비정상적인 루트일때
            networkCheckCanvas.SetActive(true);
        }
        else
        {
            // 성공적인 연결
            networkCheckCanvas.SetActive(false);

            if (!FB.IsInitialized)
            {
                // Facebook SDK 초기화
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // 초기화 되어 있을 때
                FB.ActivateApp();
                // 정상적인 처리
                DontDestroyOnLoad(Singleton.getInstance.gameObject);
                Application.LoadLevel("MainScene");
            }
        }
    }

    public void exitApp()
    {
        Application.Quit();
    }

    /// <summary>
    /// 인터넷 연결 체크를 위한 함수
    /// </summary>
    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }

    /// <summary>
    /// 페이스북 연동 준비 & 체크 함수
    /// </summary>
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
            because.text = "Failed to Initialize the Facebook SDK";
        }
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // 토큰 받아오기
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            Singleton.getInstance.userId = aToken.UserId;

            // 기본 데이터를 전송
            WWWForm form = new WWWForm();
            form.AddField("id", aToken.UserId);
            form.AddField("nick", "nickname");
            form.AddField("gold", 0);


            // php 내에서 처리
            WWW w = new WWW("http://cid.dothome.co.kr/swagmon/userManager.php", form);
            StartCoroutine(waitForRequest(w));

            /* 이미지 추출 http://graph.facebook.com//picture?type=small */
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    IEnumerator waitForRequest(WWW w)
    {
        yield return w;

        if (w.error == null)
        {
            DontDestroyOnLoad(Singleton.getInstance.gameObject);
            // 정상적인 처리
            Application.LoadLevel("MainScene");

        }
        else
        {
            Debug.Log("Error : " + w.error);
            because.text = w.error;
        }
    }

    // 멈춰있는지 체크
    IEnumerator checkIsPause()
    {
        yield return new WaitForSeconds(3);

        failedCanvas.SetActive(true);
        checkNetwork();

        StartCoroutine(checkIsPause());
    }
}

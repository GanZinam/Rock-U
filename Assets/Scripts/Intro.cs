using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Intro : MonoBehaviour {

    public Animator anim;
    bool a = true;

    bool b = false;

    bool c = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (a)
        {
            GameObject.Find("Team_Icon").GetComponent<Image>().color += new Color(0, 0, 0, 0.01f);
            GameObject.Find("Team_Logo").GetComponent<Image>().color += new Color(0, 0, 0, 0.01f);
        
            if(GameObject.Find("Team_Icon").GetComponent<Image>().color.a >= 1)
            {

                StartCoroutine(StartFunc(1.5f));
            }
        }
        else
        {
            if(b)
            {
                GameObject.Find("game_name").transform.localPosition = new Vector3(0,0,0);
                //GameObject.Find("Button").transform.localPosition = new Vector3(0, 0, 0);

                if (c)
                {
                    anim.SetTrigger("ani");
                    StartCoroutine(StartFunc_(1.5f));
                    c = false;
                }
            }
        }
	}

    IEnumerator StartFunc(float time)
    {
        yield return new WaitForSeconds(time);
        if (a)
        {
            GameObject.Find("Team_Icon").SetActive(false);
            GameObject.Find("Team_Logo").SetActive(false);
            a = false;
            b = true;

        }
    }

    IEnumerator StartFunc_(float time)
    {
        yield return new WaitForSeconds(time);

        // GameObject.Find("Button").transform.localPosition = new Vector3(0, 0, 0);
        startGame();
    }


    public void startGame()
    {
        Debug.Log(1);
        Application.LoadLevel("MainScene");
    }
}

//Application.LoadLevel("MainScene");

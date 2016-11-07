using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LongNote : MonoBehaviour
{
    public GameObject note;
    public GameObject ring;
    public GameObject bar;
    public GameObject dot;
    public GameObject effect;
    public GameObject[] decisions;
    public GameObject starEff;

    GameObject myEffect = null;
    GameObject myDecision = null;
    GameObject myStar = null;
    GameObject myBar;

    Transform gm;
    LONGNOTE myData;

    int count = 0;
    float moveX = 0, moveY = 0;
    bool isTouch = false;
    bool isDecrease = false;
    Vector2 ePos;


    void Start()
    {
        gm = GameObject.Find("_GameManager").transform;
        StartCoroutine(descrease());
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && isTouch)
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            touchRay(touchPos);
        }
        else if (Input.GetMouseButtonUp(0) && isTouch)
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            touchRay(touchPos);
        }
        else if (Input.GetMouseButton(0) && isTouch)
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            touchRay(touchPos);
        }

#elif UNITY_ANDROID
        int nbTouches = Input.touchCount;

        if (nbTouches > 0)
        {
            DEC dec = DEC.NOT;

            for (int i = 0; i < nbTouches; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(touch.position).x, Camera.main.ScreenToWorldPoint(touch.position).y);

                if (touch.phase == TouchPhase.Began && !isDecrease)
                {
                    dec = touchRay(touchPos);
                }
                else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isTouch)
                {
                    dec = DEC.NOT;
                }
                else if ((touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) && isTouch)
                {
                    dec = touchRay(touchPos);
                }

                if (dec == DEC.C_COLL) break;
            }

            if (dec == DEC.NOT && isTouch)
            {
                // 끝까지 터치하고 있지 않았을 때
                isTouch = false;
                note.transform.GetComponent<SpriteRenderer>().material.color = Color.gray;
                gm.SendMessage("missNote");

                Destroy(myEffect);
                Destroy(myDecision);
                //myEffect.SetActive(false);
            }
        }
#else 
        if (Input.GetMouseButtonDown(0) && isTouch)
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            touchRay(touchPos);
        }
        else if (Input.GetMouseButtonUp(0) && isTouch)
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            touchRay(touchPos);
        }
        else if (Input.GetMouseButton(0) && isTouch)
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            touchRay(touchPos);
        }
#endif
    }

    DEC touchRay(Vector2 touchPos)
    {
        DEC dec = DEC.NOT;

        Ray2D ray = new Ray2D(touchPos, Vector2.zero);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            dec = DEC.N_COLL;
            if (hit.collider.transform.parent.gameObject == gameObject)
            {
                dec = DEC.C_COLL;
                if (!isDecrease)
                {
                    isTouch = true;
                    isDecrease = true;
                    ring.SetActive(false);

                    hitCheck();

                    StartCoroutine(showEffect());

                    moving();
                    StartCoroutine(countTime());
                }
            }
            else if (isDecrease)
            {
                note.transform.GetComponent<SpriteRenderer>().material.color = Color.gray;
                //Destroy(myEffect);
                myEffect.SetActive(false);
            }
        }
        else if (isDecrease)
        {
            note.transform.GetComponent<SpriteRenderer>().material.color = Color.gray;
            //Destroy(myEffect);
            myEffect.SetActive(false);
        }

        return dec;
    }

    public void initData(LONGNOTE baseData)
    {
        myData = baseData;

        //_myChilds.Add(myData.ePos);

        switch (myData.moveType)
        {
            case 0:
                moveX = 0; moveY = 0.02f;
                break;
            case 1:
                moveX = 0.0066f; moveY = 0.0166f;
                break;
            case 2:
                moveX = 0.0166f; moveY = 0.0066f;
                break;
            case 3:
                moveX = 0.02f; moveY = 0;
                break;
            case 4:
                moveX = 0.0166f; moveY = -0.0066f;
                break;
            case 5:
                moveX = 0.0066f; moveY = -0.0166f;
                break;
            case 6:
                moveX = 0; moveY = -0.02f;
                break;
            case 7:
                moveX = -0.0066f; moveY = -0.0166f;
                break;
            case 8:
                moveX = -0.0166f; moveY = -0.0066f;
                break;
            case 9:
                moveX = -0.02f; moveY = 0;
                break;
            case 10:
                moveX = -0.0166f; moveY = 0.0066f;
                break;
            case 11:
                moveX = -0.0066f; moveY = 0.0166f;
                break;
        }

        ePos.x = moveX * myData.eTime * 30 * 0.75f; // 1 * 5 * 30 == 150
        ePos.y = moveY * myData.eTime * 30 * 0.75f;

        GameObject a = Instantiate(dot, ePos, Quaternion.identity) as GameObject;
        a.transform.parent = transform;
        a.transform.localPosition = ePos;


        myBar = Instantiate(bar, a.transform.position, Quaternion.identity) as GameObject;
        myBar.transform.parent = a.transform;
        myBar.transform.localScale = new Vector3(0.3f * myData.eTime * 0.75f, 1, 0);
        //myBar.transform.localScale -= new Vector3(0.064f, 0, 0);
        myBar.transform.Rotate(new Vector3(0, 0, (Mathf.Atan2(ePos.y, ePos.x) * Mathf.Rad2Deg + 180) % 360));


    }

    public void touchNote()
    {
        isTouch = true;
    }

    // 원 줄어들기
    IEnumerator descrease()
    {
        yield return new WaitForSeconds(0.01f);

        if (ring.active)
        {
            ring.transform.localScale -= new Vector3(0.025f, 0.025f, 0);

            if (ring.transform.localScale.x <= 0.42f)
            {
                if (Singleton.getInstance.cheat)
                {
                    isTouch = true;
                    isDecrease = true;
                    ring.SetActive(false);

                    hitCheck();

                    StartCoroutine(showEffect());

                    moving();
                    StartCoroutine(countTime());
                }
                else
                {
                    gm.SendMessage("missNote");
                    Destroy(gameObject);
                }
            }
            else
                StartCoroutine(descrease());
        }
    }

    // 노드 움직이기
    IEnumerator moveTo()
    {
        yield return new WaitForSeconds(0.01f);

        moving();
    }

    //50 - 1.5
    //10 - 0.3
    //1 - 0.03

    void moving()
    {
        myBar.transform.localScale -= new Vector3(0.03f, 0, 0);
        note.transform.position += new Vector3(moveX * 3, moveY * 3, 0);

        if (myDecision != null)
            myDecision.transform.position = note.transform.position;

        if (myStar != null)
            myStar.transform.position = note.transform.position;
        //if (myEffect != null)
        //    myEffect.transform.position = note.transform.position;

        StartCoroutine(moveTo());
    }

    // 이펙트 보여주기
    IEnumerator showEffect()
    {
        yield return new WaitForSeconds(0.1f);

        if (isTouch)
        {
            hitCheck();

            StartCoroutine(showEffect());
        }
    }

    void hitCheck()
    {
        if (myStar == null)
        {
            myStar = Instantiate(starEff, note.transform.position, Quaternion.identity) as GameObject;
            myStar.transform.parent = note.transform;
        }

        if (myEffect == null)
        {
            myEffect = Instantiate(effect, note.transform.position, Quaternion.identity) as GameObject;
            myEffect.transform.parent = note.transform;
        }

        if (ring.transform.localScale.x < Define.PERFECT)
        {
            if (myDecision == null)
            {
                myDecision = Instantiate(decisions[0], transform.position, Quaternion.identity) as GameObject;
                myDecision.GetComponent<DecisionManager>().enabled = false;
            }
            gm.SendMessage("hitNote", 4);
        }
        else if (ring.transform.localScale.x < Define.GREAT)
        {
            if (myDecision == null)
            {
                myDecision = Instantiate(decisions[1], transform.position, Quaternion.identity) as GameObject;
                myDecision.GetComponent<DecisionManager>().enabled = false;
            }
            gm.SendMessage("hitNote", 3);
        }
        else if (ring.transform.localScale.x < Define.GOOD)
        {
            if (myDecision == null)
            {
                myDecision = Instantiate(decisions[2], transform.position, Quaternion.identity) as GameObject;
                myDecision.GetComponent<DecisionManager>().enabled = false;
            }
            gm.SendMessage("hitNote", 2);
        }
        else
        {
            if (myDecision == null)
            {
                myDecision = Instantiate(decisions[3], transform.position, Quaternion.identity) as GameObject;
                myDecision.GetComponent<DecisionManager>().enabled = false;
            }
            transform.GetComponent<SpriteRenderer>().material.color = Color.gray;
            gm.SendMessage("hitNote", 1);
        }
    }

    // 시간 재기
    IEnumerator countTime()
    {
        yield return new WaitForSeconds(0.1f);


        if (isDecrease)
        {
            if (count >= myData.eTime)
            {
                Destroy(gameObject);
                Destroy(myEffect);
                Destroy(myDecision);

                if (!isTouch)
                {
                    Instantiate(decisions[4], transform.position, Quaternion.identity);
                }
            }
        }
        count++;

        StartCoroutine(countTime());
    }
}

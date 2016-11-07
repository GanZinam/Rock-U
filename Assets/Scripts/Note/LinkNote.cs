using UnityEngine;
using System.Collections;


public enum DEC : byte { NOT, N_COLL, C_COLL}

public class LinkNote : MonoBehaviour
{
    public GameObject starEff;
    public GameObject ring;
    public GameObject link;
    public GameObject effect;
    public GameObject[] decisions;

    GameObject myEffect = null;
    GameObject myDecision = null;
    GameObject myStar = null;

    Transform gm;

    LINKNOTE myData;
    bool isDecrease = false;
    bool isTouch = false;
    int count = 0;

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

#elif UNITY_ANDROID
        int nbTouches = Input.touchCount;

        if (nbTouches > 0)
        {
            DEC dec = DEC.NOT;

            for (int i = 0; i < nbTouches; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(touch.position).x, Camera.main.ScreenToWorldPoint(touch.position).y);

                if (touch.phase == TouchPhase.Began)
                {
                    dec = touchRay(touchPos);
                }
                else if ((touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) && isTouch)
                {
                    dec = touchRay(touchPos);
                }
                else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isTouch)
                {
                    dec = DEC.NOT;
                }

                if (dec == DEC.C_COLL) break;
            }

            if (dec == DEC.NOT && isTouch)
            {
                // 끝까지 터치하고 있지 않았을 때
                isTouch = false;
                transform.GetComponent<SpriteRenderer>().material.color = Color.gray; // Color.black;// new Color(1, 1, 1, 0.4f);
                
                gm.SendMessage("missNote");

                Destroy(myEffect);
                Destroy(myDecision);
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
#endif
    }

    /* isUp = false : 처음
     * isUp = true  : 그 후
     */
    DEC touchRay(Vector2 touchPos)
    {
        DEC dec = DEC.NOT;

        Ray2D ray = new Ray2D(touchPos, Vector2.zero);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            dec = DEC.N_COLL;
            if (hit.collider.gameObject.CompareTag("LINKNOTE") && hit.collider.gameObject == gameObject)
            {
                dec = DEC.C_COLL;
                if (!isDecrease)
                {
                    // 처음 터치했을 때
                    isTouch = true;
                    isDecrease = true;
                    ring.SetActive(false);

                    hitCheck();

                    StartCoroutine(minusLinkScale());
                    StartCoroutine(showEffect());
                    StartCoroutine(countTime());
                }
            }
        }

        return dec;
    }

    public void initData(LINKNOTE baseData)
    {
        myData = baseData;

        link.transform.localScale = new Vector3(myData.eTime * 0.08f, 1, 0);
        link.transform.localScale -= new Vector3(0.064f, 0, 0);

        link.transform.Rotate(new Vector3(0, 0, myData.angle));
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
                    // 처음 터치했을 때
                    isTouch = true;
                    isDecrease = true;
                    ring.SetActive(false);

                    hitCheck();

                    StartCoroutine(minusLinkScale());
                    StartCoroutine(showEffect());
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

    // 링크 줄어들기
    IEnumerator minusLinkScale()
    {
        yield return new WaitForSeconds(0.01f);

        if (isDecrease)
        {
            link.transform.localScale -= new Vector3(0.01f, 0, 0);
            StartCoroutine(minusLinkScale());
        }
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
        if (myEffect == null)
            myEffect = Instantiate(effect, transform.position, Quaternion.identity) as GameObject;

        if (myStar == null)
        {
            myStar = Instantiate(starEff, transform.position, Quaternion.identity) as GameObject;
            myStar.GetComponent<ClickEffect>().enabled = false;
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
                Destroy(myStar);

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

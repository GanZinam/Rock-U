using UnityEngine;
using System.Collections;
public class Note : MonoBehaviour
{
    public GameObject starEff;
    public GameObject ring;
    public GameObject effect;
    public GameObject[] decisions;

    Transform gm;

    void Start()
    {
        gm = GameObject.Find("_GameManager").transform;
        StartCoroutine(descrease());
    }

//    void Update()
//    {
//#if UNITY_EDITOR
//        if (Input.GetMouseButtonDown(0))
//        {
//            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            Debug.Log(pos.x + ", " + pos.y);

//            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
//            touchCheck(touchPos);
//        }

//#elif UNITY_ANDROID
//            int nbTouches = Input.touchCount;

//            if (nbTouches > 0)
//            {
//                for (int i = 0; i < nbTouches; i++)
//                {
//                    Touch touch = Input.GetTouch(i);
//                    Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(touch.position).x, Camera.main.ScreenToWorldPoint(touch.position).y);

//                    if (touch.phase == TouchPhase.Began)
//        {

//                        if (touchCheck(touchPos)) break;
//        }
//                }
//            }
//#endif
//    }


//    bool touchCheck(Vector2 touchPos)
//    {
//        bool che = false;
//        RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

//        if (hit.collider != null)
//        {
//            if (hit.transform.CompareTag("NOTE") && hit.collider.gameObject == gameObject)
//            {
//                touchNote();
//                che = true;
//            }
//        }
//        return che;
//    }

    public void touchNote()
    {
        hitCheck();

        Destroy(gameObject);
    }

    void hitCheck()
    {
        Instantiate(effect, transform.position, Quaternion.identity);
        Instantiate(starEff, transform.position, Quaternion.identity);

        Singleton.getInstance.score += 100;

        if (ring.transform.localScale.x < Define.PERFECT)
        {
            Instantiate(decisions[0], transform.position, Quaternion.identity);
            gm.SendMessage("hitNote", 4);
        }
        else if (ring.transform.localScale.x < Define.GREAT)
        {
            Instantiate(decisions[1], transform.position, Quaternion.identity);
            gm.SendMessage("hitNote", 3);
        }
        else if (ring.transform.localScale.x < Define.GOOD)
        {
            Instantiate(decisions[2], transform.position, Quaternion.identity);
            gm.SendMessage("hitNote", 2);
        }
        else
        {
            Instantiate(decisions[3], transform.position, Quaternion.identity);
            transform.GetComponent<SpriteRenderer>().material.color = Color.gray;
            gm.SendMessage("hitNote", 1);
        }
    }

    IEnumerator descrease()
    {
        yield return new WaitForSeconds(0.01f);

        ring.transform.localScale -= new Vector3(0.025f, 0.025f, 0);


        if (ring.transform.localScale.x <= 0.42f)
        {
            if(Singleton.getInstance.cheat)
            {
                hitCheck();
                Destroy(gameObject);
            }
            else
            {
                gm.SendMessage("missNote");

                Destroy(gameObject);
            }

            //Instantiate(effect[3], transform.position, Quaternion.identity);
        }
        else
            StartCoroutine(descrease());
    }
}

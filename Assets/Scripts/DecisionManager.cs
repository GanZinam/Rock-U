using UnityEngine;
using System.Collections;

public class DecisionManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(remove());
    }
    IEnumerator remove()
    {
        yield return new WaitForSeconds(0.3f);

        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class ClickEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(remove());
	}
	
    IEnumerator remove()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}

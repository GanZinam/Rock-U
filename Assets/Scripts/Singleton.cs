using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour
{
    private static Singleton instance = null;
    public string userId = "";
    public bool cheat = false;
    public int level = 0;
    public int songNum = 0;

    public int miss = 0;
    public int score = 0;
    public int maxCombo = 0;
    public int noteCount = 0;

    public static Singleton getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(Singleton)) as Singleton;
                if (instance == null)
                {
                    Debug.Log("No Instance");
                }
            }
            return instance;
        }
    }
}

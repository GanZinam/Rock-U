using UnityEngine;
using System.Collections;

public class OPtion : MonoBehaviour
{

    public Canvas _1;
    public Canvas _2;
    public GameObject _3;
    public Canvas _4;

    public UnityEngine.UI.Image Sound;
    public Sprite Sound_On;
    public Sprite Sound_Off;
    public UnityEngine.UI.Image Vibe;
    public Sprite Vibe_On;
    public Sprite Vibe_Off;
    public bool sound_bool;
    public bool vibe_bool;

    public void SoundOn()
    {
        if (sound_bool)
        {
            Sound.sprite = Sound_Off;
            sound_bool = false;
        }
        else
        {
            Sound.sprite = Sound_On;
            sound_bool = true;
        }
    }
    public void VibeOn()
    {
        if (vibe_bool)
        {
            Vibe.sprite = Vibe_Off;
            vibe_bool = false;
        }
        else
        {
            Vibe.sprite = Vibe_On;
            vibe_bool = true;
        }
    }
    public void Goback(GameObject Canvas)
    {
        Canvas.SetActive(false);
    }
    public void PushCredit()
    {
        _1.gameObject.SetActive(false);
        _2.gameObject.SetActive(false);
        _3.gameObject.SetActive(false);
        _4.gameObject.SetActive(true);
    }
    public void PushBack()
    {
        _1.gameObject.SetActive(true);
        _2.gameObject.SetActive(true);
        _3.gameObject.SetActive(true);
        _4.gameObject.SetActive(false);
    }
}

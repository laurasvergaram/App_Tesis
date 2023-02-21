using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundFX : MonoBehaviour
{
    private static SoundFX instance = null;
    public static SoundFX Instance
    {
        get {return instance; }
    }

    [SerializeField] private Button boton = null;

    public void Start()
    {
        boton.onClick.AddListener(this.MenuButtons);
    }

    // Update is called once per frame
    public void MenuButtons()
    {
        if(instance != null && instance != this)
        {
            instance.GetComponent<AudioSource>().Play();
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            instance.GetComponent<AudioSource>().Play();
        }

        DontDestroyOnLoad(this.gameObject);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    CanvasGroup canvasgroup;

    public float fadeInDuration;
    public float fadeOutDuration;
    // Start is called before the first frame update
    private void Awake()
    {
        canvasgroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }



    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(fadeInDuration);

        yield return FadeInput(fadeOutDuration);

    }
    //场景渐出的动画
    public IEnumerator FadeOut(float time)
    {
        while(canvasgroup.alpha <1)
        {
            canvasgroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    
    }

    //场景渐入的动画
    public IEnumerator FadeInput(float time)
    {
        while(canvasgroup.alpha !=0)
        {
            canvasgroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        Destroy(gameObject);
    }
}

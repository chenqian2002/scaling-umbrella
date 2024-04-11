using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;


public class MainMenu : MonoBehaviour
{
    Button newGameBt;
    Button continueBt;
    Button quitBt;
    //动画导演
    PlayableDirector director;

     void Awake()
    {
        newGameBt = transform.GetChild(1).GetComponent<Button>();

        continueBt= transform.GetChild(2).GetComponent<Button>();

        quitBt = transform.GetChild(3).GetComponent<Button>();

        newGameBt.onClick.AddListener(PlayeTimeLine);

        continueBt.onClick.AddListener(ContinueGamge);

        quitBt.onClick.AddListener(QuitGame);

        //获取PlayableDirector的所有组件
        director = FindObjectOfType<PlayableDirector>();
        //当动画结束后执行新场景
        director.stopped += NewGamge;
    }


    void PlayeTimeLine()
    {//播放动画 
        director.Play();
    }

    void NewGamge(PlayableDirector obj)
    {
        //清除游戏数据
        PlayerPrefs.DeleteAll();

        //转换场景调用函数方法

        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGamge()
    {
        //转换场景

        SceneController.Instance.TransitionToLoadGame();
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("退出游戏");
    }
}


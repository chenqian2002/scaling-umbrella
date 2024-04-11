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
    //��������
    PlayableDirector director;

     void Awake()
    {
        newGameBt = transform.GetChild(1).GetComponent<Button>();

        continueBt= transform.GetChild(2).GetComponent<Button>();

        quitBt = transform.GetChild(3).GetComponent<Button>();

        newGameBt.onClick.AddListener(PlayeTimeLine);

        continueBt.onClick.AddListener(ContinueGamge);

        quitBt.onClick.AddListener(QuitGame);

        //��ȡPlayableDirector���������
        director = FindObjectOfType<PlayableDirector>();
        //������������ִ���³���
        director.stopped += NewGamge;
    }


    void PlayeTimeLine()
    {//���Ŷ��� 
        director.Play();
    }

    void NewGamge(PlayableDirector obj)
    {
        //�����Ϸ����
        PlayerPrefs.DeleteAll();

        //ת���������ú�������

        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGamge()
    {
        //ת������

        SceneController.Instance.TransitionToLoadGame();
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("�˳���Ϸ");
    }
}


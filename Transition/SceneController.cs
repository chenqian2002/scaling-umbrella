using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;


public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPerfab;
    public SceneFader scenefader;

    GameObject player;
    NavMeshAgent playerAgent;

    bool faderFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }


     void Start()
    {
       
        GameManager.Instance.AddObserver(this);
       faderFinished = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:

                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScen:
                StartCoroutine(Transition(transitionPoint.scenName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        SceneFader fade= Instantiate(scenefader);
        SaveManger.Instance.SavePlayerData();
        if (SceneManager.GetActiveScene().name !=sceneName)
        {
            yield return StartCoroutine(fade.FadeOut(2.5f));
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPerfab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManger.Instance.LoadPlayerData();

            yield return StartCoroutine(fade.FadeInput(2.5f));
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
     
    }
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }
        return null;

    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());   
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManger.Instance.SceneName));
    }


    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }


    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(scenefader);
        if (scene != "")
        {

            yield return StartCoroutine(fade.FadeOut(2.5f));
          //如果场景不为空，加载场景
          yield return SceneManager.LoadSceneAsync(scene);
            //生成一个游戏角色
            yield return player = Instantiate(playerPerfab,GameManager.Instance.GetEntrance().position, 
                GameManager.Instance.GetEntrance().rotation);
            SaveManger.Instance.SavePlayerData();

            yield return StartCoroutine(fade.FadeInput(2.5f));
            yield break;
        } 
    
    }


    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(scenefader);

        yield return StartCoroutine(fade.FadeOut(2.5f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeInput(2.5f));
        yield  break;

    }

    public void EndNotify()
    {
        if(faderFinished)
        {
            faderFinished = false;
            StartCoroutine(LoadMain());
        }
   
    }
}


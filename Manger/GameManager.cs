using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats characterStats;


    private CinemachineFreeLook followCamera;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public CharacterStats playerStats;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();



    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();

        if (followCamera !=null)
        {
            followCamera.Follow = player.transform.GetChild(2);
            followCamera.LookAt = player.transform.GetChild(2);
        }
    }




 public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }


    public void NotifyObserver()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    internal void AddObserver(object enemyControl1Instance)
    {
        throw new NotImplementedException();
    }
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER) 
            return item.transform;

        }
        return null;
    }
}

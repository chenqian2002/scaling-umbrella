using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScen
    }

    [Header("Transition Info")]

    public string scenName;

    public TransitionType transitionType;


    public TransitionDestination.DestinationTag destinationTag;


    public bool canTrans;

     void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false ;
    }
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)&&canTrans)
        {
            //³¡¾°´«ËÍ
            SceneController.Instance.TransitionToDestination(this); 
        }
    }
}

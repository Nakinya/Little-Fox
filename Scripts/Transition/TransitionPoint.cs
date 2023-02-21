using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType { SameScene,DifferentScene}//表示是同场景传送还是不同场景

    [Header("Transition Info")]
    public string scenceName;//要传送的场景名
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;//传送到的场景的位置

    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            //TODO:SceneController传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)//注意，trigger必须要有一个物体带有刚体组件，否则不会触发
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            canTrans = false;
    }
}

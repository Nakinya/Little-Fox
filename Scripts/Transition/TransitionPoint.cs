using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType { SameScene,DifferentScene}//��ʾ��ͬ�������ͻ��ǲ�ͬ����

    [Header("Transition Info")]
    public string scenceName;//Ҫ���͵ĳ�����
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;//���͵��ĳ�����λ��

    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            //TODO:SceneController����
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)//ע�⣬trigger����Ҫ��һ��������и�����������򲻻ᴥ��
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

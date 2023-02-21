using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///��չ����
///</summary>
public static class ExtensionMethod //��չ��������̳��������࣬����Ϊ��̬��
{
    private const float dotThreshold = 0.5f;

    public static bool IsFacingTarget(this Transform transform, Transform target)//this���η���Ҫ���õ����ͣ�֮���Ϊ��������.��չ���������Ǿ�̬��
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();
        float dot =  Vector3.Dot(vectorToTarget, transform.forward);//���������ĵ�˻�
        return dot >= dotThreshold; //Ŀ������Ұ��Χ120����
    }
}

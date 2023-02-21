using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///拓展方法
///</summary>
public static class ExtensionMethod //拓展方法不会继承其他的类，并且为静态类
{
    private const float dotThreshold = 0.5f;

    public static bool IsFacingTarget(this Transform transform, Transform target)//this修饰方法要作用的类型，之后的为方法参数.拓展方法必须是静态的
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();
        float dot =  Vector3.Dot(vectorToTarget, transform.forward);//两个向量的点乘积
        return dot >= dotThreshold; //目标在视野范围120°内
    }
}

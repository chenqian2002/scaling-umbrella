using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f;
    // Start is called before the first frame update
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {   //��ȡplayer ���� enemy�����λ��
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;
    }
}

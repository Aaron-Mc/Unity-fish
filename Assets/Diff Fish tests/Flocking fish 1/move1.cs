using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move1 : MonoBehaviour
{
    public Vector3 pointB;
   
    IEnumerator Start()
    {
        var pointA = transform.position;
        while(true)
        {
            yield return StartCoroutine(MoveObject(transform, pointA, Vector3.forward, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.forward, Vector3.zero, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.zero, Vector3.right, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.right, Vector3.left, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.left, Vector3.zero, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.zero, Vector3.up, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.up, Vector3.down, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.down, Vector3.zero, 5.0f));
            yield return StartCoroutine(MoveObject(transform, Vector3.zero, pointA, 5.0f));

        }
    }
   
    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        var i= 0.0f;
        var rate= 5.0f/time;
        while(i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
    }
}

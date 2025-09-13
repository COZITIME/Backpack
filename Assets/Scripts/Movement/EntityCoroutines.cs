using System.Collections;
using UnityEngine;

public static class EntityCoroutines
{
    public static IEnumerator ScaleToCoroutine(Transform transform, float duration, Vector3 startScale, Vector3 endScale)
    {
        float increment = 1f / duration;
        float t = 0f;
        while (t < 1)
        {
            t += increment * Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }
    
    public static IEnumerator MoveToPositionCoroutine(Transform transform, float duration, Vector3 startPosition, Vector3 endPosition)
    {
        float increment = 1f / duration;

        float t = 0f;
        while (t < 1)
        {
            t += increment * Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
    }
}
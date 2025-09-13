using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Extension methods for MonoBehaviour to easily schedule delayed or interpolated actions.
/// </summary>
public static class TweenExtensions
{
    // Cache common yield instructions to reduce GC allocations
    private static readonly WaitForEndOfFrame WaitForEndOfFrameCached = new WaitForEndOfFrame();

    #region Delay by Seconds

    /// <summary>
    /// Executes an action after a given number of seconds (scaled by Time.timeScale).
    /// </summary>
    public static Coroutine ExecuteDelayedSeconds(this MonoBehaviour monoBehaviour, float delay, Action action)
    {
        return monoBehaviour.StartCoroutine(ExecuteAfterDelaySeconds(delay, action));
    }

    private static IEnumerator ExecuteAfterDelaySeconds(float delay, Action action)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        action?.Invoke();
    }

    /// <summary>
    /// Executes an action after a given number of realtime seconds (ignores Time.timeScale).
    /// </summary>
    public static Coroutine ExecuteDelayedRealtime(this MonoBehaviour monoBehaviour, float delay, Action action)
    {
        return monoBehaviour.StartCoroutine(ExecuteAfterDelayRealtime(delay, action));
    }

    private static IEnumerator ExecuteAfterDelayRealtime(float delay, Action action)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        action?.Invoke();
    }

    #endregion

    #region Delay by Frame

    /// <summary>
    /// Executes an action after 1 frame.
    /// </summary>
    public static Coroutine ExecuteDelayedFrame(this MonoBehaviour monoBehaviour, Action action)
    {
        return monoBehaviour.StartCoroutine(ExecuteAfterDelayFrame(1, action));
    }

    /// <summary>
    /// Executes an action after the specified number of frames.
    /// </summary>
    public static Coroutine ExecuteDelayedFrame(this MonoBehaviour monoBehaviour, int frames, Action action)
    {
        return monoBehaviour.StartCoroutine(ExecuteAfterDelayFrame(frames, action));
    }

    private static IEnumerator ExecuteAfterDelayFrame(int frames, Action action)
    {
        for (int i = 0; i < frames; i++)
            yield return WaitForEndOfFrameCached;

        action?.Invoke();
    }

    #endregion

    #region Lerp Execution

    /// <summary>
    /// Executes an action over time with a t value from 0 → 1 (scaled by Time.timeScale).
    /// </summary>
    /// <param name="monoBehaviour">The MonoBehaviour running the coroutine.</param>
    /// <param name="duration">Time in seconds for the lerp.</param>
    /// <param name="onUpdate">Action to execute each frame, receives a t value in [0,1].</param>
    /// <param name="onComplete">Optional action executed at t=1.</param>
    public static Coroutine ExecuteLerp(this MonoBehaviour monoBehaviour, float duration, Action<float> onUpdate, Action onComplete = null)
    {
        return monoBehaviour.StartCoroutine(LerpRoutine(duration, false, onUpdate, onComplete));
    }

    /// <summary>
    /// Executes an action over time with a t value from 0 → 1 (ignores Time.timeScale).
    /// </summary>
    public static Coroutine ExecuteLerpRealtime(this MonoBehaviour monoBehaviour, float duration, Action<float> onUpdate, Action onComplete = null)
    {
        return monoBehaviour.StartCoroutine(LerpRoutine(duration, true, onUpdate, onComplete));
    }

    private static IEnumerator LerpRoutine(float duration, bool unscaled, Action<float> onUpdate, Action onComplete)
    {
        if (duration <= 0f)
        {
            onUpdate?.Invoke(1f);
            onComplete?.Invoke();
            yield break;
        }

        float time = 0f;

        while (time < duration)
        {
            float delta = unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            time += delta;
            float t = Mathf.Clamp01(time / duration);

            onUpdate?.Invoke(t);
            yield return null;
        }

        onComplete?.Invoke();
    }

    #endregion
}

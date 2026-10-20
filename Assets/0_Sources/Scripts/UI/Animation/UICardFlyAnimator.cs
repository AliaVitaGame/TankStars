using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICardFlyAnimator : MonoBehaviour
{
    [SerializeField] private float popDuration = 0.14f;
    [SerializeField] private float flyDuration = 0.62f;
    [SerializeField] private float popScale = 1.18f;
    [SerializeField] private float endScale = 0.28f;
    [SerializeField] private float arcHeight = 220f;
    [SerializeField] private float spinAngle = 18f;

    public void Play(RectTransform target, Action onComplete)
    {
        StopAllCoroutines();
        StartCoroutine(FlyRoutine(target, onComplete));
    }

    private IEnumerator FlyRoutine(RectTransform target, Action onComplete)
    {
        var rt = transform as RectTransform;
        if (rt == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        var canvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        var layout = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        layout.ignoreLayout = true;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            rt.SetParent(canvas.rootCanvas.transform, true);
            rt.SetAsLastSibling();
        }

        Vector3 start = rt.position;
        Vector3 end = target != null
            ? (Vector3)target.position
            : start + new Vector3(420f, 260f, 0f);
        Vector3 mid = Vector3.Lerp(start, end, 0.42f);
        Vector3 control = mid + Vector3.up * arcHeight + Vector3.right * 90f;

        Vector3 startScale = rt.localScale;
        if (startScale.sqrMagnitude < 0.0001f)
            startScale = Vector3.one;
        Quaternion startRot = rt.localRotation;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / popDuration));
            rt.localScale = Vector3.LerpUnclamped(startScale, startScale * popScale, k);
            yield return null;
        }

        t = 0f;
        while (t < flyDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / flyDuration);
            float eased = k * k * (3f - 2f * k);

            rt.position = Bezier(start, control, end, eased);
            rt.localRotation = startRot * Quaternion.Euler(0f, 0f, Mathf.Sin(k * Mathf.PI) * spinAngle);
            rt.localScale = Vector3.LerpUnclamped(startScale * popScale, startScale * endScale, eased);

            const float fadeFrom = 0.48f;
            if (k > fadeFrom)
                canvasGroup.alpha = 1f - (k - fadeFrom) / (1f - fadeFrom);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        onComplete?.Invoke();
    }

    private static Vector3 Bezier(Vector3 a, Vector3 control, Vector3 b, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * control + t * t * b;
    }
}

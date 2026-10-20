using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIPanelAnimator : MonoBehaviour
{
    [SerializeField] private Graphic background;
    [SerializeField] private float fadeDuration = 0.22f;
    [SerializeField] private float bounceStagger = 0.035f;

    private float _backgroundAlpha = 1f;
    private bool _captured;

    private void Awake()
    {
        CaptureBackground();
    }

    private void OnEnable()
    {
        CaptureBackground();
        StopAllCoroutines();
        StartCoroutine(PlayIn());
    }

    private void CaptureBackground()
    {
        if (_captured)
            return;

        if (background == null)
            background = GetComponent<Graphic>();

        if (background != null)
            _backgroundAlpha = background.color.a;

        _captured = true;
    }

    private IEnumerator PlayIn()
    {
        if (background != null)
        {
            Color color = background.color;
            color.a = 0f;
            background.color = color;
            StartCoroutine(FadeBackground(_backgroundAlpha));
        }

        int index = 0;
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
                continue;

            if (background != null && child == background.transform)
                continue;

            if (child.GetComponent<Animator>() != null)
                continue;

            var animator = child.GetComponent<UIElementAnimator>();
            if (animator == null)
                animator = child.gameObject.AddComponent<UIElementAnimator>();

            animator.Mode = UIElementAnimator.AppearMode.Bounce;
            animator.AppearDelay = index * bounceStagger;
            animator.PlayBounce();
            index++;
        }

        if (fadeDuration > 0f)
            yield return null;
    }

    private IEnumerator FadeBackground(float targetAlpha)
    {
        Color color = background.color;
        float from = color.a;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / fadeDuration));
            color.a = Mathf.Lerp(from, targetAlpha, k);
            background.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        background.color = color;
    }
}

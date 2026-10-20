using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIElementAnimator : MonoBehaviour
{
    public enum AppearMode
    {
        Bounce,
        None
    }

    [SerializeField] private AppearMode appearMode = AppearMode.Bounce;
    [SerializeField] private bool punchOnClick = true;
    [SerializeField] private float appearDuration = 0.32f;
    [SerializeField] private float punchScale = 0.9f;
    [SerializeField] private float punchDuration = 0.12f;
    [SerializeField] private float appearDelay;

    private Vector3 _baseScale = Vector3.one;
    private Button _button;
    private Coroutine _routine;
    private bool _baseScaleCaptured;

    public AppearMode Mode
    {
        get => appearMode;
        set => appearMode = value;
    }

    public bool PunchOnClick
    {
        get => punchOnClick;
        set => punchOnClick = value;
    }

    public float AppearDelay
    {
        get => appearDelay;
        set => appearDelay = value;
    }

    private void Awake()
    {
        CaptureBaseScale();
        _button = GetComponent<Button>();
        if (_button != null && punchOnClick)
            _button.onClick.AddListener(PlayPunch);
    }

    private void OnEnable()
    {
        CaptureBaseScale();
        var parentPanel = GetComponentInParent<UIPanelAnimator>(true);
        if (parentPanel != null && parentPanel.gameObject != gameObject)
            return;

        if (appearMode == AppearMode.Bounce)
            PlayBounce();
        else
            transform.localScale = _baseScale;
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        transform.localScale = _baseScale;
    }

    public void PlayBounce()
    {
        if (!isActiveAndEnabled)
            return;

        Restart(BounceRoutine());
    }

    public void PlayPunch()
    {
        if (!isActiveAndEnabled)
            return;

        Restart(PunchRoutine());
    }

    private void CaptureBaseScale()
    {
        if (_baseScaleCaptured)
            return;

        _baseScale = transform.localScale;
        if (_baseScale.sqrMagnitude < 0.0001f)
            _baseScale = Vector3.one;

        _baseScaleCaptured = true;
    }

    private void Restart(IEnumerator routine)
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(routine);
    }

    private IEnumerator BounceRoutine()
    {
        transform.localScale = _baseScale * 0.55f;
        if (appearDelay > 0f)
            yield return Wait(appearDelay);

        yield return ScaleTo(_baseScale * 1.12f, appearDuration * 0.55f);
        yield return ScaleTo(_baseScale, appearDuration * 0.45f);
        transform.localScale = _baseScale;
        _routine = null;
    }

    private IEnumerator PunchRoutine()
    {
        yield return ScaleTo(_baseScale * punchScale, punchDuration * 0.4f);
        yield return ScaleTo(_baseScale, punchDuration * 0.6f);
        transform.localScale = _baseScale;
        _routine = null;
    }

    private IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 from = transform.localScale;
        if (duration <= 0f)
        {
            transform.localScale = target;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            transform.localScale = Vector3.LerpUnclamped(from, target, k);
            yield return null;
        }

        transform.localScale = target;
    }

    private static IEnumerator Wait(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}

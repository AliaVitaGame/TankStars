using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class CellProportionalLayout : MonoBehaviour
{
    [SerializeField] private Vector2 designSize = new Vector2(370f, 548f);

    [Serializable]
    private class ChildLayout
    {
        public string path;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector3 localScale;
    }

    [SerializeField] private List<ChildLayout> children = new List<ChildLayout>();

    private RectTransform _rect;
    private bool _captured;

    public Vector2 DesignSize
    {
        get => designSize;
        set => designSize = value;
    }

    private void OnEnable()
    {
        _rect = transform as RectTransform;
        EnsureCaptured();
        Apply();
    }

    private void OnRectTransformDimensionsChange()
    {
        Apply();
    }

    private void EnsureCaptured()
    {
        if (_captured || children.Count > 0)
        {
            _captured = true;
            return;
        }

        Capture();
        _captured = true;
    }

    public void Capture()
    {
        children.Clear();
        CaptureRecursive(transform, string.Empty);
    }

    private void CaptureRecursive(Transform parent, string parentPath)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i) as RectTransform;
            if (child == null)
                continue;

            string path = string.IsNullOrEmpty(parentPath) ? child.name : parentPath + "/" + child.name;
            children.Add(new ChildLayout
            {
                path = path,
                anchorMin = child.anchorMin,
                anchorMax = child.anchorMax,
                pivot = child.pivot,
                anchoredPosition = child.anchoredPosition,
                sizeDelta = child.sizeDelta,
                localScale = child.localScale
            });

            CaptureRecursive(child, path);
        }
    }

    public void Apply()
    {
        if (_rect == null)
            _rect = transform as RectTransform;

        if (_rect == null || designSize.x < 1f || designSize.y < 1f)
            return;

        EnsureCaptured();

        float width = _rect.rect.width;
        float height = _rect.rect.height;
        if (width <= 1f || height <= 1f)
            return;

        float sx = width / designSize.x;
        float sy = height / designSize.y;

        for (int i = 0; i < children.Count; i++)
        {
            var layout = children[i];
            var child = FindChild(layout.path);
            if (child == null)
                continue;

            child.anchorMin = layout.anchorMin;
            child.anchorMax = layout.anchorMax;
            child.pivot = layout.pivot;
            child.anchoredPosition = new Vector2(layout.anchoredPosition.x * sx, layout.anchoredPosition.y * sy);
            child.sizeDelta = new Vector2(layout.sizeDelta.x * sx, layout.sizeDelta.y * sy);
            child.localScale = layout.localScale;
        }
    }

    private RectTransform FindChild(string path)
    {
        var current = transform;
        string[] parts = path.Split('/');
        for (int i = 0; i < parts.Length; i++)
        {
            Transform next = current.Find(parts[i]);
            if (next == null)
                return null;

            current = next;
        }

        return current as RectTransform;
    }
}

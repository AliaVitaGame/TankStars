using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class AdaptiveGridLayout : MonoBehaviour
{
    [SerializeField] private int columns = 2;
    [SerializeField] private float aspectRatio = 0.675f;
    [SerializeField] private float minCellWidth = 160f;
    [SerializeField] private float maxCellWidth = 480f;

    private GridLayoutGroup _grid;
    private RectTransform _rect;
    private float _lastWidth = -1f;

    private void OnEnable()
    {
        Cache();
        Refresh();
    }

    private void OnRectTransformDimensionsChange()
    {
        Refresh();
    }

    private void LateUpdate()
    {
        if (_rect == null)
            return;

        if (!Mathf.Approximately(_rect.rect.width, _lastWidth))
            Refresh();
    }

    private void Cache()
    {
        _grid = GetComponent<GridLayoutGroup>();
        _rect = transform as RectTransform;
    }

    public void Refresh()
    {
        if (_grid == null || _rect == null)
            Cache();

        if (_grid == null || _rect == null)
            return;

        float width = _rect.rect.width;
        if (width <= 1f)
            return;

        _lastWidth = width;

        float inner = width - _grid.padding.left - _grid.padding.right;
        int cols = Mathf.Max(1, columns);
        float spacing = _grid.spacing.x;
        float cellWidth = (inner - spacing * (cols - 1)) / cols;

        while (cols > 1 && cellWidth < minCellWidth)
        {
            cols--;
            cellWidth = (inner - spacing * (cols - 1)) / cols;
        }

        cellWidth = Mathf.Clamp(cellWidth, minCellWidth, maxCellWidth);
        float ratio = aspectRatio > 0.01f ? aspectRatio : 0.675f;
        float cellHeight = cellWidth / ratio;

        _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _grid.constraintCount = cols;
        _grid.cellSize = new Vector2(cellWidth, cellHeight);
        _grid.childAlignment = TextAnchor.UpperCenter;
    }
}

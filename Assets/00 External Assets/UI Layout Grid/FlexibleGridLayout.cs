using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterfaceGridLayout
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        public enum SortEnum
        {
            Rows,
            Columns
        }

        public enum SortVerticalyEnum
        {
            TopToBottom,
            BottomToTop
        }

        public enum SortHorizontalyEnum
        {
            LeftToRight,
            RightToLeft
        }

        public FitType fitType = FitType.Uniform;
        public int rows;
        public int columns;
        public Vector2 cellSize;
        public Vector2 spacing;

        public bool fitX = true;
        public bool fitY = true;
        public bool keepCellsSquare;

        public bool autoResizeWidth = true;
        public bool autoResizeHeight = true;

        public SortEnum fillFirst = SortEnum.Rows;
        public SortVerticalyEnum sortVertically = SortVerticalyEnum.TopToBottom;
        public SortHorizontalyEnum sortHorizontally = SortHorizontalyEnum.LeftToRight;

        private float _calculatedWidth;
        private float _calculatedHeight;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalculateGrid();
        }

        public override void CalculateLayoutInputVertical()
        {
            SetLayoutInputForAxis(_calculatedWidth, _calculatedWidth, -1, 0);
            SetLayoutInputForAxis(_calculatedHeight, _calculatedHeight, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetCells();
        }

        public override void SetLayoutVertical() { }

        private void CalculateGrid()
        {
            var childCount = rectChildren.Count;

            if (childCount == 0)
                return;

            if (fitType == FitType.Uniform || fitType == FitType.Width || fitType == FitType.Height)
            {
                var sqrt = Mathf.Sqrt(childCount);
                rows = columns = Mathf.CeilToInt(sqrt);
            }

            if (fitType == FitType.Width || fitType == FitType.FixedColumns)
                rows = Mathf.CeilToInt(childCount / (float)columns);

            if (fitType == FitType.Height || fitType == FitType.FixedRows)
                columns = Mathf.CeilToInt(childCount / (float)rows);

            var parentWidth = rectTransform.rect.width - padding.left - padding.right;
            var parentHeight = rectTransform.rect.height - padding.top - padding.bottom;

            var cellWidth = parentWidth / columns - (spacing.x * (columns - 1) / columns);
            var cellHeight = parentHeight / rows - (spacing.y * (rows - 1) / rows);

            if (fitX)
                cellSize.x = cellWidth;

            if (fitY)
                cellSize.y = cellHeight;

            if (keepCellsSquare)
                cellSize.y = cellSize.x;

            _calculatedWidth =
                padding.left + padding.right +
                columns * cellSize.x +
                (columns - 1) * spacing.x;

            _calculatedHeight =
                padding.top + padding.bottom +
                rows * cellSize.y +
                (rows - 1) * spacing.y;

            if (!autoResizeWidth)
                _calculatedWidth = rectTransform.rect.width;

            if (!autoResizeHeight)
                _calculatedHeight = rectTransform.rect.height;
        }

        private void SetCells()
        {
            var sortedChildren = GetSortedChildren();

            for (var i = 0; i < sortedChildren.Count; i++)
            {
                int rowIndex;
                int columnIndex;

                if (fillFirst == SortEnum.Rows)
                {
                    rowIndex = i / columns;
                    columnIndex = i % columns;
                }
                else
                {
                    columnIndex = i / rows;
                    rowIndex = i % rows;
                }

                var item = sortedChildren[i];

                var xPos = padding.left + (cellSize.x + spacing.x) * columnIndex;
                var yPos = padding.top + (cellSize.y + spacing.y) * rowIndex;

                var totalContentWidth = columns * cellSize.x + (columns - 1) * spacing.x;
                var totalContentHeight = rows * cellSize.y + (rows - 1) * spacing.y;

                var offsetX = 0f;
                var offsetY = 0f;

                switch (childAlignment)
                {
                    case TextAnchor.MiddleCenter:
                        offsetX = (rectTransform.rect.width - padding.left - padding.right - totalContentWidth) / 2;
                        offsetY = (rectTransform.rect.height - padding.top - padding.bottom - totalContentHeight) / 2;
                        break;

                    case TextAnchor.UpperCenter:
                        offsetX = (rectTransform.rect.width - padding.left - padding.right - totalContentWidth) / 2;
                        break;

                    case TextAnchor.LowerCenter:
                        offsetX = (rectTransform.rect.width - padding.left - padding.right - totalContentWidth) / 2;
                        offsetY = rectTransform.rect.height - padding.bottom - totalContentHeight;
                        break;

                    case TextAnchor.MiddleRight:
                        offsetX = rectTransform.rect.width - padding.right - totalContentWidth;
                        offsetY = (rectTransform.rect.height - padding.top - padding.bottom - totalContentHeight) / 2;
                        break;

                    case TextAnchor.UpperRight:
                        offsetX = rectTransform.rect.width - padding.right - totalContentWidth;
                        break;

                    case TextAnchor.LowerRight:
                        offsetX = rectTransform.rect.width - padding.right - totalContentWidth;
                        offsetY = rectTransform.rect.height - padding.bottom - totalContentHeight;
                        break;
                }

                xPos += offsetX;
                yPos += offsetY;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        private List<RectTransform> GetSortedChildren()
        {
            var sorted = new List<RectTransform>(rectChildren.Count);

            if (fillFirst == SortEnum.Rows)
            {
                for (var row = 0; row < rows; row++)
                {
                    for (var col = 0; col < columns; col++)
                    {
                        var r = sortVertically == SortVerticalyEnum.TopToBottom ? row : rows - 1 - row;
                        var c = sortHorizontally == SortHorizontalyEnum.LeftToRight ? col : columns - 1 - col;

                        var index = r * columns + c;

                        if (index < rectChildren.Count)
                            sorted.Add(rectChildren[index]);
                    }
                }
            }
            else
            {
                for (var col = 0; col < columns; col++)
                {
                    for (var row = 0; row < rows; row++)
                    {
                        var c = sortHorizontally == SortHorizontalyEnum.LeftToRight ? col : columns - 1 - col;
                        var r = sortVertically == SortVerticalyEnum.TopToBottom ? row : rows - 1 - row;

                        var index = r + c * rows;

                        if (index < rectChildren.Count)
                            sorted.Add(rectChildren[index]);
                    }
                }
            }

            return sorted;
        }
    }
}
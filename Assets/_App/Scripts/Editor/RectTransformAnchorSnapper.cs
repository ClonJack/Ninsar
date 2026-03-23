#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Ninsar.Editor
{
    /// <summary>
    /// Tool for automatically setting anchors to UI Element size
    /// Doing such things helpful when you need to
    /// Transform from pixel to relative size
    /// This functionality is available from context menu of Rect Transform
    /// </summary>
    public static class RectTransformAnchorSnapper
    {
        [MenuItem("CONTEXT/RectTransform/Snap Anchors/Fully")]
        private static void SnapAnchorsFully(MenuCommand command)
        {
            var rect = (RectTransform)command.context;

            var parent = rect.parent as RectTransform;

            if (parent == null)
            {
                Debug.LogWarning($"Unable to snap anchors for {rect.name}");
                return;
            }

            SnapAnchorsVertically(command);
            SnapAnchorsHorizontally(command);
        }

        [MenuItem("CONTEXT/RectTransform/Snap Anchors/Vertically")]
        private static void SnapAnchorsVertically(MenuCommand command)
        {
            var rect = (RectTransform)command.context;

            var parent = rect.parent as RectTransform;

            if (parent == null)
            {
                Debug.LogWarning($"Unable to snap anchors for {rect.name}");
                return;
            }

            var corners = new Vector3[4];
            rect.GetLocalCorners(corners);

            var parentRect = parent.rect;
            var height = parentRect.height;

            var centerY = height * parent.pivot.y;

            var localPosition = rect.localPosition;

            var minAnchorY =
                (centerY + localPosition.y + corners[0].y) / height;
            var maxAnchorY =
                (centerY + localPosition.y + corners[2].y) / height;

            var anchorMin = new Vector2(rect.anchorMin.x, minAnchorY);
            var anchorMax = new Vector2(rect.anchorMax.x, maxAnchorY);

            Undo.RecordObject(rect, "Snap anchors vertically");

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;

            rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
            rect.offsetMax = new Vector2(rect.offsetMax.x, 0);
        }

        [MenuItem("CONTEXT/RectTransform/Snap Anchors/Horizontally")]
        private static void SnapAnchorsHorizontally(MenuCommand command)
        {
            var rect = (RectTransform)command.context;

            var parent = rect.parent as RectTransform;

            if (parent == null)
            {
                Debug.LogWarning($"Unable to snap anchors for {rect.name}");
                return;
            }

            var corners = new Vector3[4];
            rect.GetLocalCorners(corners);

            var parentRect = parent.rect;
            var width = parentRect.width;

            var centerX = width * parent.pivot.x;

            var localPosition = rect.localPosition;

            var minAnchorX =
                (centerX + localPosition.x + corners[0].x) / width;
            var maxAnchorX =
                (centerX + localPosition.x + corners[2].x) / width;

            var anchorMin = new Vector2(minAnchorX, rect.anchorMin.y);
            var anchorMax = new Vector2(maxAnchorX, rect.anchorMax.y);

            Undo.RecordObject(rect, "Snap anchors vertically");

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;

            rect.offsetMin = new Vector2(0, rect.offsetMin.y);
            rect.offsetMax = new Vector2(0, rect.offsetMax.y);
        }
    }
}

#endif
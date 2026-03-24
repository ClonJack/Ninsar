using System.Linq;
using UnityEngine;

namespace UnrealTeam.GamePlay.Extension
{
    public static class GameObjectExtensions
    {
        public static void DestroyAllChildren(this Transform parentTransform, bool destroyImmediate = false)
        {
            if (parentTransform == null || parentTransform.childCount == 0) return;

            parentTransform.DestroyChildren(0, parentTransform.childCount, destroyImmediate);
        }

        public static void DestroyChildren(this Transform parentTransform, int fromId, int childCount,
            bool destroyImmediate = false)
        {
            Enumerable.Range(fromId, childCount)
                .Select(i => parentTransform.GetChild(i).gameObject)
                .ToList()
                .ForEach(destroyImmediate ? Object.DestroyImmediate : Object.Destroy);
        }
    }
}
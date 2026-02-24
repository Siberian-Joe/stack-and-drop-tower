using Game.UI.BottomBar.Contracts;
using UnityEngine;

namespace Game.UI.BottomBar.Runtime
{
    public static class TowerPlacementGeometry
    {
        public static bool TryComputeTarget(
            in TowerPlacementRuleContext context,
            out Vector2 target)
        {
            var area = context.TowerRoot.rect;

            if (context.Stack.Count == 0)
            {
                target = ClampPivotInsideRect(
                    context.DesiredPivotPos,
                    area,
                    context.CubeWidth,
                    context.CubeHeight,
                    context.CubePivot);

                return true;
            }

            var top = context.Stack.Top;

            var xMin = top.Target.x - top.Width * context.MaxXOffsetFactor;
            var xMax = top.Target.x + top.Width * context.MaxXOffsetFactor;

            var x = Mathf.Clamp(context.DesiredPivotPos.x, xMin, xMax);
            var y = top.Target.y + context.CubeHeight;

            target = ClampPivotInsideRect(
                new Vector2(x, y),
                area,
                context.CubeWidth,
                context.CubeHeight,
                context.CubePivot);

            // Если не влез по высоте
            if (!Mathf.Approximately(target.y, y))
                return false;

            return true;
        }

        private static Vector2 ClampPivotInsideRect(Vector2 pivotPos, Rect area, float w, float h, Vector2 pivot)
        {
            var minX = area.xMin + w * pivot.x;
            var maxX = area.xMax - w * (1f - pivot.x);

            var minY = area.yMin + h * pivot.y;
            var maxY = area.yMax - h * (1f - pivot.y);

            return new Vector2(
                Mathf.Clamp(pivotPos.x, minX, maxX),
                Mathf.Clamp(pivotPos.y, minY, maxY));
        }
    }
}
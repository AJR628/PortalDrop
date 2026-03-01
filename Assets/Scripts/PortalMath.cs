using UnityEngine;

/// <summary>
/// Pure math helpers for PortalDrop geometry and teleport behavior.
/// </summary>
public static class PortalMath
{
    public static Vector2 RotateVelocity(Vector2 incomingVelocity, Vector2 entryNormal, Vector2 exitNormal)
    {
        if (entryNormal == Vector2.zero || exitNormal == Vector2.zero)
            return incomingVelocity;

        float angle = Vector2.SignedAngle(entryNormal, exitNormal);
        return Quaternion.Euler(0f, 0f, angle) * incomingVelocity;
    }

    public static Vector2 ComputeSpawnPosition(float ballRadius)
    {
        float spawnY = PortalDropSpec.ArenaTop - (ballRadius + PortalDropSpec.BallSpawnMargin);
        return new Vector2(0f, spawnY);
    }

    public static Vector2 ComputeTeleportPosition(Vector2 exitPosition, Vector2 exitNormal, float ballRadius)
    {
        if (exitNormal == Vector2.zero)
            return exitPosition;

        float offset = ballRadius + PortalDropSpec.TeleportExitOffset;
        return exitPosition + exitNormal.normalized * offset;
    }

    public static Vector2 SnapToPerimeter(Vector2 worldPoint)
    {
        float distTop = Mathf.Abs(worldPoint.y - PortalDropSpec.ArenaTop);
        float distBottom = Mathf.Abs(worldPoint.y - PortalDropSpec.ArenaBottom);
        float distLeft = Mathf.Abs(worldPoint.x - PortalDropSpec.ArenaLeft);
        float distRight = Mathf.Abs(worldPoint.x - PortalDropSpec.ArenaRight);

        float clampMinX = PortalDropSpec.ArenaLeft + PortalDropSpec.PortalSnapMargin;
        float clampMaxX = PortalDropSpec.ArenaRight - PortalDropSpec.PortalSnapMargin;
        float clampMinY = PortalDropSpec.ArenaBottom + PortalDropSpec.PortalSnapMargin;
        float clampMaxY = PortalDropSpec.ArenaTop - PortalDropSpec.PortalSnapMargin;

        if (distTop <= distBottom && distTop <= distLeft && distTop <= distRight)
            return new Vector2(Mathf.Clamp(worldPoint.x, clampMinX, clampMaxX), PortalDropSpec.ArenaTop);

        if (distBottom <= distLeft && distBottom <= distRight)
            return new Vector2(Mathf.Clamp(worldPoint.x, clampMinX, clampMaxX), PortalDropSpec.ArenaBottom);

        if (distLeft <= distRight)
            return new Vector2(PortalDropSpec.ArenaLeft, Mathf.Clamp(worldPoint.y, clampMinY, clampMaxY));

        return new Vector2(PortalDropSpec.ArenaRight, Mathf.Clamp(worldPoint.y, clampMinY, clampMaxY));
    }

    public static Portal.PortalSide DetermineSideFromBoundaryPosition(Vector2 position)
    {
        if (position.y >= PortalDropSpec.ArenaTop - PortalDropSpec.SideDetectionEpsilon)
            return Portal.PortalSide.Top;

        if (position.y <= PortalDropSpec.ArenaBottom + PortalDropSpec.SideDetectionEpsilon)
            return Portal.PortalSide.Bottom;

        if (position.x <= PortalDropSpec.ArenaLeft + PortalDropSpec.SideDetectionEpsilon)
            return Portal.PortalSide.Left;

        return Portal.PortalSide.Right;
    }
}

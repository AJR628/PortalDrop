using UnityEngine;

/// <summary>
/// Shared Wave 1 constants for geometry, timing, spawn, and default scene layout.
/// Keep gameplay scripts reading from here to avoid drift.
/// </summary>
public static class PortalDropSpec
{
    public const float CameraOrthographicSize = 8.5f;
    public const float CameraZPosition = -10f;

    public const float ArenaLeft = -5f;
    public const float ArenaRight = 5f;
    public const float ArenaBottom = -8f;
    public const float ArenaTop = 8f;

    public const float WallThickness = 0.2f;
    public const float SideDetectionEpsilon = 0.01f;

    public const float PortalHalfWidth = 0.5f;
    public const float CornerMargin = 0.75f;
    public const float PortalSnapMargin = PortalHalfWidth + CornerMargin;

    public const float EntryPortalInset = 0.25f;
    public const float BallSpawnMargin = 0.10f;
    public const float FixedDeltaTime = 1f / 60f;
    public const float TeleportCooldownSeconds = 0.15f;
    public const float TeleportExitOffset = 0.05f;
    public const float DefaultBallRadius = 0.25f;

    public static readonly Vector2 EntryPortalPosition = new Vector2(0f, ArenaBottom + EntryPortalInset);
    public static readonly Vector2 DefaultExitPortalPosition = new Vector2(ArenaRight, 0f);
    public static readonly Vector2 EntryPortalSize = new Vector2(1.75f, 0.5f);
    public static readonly Vector2 ExitPortalSize = new Vector2(1.0f, 0.5f);

    public const Portal.PortalSide EntryPortalSide = Portal.PortalSide.Bottom;
    public const Portal.PortalSide DefaultExitPortalSide = Portal.PortalSide.Right;
}

using UnityEngine;

/// <summary>
/// Shared MVP constants for geometry, timing, and spawn defaults.
/// Keep gameplay scripts reading from here to avoid drift.
/// </summary>
public static class PortalDropSpec
{
    public const float ArenaLeft = -5f;
    public const float ArenaRight = 5f;
    public const float ArenaBottom = -8f;
    public const float ArenaTop = 8f;

    public const float PortalHalfWidth = 0.5f;
    public const float CornerMargin = 0.75f;
    public const float PortalSnapMargin = PortalHalfWidth + CornerMargin;

    public const float FixedDeltaTime = 1f / 60f;
    public const float TeleportCooldownSeconds = 0.15f;
    public const float TeleportExitOffset = 0.05f;
    public const float DefaultBallRadius = 0.25f;

    public static readonly Vector2 BallSpawnPosition = new Vector2(0f, 7.4f);
}

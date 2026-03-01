using NUnit.Framework;
using UnityEngine;

public class PortalMathTests
{
    [Test]
    public void RotateVelocity_PreservesMagnitude_WhenNormalsDiffer()
    {
        Vector2 incomingVelocity = new Vector2(0f, -3f);
        Vector2 rotatedVelocity = PortalMath.RotateVelocity(incomingVelocity, Vector2.up, Vector2.left);

        Assert.That(rotatedVelocity.magnitude, Is.EqualTo(incomingVelocity.magnitude).Within(0.0001f));
        Assert.That(rotatedVelocity.x, Is.EqualTo(3f).Within(0.0001f));
        Assert.That(rotatedVelocity.y, Is.EqualTo(0f).Within(0.0001f));
    }

    [Test]
    public void RotateVelocity_ReturnsOriginalVector_WhenNormalsMatch()
    {
        Vector2 incomingVelocity = new Vector2(2f, -1f);
        Vector2 rotatedVelocity = PortalMath.RotateVelocity(incomingVelocity, Vector2.up, Vector2.up);

        Assert.That(rotatedVelocity.x, Is.EqualTo(incomingVelocity.x).Within(0.0001f));
        Assert.That(rotatedVelocity.y, Is.EqualTo(incomingVelocity.y).Within(0.0001f));
    }

    [Test]
    public void SnapToPerimeter_ClampsTopEdgeWithCornerMargin()
    {
        Vector2 snapped = PortalMath.SnapToPerimeter(new Vector2(4.8f, 7.6f));

        Assert.That(snapped.x, Is.EqualTo(3.75f).Within(0.0001f));
        Assert.That(snapped.y, Is.EqualTo(PortalDropSpec.ArenaTop).Within(0.0001f));
    }

    [Test]
    public void SnapToPerimeter_UsesConfiguredTieBreakOrder()
    {
        Vector2 snapped = PortalMath.SnapToPerimeter(new Vector2(100f, 0f));

        Assert.That(snapped.x, Is.EqualTo(3.75f).Within(0.0001f));
        Assert.That(snapped.y, Is.EqualTo(PortalDropSpec.ArenaTop).Within(0.0001f));
    }

    [Test]
    public void ComputeSpawnPosition_UsesBallRadiusAndMargin()
    {
        Vector2 spawnPosition = PortalMath.ComputeSpawnPosition(0.25f);

        Assert.That(spawnPosition.x, Is.EqualTo(0f).Within(0.0001f));
        Assert.That(spawnPosition.y, Is.EqualTo(7.65f).Within(0.0001f));
    }

    [Test]
    public void ComputeTeleportPosition_UsesExitNormalAndOffset()
    {
        Vector2 teleportPosition = PortalMath.ComputeTeleportPosition(new Vector2(5f, 0f), Vector2.left, 0.25f);

        Assert.That(teleportPosition.x, Is.EqualTo(4.7f).Within(0.0001f));
        Assert.That(teleportPosition.y, Is.EqualTo(0f).Within(0.0001f));
    }
}

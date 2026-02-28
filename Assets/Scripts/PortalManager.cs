using UnityEngine;

/// <summary>
/// Attach to PortalEntry. Detects ball entering entry trigger, teleports to exit with rotated velocity.
/// Exit portal does not teleport.
/// </summary>
[RequireComponent(typeof(Portal))]
public class PortalManager : MonoBehaviour
{
    [SerializeField] private Transform exitPortalTransform;
    [SerializeField] private Portal exitPortalPortal;
    [SerializeField] private BallController ballController;

    private Portal entryPortal;

    private const float TeleportCooldownSeconds = 0.15f;

    private void Awake()
    {
        entryPortal = GetComponent<Portal>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ballController == null || exitPortalTransform == null || exitPortalPortal == null)
            return;

        BallController ball = other.GetComponent<BallController>();
        if (ball == null || ball != ballController)
            return;

        if (!ballController.CanTeleport)
            return;

        PerformTeleport(ball);
    }

    private void PerformTeleport(BallController ball)
    {
        Vector2 entryNormal = entryPortal.InwardNormal;
        Vector2 exitNormal = exitPortalPortal.InwardNormal;
        Vector2 vIn = ball.Rigidbody.velocity;

        float angle = Vector2.SignedAngle(entryNormal, exitNormal);
        Vector2 vOut = Quaternion.Euler(0f, 0f, angle) * vIn;

        float offset = ball.BallRadius + 0.05f;
        Vector2 exitPos = exitPortalTransform.position;
        Vector2 newPos = exitPos + exitNormal * offset;

        ball.transform.position = new Vector3(newPos.x, newPos.y, ball.transform.position.z);
        ball.Rigidbody.velocity = vOut;
        ball.SetTeleportCooldown(TeleportCooldownSeconds);
    }
}

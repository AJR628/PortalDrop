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
    [SerializeField] private GameManager gameManager;

    private Portal entryPortal;
    public Transform ExitPortalTransform => exitPortalTransform;
    public Portal ExitPortalPortal => exitPortalPortal;
    public BallController BallController => ballController;
    public GameManager GameManager => gameManager;

    private void Awake()
    {
        entryPortal = GetComponent<Portal>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameManager == null || gameManager.State != GameState.Running)
            return;

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
        Vector2 vIn = ball.Rigidbody.linearVelocity;
        Vector2 vOut = PortalMath.RotateVelocity(vIn, entryNormal, exitNormal);
        Vector2 newPos = PortalMath.ComputeTeleportPosition(exitPortalTransform.position, exitNormal, ball.BallRadius);

        ball.transform.position = new Vector3(newPos.x, newPos.y, ball.transform.position.z);
        ball.Rigidbody.linearVelocity = vOut;
        ball.SetTeleportCooldown(PortalDropSpec.TeleportCooldownSeconds);
    }
}

using UnityEngine;

/// <summary>
/// Ball physics and teleport cooldown. Exposes CanTeleport and SetTeleportCooldown.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D circleCollider;

    private float teleportCooldownRemaining;

    public bool CanTeleport => teleportCooldownRemaining <= 0f;

    /// <summary>
    /// Ball radius for spawn and teleport offset: radius * max(lossyScale.x, lossyScale.y)
    /// </summary>
    public float BallRadius
    {
        get
        {
            if (circleCollider == null) return PortalDropSpec.DefaultBallRadius;
            float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            return circleCollider.radius * scale;
        }
    }

    public Rigidbody2D Rigidbody => rb;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (circleCollider == null) circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (teleportCooldownRemaining > 0f)
            teleportCooldownRemaining -= Time.deltaTime;
    }

    public void SetTeleportCooldown(float seconds)
    {
        teleportCooldownRemaining = seconds;
    }

    public void Freeze()
    {
        if (rb != null)
        {
            rb.simulated = false;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void Unfreeze()
    {
        if (rb != null)
            rb.simulated = true;
    }
}

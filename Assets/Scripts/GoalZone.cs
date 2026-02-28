using UnityEngine;

/// <summary>
/// Stub for future win condition. Logs ball hits for now.
/// </summary>
public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BallController>() != null)
            Debug.Log("[GoalZone] Ball entered goal.");
    }
}

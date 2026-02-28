using UnityEngine;

/// <summary>
/// Portal data component. Holds side and inward normal for teleport math.
/// </summary>
public class Portal : MonoBehaviour
{
    public enum PortalSide
    {
        Top,
        Bottom,
        Left,
        Right
    }

    [SerializeField] private PortalSide side;
    [SerializeField] private Vector2 inwardNormal;

    public PortalSide Side => side;
    public Vector2 InwardNormal => inwardNormal;

    public void SetSide(PortalSide newSide)
    {
        side = newSide;
        inwardNormal = GetNormalForSide(newSide);
    }

    public static Vector2 GetNormalForSide(PortalSide s)
    {
        return s switch
        {
            PortalSide.Top => Vector2.down,
            PortalSide.Bottom => Vector2.up,
            PortalSide.Left => Vector2.right,
            PortalSide.Right => Vector2.left,
            _ => Vector2.zero
        };
    }

    private void OnValidate()
    {
        inwardNormal = GetNormalForSide(side);
    }
}

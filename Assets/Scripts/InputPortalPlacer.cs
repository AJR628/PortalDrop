using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Reads tap/click in PlacingPortal or Ready, snaps to arena perimeter, positions PortalExit.
/// Ignores input when pointer is over UI.
/// </summary>
public class InputPortalPlacer : MonoBehaviour
{
    [SerializeField] private Transform exitPortalTransform;
    [SerializeField] private Portal exitPortalPortal;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera mainCamera;

    private Camera Cam => mainCamera != null ? mainCamera : Camera.main;

    private void Update()
    {
        if (gameManager == null ||
            (gameManager.State != GameManager.GameState.PlacingPortal &&
             gameManager.State != GameManager.GameState.Ready))
            return;

        if (exitPortalTransform == null || exitPortalPortal == null || Cam == null)
            return;

        Vector2? inputPoint = GetInputPoint();
        if (!inputPoint.HasValue)
            return;

        Vector2 worldPoint = Cam.ScreenToWorldPoint(new Vector3(inputPoint.Value.x, inputPoint.Value.y, -Cam.transform.position.z));
        Vector2 snapped = SnapToPerimeter(worldPoint);
        PlaceExitPortal(snapped);
    }

    private Vector2? GetInputPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return null;
            return Input.mousePosition;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return null;
                return touch.position;
            }
        }

        return null;
    }

    private Vector2 SnapToPerimeter(Vector2 worldPoint)
    {
        float distTop = Mathf.Abs(worldPoint.y - PortalDropSpec.ArenaTop);
        float distBottom = Mathf.Abs(worldPoint.y - PortalDropSpec.ArenaBottom);
        float distLeft = Mathf.Abs(worldPoint.x - PortalDropSpec.ArenaLeft);
        float distRight = Mathf.Abs(worldPoint.x - PortalDropSpec.ArenaRight);

        float minDist = Mathf.Min(distTop, distBottom, distLeft, distRight);

        float clampMin = PortalDropSpec.ArenaLeft + PortalDropSpec.PortalSnapMargin;
        float clampMax = PortalDropSpec.ArenaRight - PortalDropSpec.PortalSnapMargin;
        float clampYMin = PortalDropSpec.ArenaBottom + PortalDropSpec.PortalSnapMargin;
        float clampYMax = PortalDropSpec.ArenaTop - PortalDropSpec.PortalSnapMargin;

        if (minDist == distTop)
        {
            float x = Mathf.Clamp(worldPoint.x, clampMin, clampMax);
            return new Vector2(x, PortalDropSpec.ArenaTop);
        }
        if (minDist == distBottom)
        {
            float x = Mathf.Clamp(worldPoint.x, clampMin, clampMax);
            return new Vector2(x, PortalDropSpec.ArenaBottom);
        }
        if (minDist == distLeft)
        {
            float y = Mathf.Clamp(worldPoint.y, clampYMin, clampYMax);
            return new Vector2(PortalDropSpec.ArenaLeft, y);
        }
        float yRight = Mathf.Clamp(worldPoint.y, clampYMin, clampYMax);
        return new Vector2(PortalDropSpec.ArenaRight, yRight);
    }

    private void PlaceExitPortal(Vector2 position)
    {
        exitPortalTransform.position = new Vector3(position.x, position.y, exitPortalTransform.position.z);

        if (position.y >= PortalDropSpec.ArenaTop - 0.01f)
            exitPortalPortal.SetSide(Portal.PortalSide.Top);
        else if (position.y <= PortalDropSpec.ArenaBottom + 0.01f)
            exitPortalPortal.SetSide(Portal.PortalSide.Bottom);
        else if (position.x <= PortalDropSpec.ArenaLeft + 0.01f)
            exitPortalPortal.SetSide(Portal.PortalSide.Left);
        else
            exitPortalPortal.SetSide(Portal.PortalSide.Right);
    }
}

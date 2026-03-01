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
    public Transform ExitPortalTransform => exitPortalTransform;
    public Portal ExitPortalPortal => exitPortalPortal;
    public GameManager GameManager => gameManager;
    public Camera MainCamera => mainCamera;

    private void Update()
    {
        if (gameManager == null || !gameManager.CanPlaceExitPortal)
            return;

        if (exitPortalTransform == null || exitPortalPortal == null || Cam == null)
            return;

        Vector2? inputPoint = GetInputPoint();
        if (!inputPoint.HasValue)
            return;

        Vector2 worldPoint = Cam.ScreenToWorldPoint(new Vector3(inputPoint.Value.x, inputPoint.Value.y, -Cam.transform.position.z));
        Vector2 snapped = PortalMath.SnapToPerimeter(worldPoint);
        PlaceExitPortal(snapped);
        gameManager.NotifyExitPortalPlaced();
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

    private void PlaceExitPortal(Vector2 position)
    {
        exitPortalTransform.position = new Vector3(position.x, position.y, exitPortalTransform.position.z);
        exitPortalPortal.SetSide(PortalMath.DetermineSideFromBoundaryPosition(position));
    }
}

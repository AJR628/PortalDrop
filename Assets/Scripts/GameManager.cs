using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// State machine: PlacingPortal, Ready, Running. Start/Reset, ball spawn, fixedDeltaTime.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private BallController ball;
    [SerializeField] private Transform portalEntry;
    [SerializeField] private Transform portalExit;
    [SerializeField] private Button startButton;
    [SerializeField] private Button resetButton;

    public GameState State { get; private set; } = GameState.PlacingPortal;
    public BallController Ball => ball;
    public Transform PortalEntry => portalEntry;
    public Transform PortalExit => portalExit;
    public Button StartButton => startButton;
    public Button ResetButton => resetButton;
    public bool HasPlacedExitPortal { get; private set; }
    public bool CanPlaceExitPortal => State == GameState.PlacingPortal || State == GameState.Ready;

    private Portal exitPortalComponent;

    private void Awake()
    {
        Time.fixedDeltaTime = PortalDropSpec.FixedDeltaTime;
        exitPortalComponent = portalExit != null ? portalExit.GetComponent<Portal>() : null;

        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetPressed);
    }

    private void Start()
    {
        ResetToPlacement();
    }

    private void OnDestroy()
    {
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartPressed);
        if (resetButton != null)
            resetButton.onClick.RemoveListener(OnResetPressed);
    }

    public void OnStartPressed()
    {
        if (State != GameState.Ready || !HasPlacedExitPortal)
            return;

        State = GameState.Running;
        if (ball != null)
            ball.Unfreeze();
        UpdateButtonState();
    }

    public void OnResetPressed()
    {
        ResetToPlacement();
    }

    public void NotifyExitPortalPlaced()
    {
        State = GameState.Ready;
        HasPlacedExitPortal = true;
        UpdateButtonState();
    }

    private void ResetToPlacement()
    {
        State = GameState.PlacingPortal;
        HasPlacedExitPortal = false;

        if (portalExit != null)
        {
            portalExit.position = new Vector3(
                PortalDropSpec.DefaultExitPortalPosition.x,
                PortalDropSpec.DefaultExitPortalPosition.y,
                portalExit.position.z);
        }

        if (exitPortalComponent == null && portalExit != null)
            exitPortalComponent = portalExit.GetComponent<Portal>();
        if (exitPortalComponent != null)
            exitPortalComponent.SetSide(PortalDropSpec.DefaultExitPortalSide);

        if (ball != null)
        {
            Vector2 spawnPosition = PortalMath.ComputeSpawnPosition(ball.BallRadius);
            ball.ResetForPlacement(spawnPosition);
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (startButton != null)
            startButton.interactable = State == GameState.Ready && HasPlacedExitPortal;

        if (resetButton != null)
            resetButton.interactable = true;
    }
}

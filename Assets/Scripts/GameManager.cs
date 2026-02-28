using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// State machine: PlacingPortal, Ready, Running. Start/Reset, ball spawn, fixedDeltaTime.
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PlacingPortal,
        Ready,
        Running
    }

    [SerializeField] private BallController ball;
    [SerializeField] private Transform portalEntry;
    [SerializeField] private Transform portalExit;
    [SerializeField] private Button startButton;
    [SerializeField] private Button resetButton;

    public GameState State { get; private set; } = GameState.PlacingPortal;

    private void Awake()
    {
        Time.fixedDeltaTime = PortalDropSpec.FixedDeltaTime;

        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetPressed);
    }

    private void Start()
    {
        ResetToReady();
    }

    public void OnStartPressed()
    {
        if (State != GameState.Ready && State != GameState.PlacingPortal)
            return;

        State = GameState.Running;
        if (ball != null)
            ball.Unfreeze();
    }

    public void OnResetPressed()
    {
        ResetToReady();
    }

    private void ResetToReady()
    {
        State = GameState.Ready;

        if (ball != null)
        {
            Vector2 ballSpawnPosition = PortalDropSpec.BallSpawnPosition;
            ball.transform.position = new Vector3(ballSpawnPosition.x, ballSpawnPosition.y, ball.transform.position.z);
            ball.Freeze();
        }
    }
}

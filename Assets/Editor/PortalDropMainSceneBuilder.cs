using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PortalDropMainSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Main.unity";
    private const string GeneratedFolderPath = "Assets/Generated";
    private const string SpritePath = "Assets/Generated/PortalDropSquare.png";

    private static readonly Color CameraBackground = new Color(0.09f, 0.11f, 0.14f, 1f);
    private static readonly Color WallColor = new Color(0.18f, 0.22f, 0.26f, 1f);
    private static readonly Color BallColor = new Color(0.95f, 0.95f, 0.96f, 1f);
    private static readonly Color EntryPortalColor = new Color(0.18f, 0.61f, 0.95f, 1f);
    private static readonly Color ExitPortalColor = new Color(1f, 0.57f, 0.15f, 1f);
    private static readonly Color StartButtonColor = new Color(0.21f, 0.67f, 0.37f, 1f);
    private static readonly Color ResetButtonColor = new Color(0.82f, 0.28f, 0.28f, 1f);
    private static readonly Vector2 ButtonSize = new Vector2(180f, 60f);
    private static readonly Vector2 StartButtonAnchoredPosition = new Vector2(130f, 70f);
    private static readonly Vector2 ResetButtonAnchoredPosition = new Vector2(-130f, 70f);

    [MenuItem("PortalDrop/Rebuild Main Scene")]
    public static void BuildWave1MainScene()
    {
        Sprite squareSprite = EnsureSquareSprite();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Camera mainCamera = CreateMainCamera();
        GameObject arena = new GameObject("Arena");
        CreateWall(arena.transform, "TopWall", new Vector3(0f, PortalDropSpec.ArenaTop + PortalDropSpec.WallThickness * 0.5f, 0f), new Vector2(10f + (PortalDropSpec.WallThickness * 2f), PortalDropSpec.WallThickness), squareSprite);
        CreateWall(arena.transform, "BottomWall", new Vector3(0f, PortalDropSpec.ArenaBottom - PortalDropSpec.WallThickness * 0.5f, 0f), new Vector2(10f + (PortalDropSpec.WallThickness * 2f), PortalDropSpec.WallThickness), squareSprite);
        CreateWall(arena.transform, "LeftWall", new Vector3(PortalDropSpec.ArenaLeft - PortalDropSpec.WallThickness * 0.5f, 0f, 0f), new Vector2(PortalDropSpec.WallThickness, 16f + (PortalDropSpec.WallThickness * 2f)), squareSprite);
        CreateWall(arena.transform, "RightWall", new Vector3(PortalDropSpec.ArenaRight + PortalDropSpec.WallThickness * 0.5f, 0f, 0f), new Vector2(PortalDropSpec.WallThickness, 16f + (PortalDropSpec.WallThickness * 2f)), squareSprite);

        BallController ball = CreateBall(squareSprite);
        PortalManager portalManager = CreateEntryPortal(squareSprite);
        Portal exitPortal = CreateExitPortal(squareSprite);

        Button startButton;
        Button resetButton;
        CreateCanvas(out startButton, out resetButton);
        CreateEventSystem();

        GameObject gameManagerObject = new GameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
        InputPortalPlacer inputPortalPlacer = gameManagerObject.AddComponent<InputPortalPlacer>();

        WireGameManager(gameManager, ball, portalManager.transform, exitPortal.transform, startButton, resetButton);
        WireInputPortalPlacer(inputPortalPlacer, exitPortal.transform, exitPortal, gameManager, mainCamera);
        WirePortalManager(portalManager, exitPortal.transform, exitPortal, ball, gameManager);

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static Camera CreateMainCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera cameraComponent = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = new Vector3(0f, 0f, PortalDropSpec.CameraZPosition);
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = PortalDropSpec.CameraOrthographicSize;
        cameraComponent.clearFlags = CameraClearFlags.SolidColor;
        cameraComponent.backgroundColor = CameraBackground;
        return cameraComponent;
    }

    private static void CreateWall(Transform parent, string name, Vector3 position, Vector2 size, Sprite sprite)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent, false);
        wall.transform.position = position;
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = WallColor;

        BoxCollider2D boxCollider = wall.AddComponent<BoxCollider2D>();
        boxCollider.size = Vector2.one;
    }

    private static BallController CreateBall(Sprite sprite)
    {
        GameObject ballObject = new GameObject("Ball");
        Vector2 spawnPosition = PortalMath.ComputeSpawnPosition(PortalDropSpec.DefaultBallRadius);
        ballObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);

        SpriteRenderer spriteRenderer = ballObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = BallColor;
        ballObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Rigidbody2D rigidbody2D = ballObject.AddComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 1f;
        rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;

        CircleCollider2D circleCollider = ballObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;

        BallController ballController = ballObject.AddComponent<BallController>();
        SetSerializedReference(ballController, "rb", rigidbody2D);
        SetSerializedReference(ballController, "circleCollider", circleCollider);
        ballController.ResetForPlacement(spawnPosition);
        return ballController;
    }

    private static PortalManager CreateEntryPortal(Sprite sprite)
    {
        GameObject portalObject = new GameObject("PortalEntry");
        portalObject.transform.position = new Vector3(PortalDropSpec.EntryPortalPosition.x, PortalDropSpec.EntryPortalPosition.y, 0f);
        portalObject.transform.localScale = new Vector3(PortalDropSpec.EntryPortalSize.x, PortalDropSpec.EntryPortalSize.y, 1f);

        SpriteRenderer spriteRenderer = portalObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = EntryPortalColor;
        spriteRenderer.sortingOrder = 5;

        BoxCollider2D boxCollider = portalObject.AddComponent<BoxCollider2D>();
        boxCollider.size = Vector2.one;
        boxCollider.isTrigger = true;

        Portal portal = portalObject.AddComponent<Portal>();
        portal.SetSide(PortalDropSpec.EntryPortalSide);

        return portalObject.AddComponent<PortalManager>();
    }

    private static Portal CreateExitPortal(Sprite sprite)
    {
        GameObject portalObject = new GameObject("PortalExit");
        portalObject.transform.position = new Vector3(PortalDropSpec.DefaultExitPortalPosition.x, PortalDropSpec.DefaultExitPortalPosition.y, 0f);
        portalObject.transform.localScale = new Vector3(PortalDropSpec.ExitPortalSize.x, PortalDropSpec.ExitPortalSize.y, 1f);

        SpriteRenderer spriteRenderer = portalObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = ExitPortalColor;
        spriteRenderer.sortingOrder = 5;

        BoxCollider2D boxCollider = portalObject.AddComponent<BoxCollider2D>();
        boxCollider.size = Vector2.one;
        boxCollider.isTrigger = true;

        Portal portal = portalObject.AddComponent<Portal>();
        portal.SetSide(PortalDropSpec.DefaultExitPortalSide);
        return portal;
    }

    private static void CreateCanvas(out Button startButton, out Button resetButton)
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        startButton = CreateButton(canvasObject.transform, "StartButton", StartButtonAnchoredPosition, StartButtonColor, "Start");
        startButton.interactable = false;
        resetButton = CreateButton(canvasObject.transform, "ResetButton", ResetButtonAnchoredPosition, ResetButtonColor, "Reset");
    }

    private static Button CreateButton(Transform parent, string name, Vector2 anchoredPosition, Color color, string label)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(name == "StartButton" ? 0f : 1f, 0f);
        rectTransform.anchorMax = rectTransform.anchorMin;
        rectTransform.pivot = new Vector2(name == "StartButton" ? 0f : 1f, 0f);
        rectTransform.sizeDelta = ButtonSize;
        rectTransform.anchoredPosition = anchoredPosition;

        buttonObject.AddComponent<CanvasRenderer>();
        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        Font font = LoadBuiltinFont();
        if (font != null)
        {
            GameObject textObject = new GameObject("Label");
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            textObject.AddComponent<CanvasRenderer>();
            Text text = textObject.AddComponent<Text>();
            text.font = font;
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.raycastTarget = false;
        }

        return button;
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private static void WireGameManager(GameManager gameManager, BallController ball, Transform portalEntry, Transform portalExit, Button startButton, Button resetButton)
    {
        SetSerializedReference(gameManager, "ball", ball);
        SetSerializedReference(gameManager, "portalEntry", portalEntry);
        SetSerializedReference(gameManager, "portalExit", portalExit);
        SetSerializedReference(gameManager, "startButton", startButton);
        SetSerializedReference(gameManager, "resetButton", resetButton);
    }

    private static void WireInputPortalPlacer(InputPortalPlacer inputPortalPlacer, Transform exitPortalTransform, Portal exitPortalPortal, GameManager gameManager, Camera mainCamera)
    {
        SetSerializedReference(inputPortalPlacer, "exitPortalTransform", exitPortalTransform);
        SetSerializedReference(inputPortalPlacer, "exitPortalPortal", exitPortalPortal);
        SetSerializedReference(inputPortalPlacer, "gameManager", gameManager);
        SetSerializedReference(inputPortalPlacer, "mainCamera", mainCamera);
    }

    private static void WirePortalManager(PortalManager portalManager, Transform exitPortalTransform, Portal exitPortalPortal, BallController ballController, GameManager gameManager)
    {
        SetSerializedReference(portalManager, "exitPortalTransform", exitPortalTransform);
        SetSerializedReference(portalManager, "exitPortalPortal", exitPortalPortal);
        SetSerializedReference(portalManager, "ballController", ballController);
        SetSerializedReference(portalManager, "gameManager", gameManager);
    }

    private static void SetSerializedReference(Object target, string propertyName, Object value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Sprite EnsureSquareSprite()
    {
        if (!AssetDatabase.IsValidFolder(GeneratedFolderPath))
            AssetDatabase.CreateFolder("Assets", "Generated");

        if (!File.Exists(SpritePath))
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(SpritePath, ImportAssetOptions.ForceSynchronousImport);

            TextureImporter importer = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 1f;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
    }

    private static Font LoadBuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null)
            return font;

        return Resources.GetBuiltinResource<Font>("Inter-Regular.ttf");
    }
}

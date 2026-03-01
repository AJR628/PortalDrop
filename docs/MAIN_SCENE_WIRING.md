# Main.unity Wiring Checklist

Use this checklist when creating `Assets/Scenes/Main.unity`. Values below must match `PortalDropSpec`.

## Root Objects

Create these root objects:

- `Main Camera`
- `Global Light 2D`
- `Arena`
- `Ball`
- `PortalEntry`
- `PortalExit`
- `Canvas`
- `EventSystem`
- `GameManager`

## Main Camera

- Camera projection: Orthographic
- Position: `(0, 0, -10)`
- Orthographic size: `8.5`
- Clear flags: Solid Color

## Arena

`Arena` is an empty parent for the four wall strips.

Create these child objects under `Arena`:

- `TopWall`
- `BottomWall`
- `LeftWall`
- `RightWall`

Each wall uses:

- `SpriteRenderer` for visibility
- `BoxCollider2D`
- no Rigidbody2D
- collider `isTrigger = false`

Wall placement:

- `TopWall`: position `(0, 8.1, 0)`, scale `(10.4, 0.2, 1)`, collider size `(10.4, 0.2)`
- `BottomWall`: position `(0, -8.1, 0)`, scale `(10.4, 0.2, 1)`, collider size `(10.4, 0.2)`
- `LeftWall`: position `(-5.1, 0, 0)`, scale `(0.2, 16.4, 1)`, collider size `(0.2, 16.4)`
- `RightWall`: position `(5.1, 0, 0)`, scale `(0.2, 16.4, 1)`, collider size `(0.2, 16.4)`

## Ball

- Position must match `PortalMath.ComputeSpawnPosition(ballRadius)` on load
- Components:
  - `SpriteRenderer`
  - `Rigidbody2D`
  - `CircleCollider2D`
  - `BallController`
- Rigidbody2D:
  - Body Type: Dynamic
  - Gravity Scale: `1`
  - Collision Detection: Continuous
  - Interpolation: Interpolate
- CircleCollider2D radius: `0.25`

## PortalEntry

- Position: `(0, -7.75, 0)`
- Scale: `(1.75, 0.5, 1)`
- Components:
  - `SpriteRenderer`
  - `BoxCollider2D`
  - `Portal`
  - `PortalManager`
- BoxCollider2D:
  - size `(1.75, 0.5)`
  - `isTrigger = true`
- Portal:
  - side `Bottom`
  - inward normal `(0, 1)`

## PortalExit

- Position: `(5, 0, 0)`
- Scale: `(1.0, 0.5, 1)`
- Components:
  - `SpriteRenderer`
  - `BoxCollider2D`
  - `Portal`
- BoxCollider2D:
  - size `(1.0, 0.5)`
  - `isTrigger = true`
- Portal:
  - side `Right`
  - inward normal `(-1, 0)`

## Canvas

- Render Mode: Screen Space - Overlay
- Components:
  - `Canvas`
  - `CanvasScaler`
  - `GraphicRaycaster`

Canvas contains:

- `StartButton`
- `ResetButton`

Buttons use:

- `RectTransform`
- `CanvasRenderer`
- `Image`
- `Button`

`StartButton` must start disabled. GameManager enables it only after user placement.

## EventSystem

- Components:
  - `EventSystem`
  - `StandaloneInputModule`

## GameManager Object

`GameManager` is an empty root object with:

- `GameManager`
- `InputPortalPlacer`

Serialized refs on `GameManager`:

- `ball` -> `Ball/BallController`
- `portalEntry` -> `PortalEntry/Transform`
- `portalExit` -> `PortalExit/Transform`
- `startButton` -> `Canvas/StartButton/Button`
- `resetButton` -> `Canvas/ResetButton/Button`

Serialized refs on `InputPortalPlacer`:

- `exitPortalTransform` -> `PortalExit/Transform`
- `exitPortalPortal` -> `PortalExit/Portal`
- `gameManager` -> `GameManager/GameManager`
- `mainCamera` -> `Main Camera/Camera`

Serialized refs on `PortalManager`:

- `exitPortalTransform` -> `PortalExit/Transform`
- `exitPortalPortal` -> `PortalExit/Portal`
- `ballController` -> `Ball/BallController`
- `gameManager` -> `GameManager/GameManager`

## Build Settings

- `Assets/Scenes/Main.unity` must exist
- `ProjectSettings/EditorBuildSettings.asset` must register `Main.unity`
- `Main.unity` is the only required scene for Wave 1

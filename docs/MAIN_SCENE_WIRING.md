# Main.unity Wiring Checklist

Use this checklist when setting up the Main scene. ArenaRect: left=-5, right=+5, bottom=-8, top=+8.

---

## 1. Create GameObjects

| GameObject   | Parent   | Notes                    |
|--------------|----------|--------------------------|
| Main Camera  | (root)   | Orthographic             |
| Arena        | (root)   | Empty parent for walls   |
| Ball         | (root)   | Spawn inside arena       |
| PortalEntry  | (root)   | Fixed on bottom wall     |
| PortalExit   | (root)   | User-placed on boundary  |
| Canvas       | (root)   | UI overlay               |
| GameManager  | (root)   | Empty, holds scripts     |

---

## 2. Main Camera

- Projection: Orthographic
- Size: 8–9 (for 16-height arena)
- Position: (0, 0, -10)
- Clear Flags: Solid Color

---

## 3. Arena (4 Wall Strips)

**Wall thickness t** (e.g., 0.2). Inner collision edge = ArenaRect edge.

Create 4 child GameObjects under Arena:

| Wall   | Position (center) | BoxCollider2D size | Notes                    |
|--------|-------------------|--------------------|--------------------------|
| Top    | (0, 8 + t/2)      | (10 + 2t, t)       | e.g. (0, 8.1) for t=0.2  |
| Bottom | (0, -8 - t/2)     | (10 + 2t, t)       | e.g. (0, -8.1)           |
| Left   | (-5 - t/2, 0)     | (t, 16 + 2t)       | e.g. (-5.1, 0)           |
| Right  | (5 + t/2, 0)      | (t, 16 + 2t)       | e.g. (5.1, 0)            |

- All walls: BoxCollider2D, **not** trigger
- No Rigidbody2D on walls (static)

---

## 4. Ball

- **Position**: (0, 7.4) — inside arena, below top (spawnY = top - 0.6)
- **Components**:
  - Rigidbody2D: Dynamic, Gravity Scale 1
  - Collision Detection: **Continuous**
  - Interpolation: **Interpolate**
  - CircleCollider2D: radius 0.25 (or as desired)
  - BallController
- **Tag**: "Ball" (optional, for PortalManager lookup)

---

## 5. PortalEntry

- **Position**: (0, -8 + 0.25) = (0, -7.75) — slightly inside arena from bottom
- **Components**:
  - BoxCollider2D: isTrigger = true, size (1.5–2.0, 0.5) — width 1.5–2.0 units
  - Portal: Side = Bottom, InwardNormal = (0, 1)
  - PortalManager — attach here so it receives OnTriggerEnter2D when ball enters

---

## 6. PortalExit

- **Initial position**: Right wall middle — (5, 0) on boundary
- **Components**:
  - BoxCollider2D: isTrigger = true, size (1.0, 0.5) or similar
  - Portal: Side = Right, InwardNormal = (-1, 0)
- InputPortalPlacer will move this when user taps

---

## 7. Canvas

- Render Mode: Screen Space - Overlay
- Add **EventSystem** (usually auto-created)
- **Start/Drop** button: Wire onClick → GameManager.OnStartPressed
- **Reset** button: Wire onClick → GameManager.OnResetPressed

---

## 8. GameManager (Empty GameObject)

- **Components**: GameManager, InputPortalPlacer
- **Serialized refs** (assign in Inspector):
  - Ball (BallController)
  - PortalEntry (Transform)
  - PortalExit (Transform)
  - Start button
  - Reset button
  - Main Camera (for InputPortalPlacer)
- **Note**: PortalManager is on PortalEntry (see §9). InputPortalPlacer needs: exitPortal, gameManager (for state), mainCamera

---

## 9. PortalManager Trigger Setup

**PortalManager** must receive `OnTriggerEnter2D` when the ball enters the entry portal. Per spec (6 scripts only), PortalManager handles this.

**Setup**: Attach **PortalManager** to **PortalEntry** (same GameObject that has the entry BoxCollider2D trigger). PortalManager gets `OnTriggerEnter2D` directly.

- PortalEntry components: BoxCollider2D (trigger) + Portal + **PortalManager**
- PortalManager serialized refs: exitPortal (Transform), exitPortalPortal (Portal), ballController (BallController)

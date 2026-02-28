# Portal Drop — SSOT Spec

Single source of truth for the Portal Drop Unity 2D prototype. Follow exactly; do not add systems beyond this scope.

---

## 1. Core Loop and Rules

- **Entry portal**: Fixed on bottom wall, center. Ball enters here.
- **Exit portal**: User places on arena boundary before run. Ball exits here.
- **Flow**: User taps boundary → exit portal snaps → Start/Drop → ball spawns → ball falls → enters entry portal → teleports to exit with rotated velocity → Reset returns to placement.
- **Win condition**: Reserved for later (GoalZone stub only).

---

## 2. Teleport Math Definition

```
angle = SignedAngle(entryNormal, exitNormal)
vOut = Rotate(vIn, angle)
```

- Preserve speed; rotate velocity from entry inward normal to exit inward normal.
- **Position**: `exitPosition + exitNormal * offset`, where `offset = ballRadius + 0.05f`.
- **Cooldown**: 0.15s to prevent instant re-teleport.
- **Trigger**: Only on ENTRY portal overlap. EXIT portal does not teleport.

---

## 3. ArenaRect (Authoritative)

**Canonical rectangle** — use for all snapping and geometry. Do **not** use Collider2D.bounds (wall strips have thickness).

| Edge   | Value |
|--------|-------|
| left   | -5    |
| right  | +5    |
| bottom | -8    |
| top    | +8    |

- Center: (0, 0)
- Size: 10 × 16 world units

---

## 4. Snap-to-Boundary Algorithm

- **Input**: Screen/tap point → `Camera.ScreenToWorldPoint` → world point.
- **Snap**: Nearest point on ArenaRect perimeter (4 segments: top, bottom, left, right).
- **No-corners margin**: When snapping to a side, clamp along that edge by `portalHalfWidth + cornerMargin`.
  - `portalHalfWidth` = `PortalExit` BoxCollider2D `size.x / 2` (when on top/bottom) or `size.y / 2` (when on left/right). Or lock `portalHalfWidth = 0.5f` for MVP.
  - `cornerMargin` default: `0.75f`.
- **Optional quantize**: 0.25 unit increments along perimeter for puzzle feel.

---

## 5. Wall Placement (Inner Edge = ArenaRect)

If using thin wall strips (thickness = t), place so the **inner** collision boundary matches ArenaRect edges:

| Wall   | Center position        |
|--------|-------------------------|
| Top    | (0, top + t/2)          |
| Bottom | (0, bottom - t/2)       |
| Left   | (left - t/2, 0)        |
| Right  | (right + t/2, 0)       |

Example: t = 0.2 → Top wall center at (0, 8.1), Bottom at (0, -8.1), Left at (-5.1, 0), Right at (5.1, 0).

---

## 6. Ball Radius and Spawn

**ballRadius** (for spawn position and teleport offset):

```
ballRadius = circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y)
```

**Spawn position**: Inside arena, near top. `spawnY = top - (ballRadius + margin)`, margin default 0.05–0.1.

- Example: top = 8, ballRadius ≈ 0.25, margin = 0.1 → spawnY = 7.65. Or use spawnY = 7.4 for simplicity.

---

## 7. Game States (MVP)

| State         | Description                                           |
|---------------|-------------------------------------------------------|
| PlacingPortal | User can tap to place/move exit portal                |
| Ready         | Exit placed, Start enabled, ball at spawn             |
| Running       | Ball dropped, physics active                          |
| Reset         | Returns to PlacingPortal or Ready, ball back at spawn |

---

## 8. Script List and Responsibilities

| Script                 | Responsibility                                                       |
|------------------------|----------------------------------------------------------------------|
| `GameManager.cs`       | State machine, Start/Reset, ball spawn, `Time.fixedDeltaTime = 1/60` |
| `PortalManager.cs`     | Entry/exit refs, trigger detection on entry only, teleport logic     |
| `Portal.cs`            | PortalSide enum, inward normal, side/normal helpers                  |
| `InputPortalPlacer.cs` | Tap/click → world point → snap to perimeter → position PortalExit   |
| `BallController.cs`    | Rigidbody2D ref, CanTeleport, SetTeleportCooldown                    |
| `GoalZone.cs`          | Stub (empty or log hits)                                             |

---

## 9. Scene Object Recipe (Main.unity)

See [MAIN_SCENE_WIRING.md](MAIN_SCENE_WIRING.md) for the full wiring checklist.

```
Main Camera
  - Orthographic, size for portrait (~8–9), arena centered

Arena (empty parent)
  - 4 thin BoxCollider2D wall strips (Top/Bottom/Left/Right) OR EdgeCollider2D loop
  - Do NOT use a single filled BoxCollider2D
  - Wall inner edges align with ArenaRect (see §5)

Ball
  - Rigidbody2D (Dynamic), CircleCollider2D
  - Collision Detection: Continuous; Interpolation: Interpolate
  - PhysicsMaterial2D (optional bounce)
  - Spawn: inside arena, spawnY = top - (ballRadius + margin)

PortalEntry
  - BoxCollider2D (isTrigger), width 1.5–2.0 units
  - Position: slightly inside arena (bottom + 0.25), center on bottom wall
  - inward normal: (0, 1)

PortalExit
  - BoxCollider2D (isTrigger)
  - Initial: right wall middle
  - Movable via tap in PlacingPortal/Ready

Canvas
  - Start/Drop button, Reset button
```

---

## 10. Critical Unity Collider + Simulation Rules

- **Arena boundary**: 4 thin BoxCollider2D strips or EdgeCollider2D loop. Not a filled box.
- **PortalEntry trigger**: Position slightly inside arena (bottom + 0.25). Width 1.5–2.0 units.
- **Ball physics frozen until Start**:
  - PlacingPortal/Ready: `rb.simulated = false`, velocities zeroed on reset
  - Running: `rb.simulated = true`
- **InputPortalPlacer**: Ignore taps on UI — `EventSystem.current.IsPointerOverGameObject()` (mouse), `IsPointerOverGameObject(touch.fingerId)` (touch).
- **Teleport**: Triggers only on ENTRY portal overlap. EXIT portal does not teleport.

---

## 11. Determinism Knobs

- `Time.fixedDeltaTime = 1/60` — set in `GameManager.Awake` (do not rely on manual Project Settings).
- Ball Rigidbody2D: Collision Detection = Continuous; Interpolation = Interpolate.
- Teleport offset: `ballRadius + 0.05f`.

---

## 12. Known Non-Goals (MVP)

- No level system, no scoring, no particles
- No backend, accounts, ads, subscriptions
- Single scene only

---

## 13. Acceptance Checks

1. Tap boundary → exit portal snaps to perimeter point (respect corner margin).
2. Start/Drop → ball drops (physics enabled).
3. Ball overlaps ENTRY portal trigger → teleports to exit with rotated velocity, continues moving.
4. Reset → ball returns to spawn, physics frozen again, repeatable.

---

## 14. Decision Log

- ArenaRect: left=-5, right=+5, bottom=-8, top=+8 (size 10×16, center 0,0).
- Wall strips: inner edge = ArenaRect edge; centers at top+t/2, bottom-t/2, left-t/2, right+t/2.
- portalHalfWidth: from BoxCollider2D or 0.5f for MVP; cornerMargin = 0.75f.
- ballRadius: `circleCollider.radius * max(lossyScale.x, lossyScale.y)`.
- Ball spawn: inside arena, top - (ballRadius + margin).
- Cooldown: 0.15s.
- Exit offset: ballRadius + 0.05f.
- fixedDeltaTime: 1/60 in GameManager.Awake.

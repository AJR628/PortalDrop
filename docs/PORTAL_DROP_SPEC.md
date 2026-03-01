# Portal Drop - Wave 1 SSOT Spec

Single source of truth for the PortalDrop Wave 1 MVP. Scope is only:

- place the exit portal
- press Start to drop the ball
- teleport from the fixed entry portal to the placed exit portal
- press Reset to return to a clean PlacingPortal state

Do not add scoring, particles, level flow, or goal logic in this wave.

## Core Loop

1. Scene boots in `PlacingPortal`.
2. Exit portal starts at the default right-wall position, but this does not count as a user placement.
3. User taps or clicks the arena boundary.
4. Exit portal snaps to the nearest valid perimeter point and `Start` becomes enabled.
5. User presses `Start`.
6. Ball physics begins and the ball falls.
7. Ball overlaps the entry portal trigger.
8. Ball teleports to the exit portal using rotated velocity.
9. User presses `Reset`.
10. Scene returns to a clean `PlacingPortal` state.

## Authoritative Constants

All Wave 1 constants live in `PortalDropSpec`.

### Arena

- Left: `-5`
- Right: `5`
- Bottom: `-8`
- Top: `8`
- Size: `10 x 16`

### Camera

- Orthographic
- Position: `(0, 0, -10)`
- Size: `8.5`

### Walls

- Thickness: `0.2`
- Use four thin `BoxCollider2D` wall strips
- Inner wall edge must align with the arena rectangle

Wall centers:

- Top: `(0, 8.1)`
- Bottom: `(0, -8.1)`
- Left: `(-5.1, 0)`
- Right: `(5.1, 0)`

Wall collider sizes:

- Top: `(10.4, 0.2)`
- Bottom: `(10.4, 0.2)`
- Left: `(0.2, 16.4)`
- Right: `(0.2, 16.4)`

### Ball

- `CircleCollider2D` radius: `0.25`
- Rigidbody2D body type: Dynamic
- Gravity Scale: `1`
- Collision Detection: Continuous
- Interpolation: Interpolate
- `ballRadius = circleCollider.radius * max(lossyScale.x, lossyScale.y)`
- Spawn margin: `0.10`
- Spawn position: `(0, 7.65)` when radius is `0.25`

### Entry Portal

- Position: `(0, -7.75)`
- Size: `(1.75, 0.5)`
- Side: `Bottom`
- Inward normal: `(0, 1)`
- Trigger only

### Exit Portal

- Default position: `(5, 0)`
- Size: `(1.0, 0.5)`
- Default side: `Right`
- Inward normal: `(-1, 0)`
- Trigger only

### Snap Rules

- Nearest arena side wins
- Corner margin: `0.75`
- Portal half-width for Wave 1: `0.5`
- Snap margin: `1.25`
- Tie-break order: `Top`, then `Bottom`, then `Left`, then `Right`
- No quantization in Wave 1

### Teleport Rules

- `angle = SignedAngle(entryNormal, exitNormal)`
- `vOut = Rotate(vIn, angle)`
- Preserve speed
- Exit position: `exitPosition + exitNormal * (ballRadius + 0.05)`
- Cooldown: `0.15` seconds
- Only the entry portal teleports

### Simulation Rules

- `Time.fixedDeltaTime = 1 / 60`
- `PlacingPortal` and `Ready`: `rb.simulated = false`
- `Running`: `rb.simulated = true`
- Reset clears velocity, angular velocity, and teleport cooldown

## Wave 1 States

- `PlacingPortal`: exit portal can be placed, Start is disabled
- `Ready`: player has placed the exit portal, Start is enabled, ball is frozen at spawn
- `Running`: Start has been pressed, placement is locked, ball physics is active

Reset always returns to `PlacingPortal`.

## Runtime Ownership

- `PortalDropSpec.cs`: single authority for constants
- `PortalMath.cs`: pure math for rotation, snap, spawn, and teleport position
- `GameManager.cs`: state machine, reset/start gating, default exit placement, spawn/reset
- `InputPortalPlacer.cs`: tap and click handling, UI rejection, exit placement
- `PortalManager.cs`: entry trigger teleport
- `Portal.cs`: side and inward normal mapping
- `BallController.cs`: radius, Rigidbody2D access, freeze/reset/cooldown
- `GoalZone.cs`: stub only, unused in Wave 1

## Acceptance Checks

1. Scene boots in `PlacingPortal` with Start disabled.
2. Default exit portal is at `(5, 0)` but Start remains disabled until the first user placement.
3. Tap or click near a wall and the exit portal snaps to the nearest valid perimeter point.
4. Start enables only after user placement.
5. Press Start and the ball begins falling.
6. Ball entering the entry portal teleports to the exit portal and keeps its speed with rotated direction.
7. Press Reset and the ball returns to spawn, the exit portal returns to default right-middle, Start becomes disabled, and state returns to `PlacingPortal`.

# Relic Guardian Implemented Architecture

Last reviewed against the actual project-owned C# source: 2026-09-02.

This file is the compact architecture map for behavior that is currently implemented. Actual code, Unity assets, current Editor state, and Git status remain authoritative. Approved but unimplemented feature designs belong in their feature-design documents and must not be treated as runtime facts.

## Runtime Foundation

- Unity version: `6000.3.19f1`.
- Player and enemy displacement use `CharacterController`.
- Apply Root Motion remains disabled.
- Project-owned gameplay scripts live under `Assets/RelicGuardian/`.
- `Assets/LocalLicensed/` contains ignored local presentation dependencies and is never a code or documentation source of truth.

## Player Component Ownership

| Component | Implemented responsibility |
| --- | --- |
| `PlayerInputReader` | Records movement/look values, held Sprint/Block state, and one-use Attack/Jump/Lock-On/Block requests. It does not decide whether an action is legal. |
| `PlayerActionController` | Sole owner of the coarse player action state and deterministic Block/Attack/Jump request arbitration. |
| `PlayerCombat` | Owns the four-step Basic Attack sequence, attack targets, windows, queue/restart state, attack facing requests, lunge requests, enemy damage requests, and complete attack cleanup. |
| `PlayerBlock` | Owns the internal Block `Startup`, `Hold`, and `Release` phases, phase-aware Hold movement permission, directional Guard Coverage decisions, and production of the explicit `GuardResult`. |
| `PlayerMovement` | Sole owner of player `CharacterController` movement and actual player Transform-facing application. Other gameplay components request facing or displacement through it. |
| `PlayerTargeting` | Owns the current Lock-On target, nearest-target acquisition, toggle/cancel behavior, and break-distance validation. Lock-On is orthogonal to the coarse action state. |
| `PlayerCameraController` | Selects Free/Lock-On Cinemachine camera priorities, input-axis ownership, and the weighted Lock-On camera target. |
| `PlayerAnimator` | Writes Animator parameters and triggers code-driven presentation changes. It does not decide gameplay permission, damage, coverage, or action state. |
| `PlayerHitReceiver` | Single entry for defendable incoming hits. It resolves same-frame action requests, asks `PlayerBlock` for a `GuardResult`, forwards `Unhandled` hits to `PlayerHealth`, and routes handled results once to Guard presentation. |
| `PlayerGuardPresentation` | Consumes an already-decided Ordinary or Perfect `GuardResult`. It owns the configurable Guard impact anchor, independent Normal/Perfect Prefab references and cleanup lifetimes, independent Ordinary/Perfect `CombatAudioData`, result-specific VFX/SFX routing, and diagnostic logs; it never decides damage or Guard legality. |
| `CombatAudioPlayer` | Reusable presentation component that owns preconfigured `AudioSource` channels, maps one `CombatAudioData` layer array to them, stops prior scheduled playback, and schedules valid layers from one DSP-time base. It does not classify hits or own combat permission. |
| `PlayerHealth` | Stores player health and subtracts integer damage forwarded by `PlayerHitReceiver`. It has no clamp, death flow, or Guard decision logic. |
| `PlayerAttackData` | Serializable per-step Basic Attack configuration for damage, target range, lunge speed, and lunge distance. |
| `CombatAudioLayer` / `CombatAudioData` | Serializable presentation data for one Clip/Volume/Pitch/Delay layer and one Master-Volume-plus-layers cue. They contain no playback or gameplay decisions. |

## Coarse Player Action State

`PlayerActionController` owns one coarse state:

```text
Free
├─ accepted grounded Attack -> Attacking
└─ accepted grounded Block  -> Blocking

Attacking
├─ natural/cancel cleanup -> Free
└─ legal Block cancel     -> Blocking

Blocking
└─ completed Release -> Free
```

Current coarse states are only `Free`, `Attacking`, and `Blocking`. Guard phases are not additional coarse states.

`ResolveActionRequests()` is the single request-arbitration boundary. Multiple components may call it in one frame, but its `Time.frameCount` gate resolves requests only once. The current fixed order is Block, then Attack, then Jump.

## Player Permission Model

| State or phase | Normal movement | Jump | Sprint |
| --- | --- | --- | --- |
| `Free` | Yes | Yes when grounded and unlocked | Yes |
| `Attacking` | No | No | No |
| Block `Startup` | No | No | No |
| Block held `Hold` | Yes | No | No |
| Block `Release` | No | No | No |

Attack lunge is an explicit `PlayerCombat` request to `PlayerMovement.MoveDuringAttack()` and does not reopen ordinary movement permission. `CanMove` and `CanSprint` remain separate so movable Guard Hold never enables Sprint or its Lock-On cancellation path.

## Input and Action Data Flow

```text
Unity Input System
-> PlayerInputReader records values/requests
-> PlayerActionController.ResolveActionRequests()
-> accepted action owner (PlayerCombat or PlayerBlock)
-> PlayerMovement and PlayerAnimator execute their owned runtime/presentation work
```

Rejected one-use requests are consumed without buffering. Persistent held state such as Block and Sprint remains available until the physical input is released.

## Basic Attack Flow

`PlayerCombat` implements a four-step indexed attack sequence:

```text
accepted Attack request
-> select locked target or nearest in-range target
-> request attack presentation
-> optional facing and code-driven lunge
-> Animation Events open/close Hit and Combo windows
-> confirmed in-range target receives EnemyHealth.TakeDamage(int)
-> FinishAttack or Block cancellation uses EndAttack()
-> coarse state returns to Free and presentation soft recovery begins
```

Animation Events include the attack-step index. Events from an outgoing or cancelled step are ignored when the current coarse state or index no longer matches. `EndAttack()` is the shared cleanup boundary for natural finish and Block cancellation.

## Guard Lifecycle and Presentation

`PlayerBlock` owns this internal phase flow while the coarse state remains `Blocking`:

```text
BeginBlock
-> Startup
-> StartupDecisionPoint
   ├─ Block still held -> Hold
   └─ released         -> Release
-> FinishRelease
-> Free
```

During Hold, `AllowsMovement` is true only while the Block input remains held. `PlayerAnimator.PlayBlockHold()` selects unlocked `Guard_Free_Locomotion` or locked `Guard_Locked_Locomotion`. An active Hold presentation refreshes once when authoritative Lock-On mode changes.

Startup and Hold can handle hits inside the adjustable horizontal Guard Coverage angle. Release, invalid horizontal direction, and coverage failure remain unhandled and continue to health. A real enemy Startup preview may begin fixed-direction Facing Assist during Startup/Hold; the matching hit still uses the saved pre-assist facing for coverage.

`BeginBlock()` opens the minimal Perfect Guard Window during Startup. The authored `Block_Start` Event closes it, and Hold/Release entry closes it defensively. Only after coverage succeeds does `ResolveGuardHit()` return `GuardResult.Perfect` while the window is open or `GuardResult.Ordinary` otherwise; failed Guard resolution returns `GuardResult.Unhandled`. `PlayerHitReceiver` routes handled results to `PlayerGuardPresentation`. Ordinary and Perfect each spawn and explicitly clean their own local Guard Impact Prefab, then submit their independent layered cue to `CombatAudioPlayer`. No Hitstop, Camera Impulse, or enemy reaction is implemented.

## Player Movement and Facing

`PlayerMovement.Update()`:

1. resolves the frame's action requests;
2. filters movement input through `CanMove`;
3. applies Free/Locked Sprint rules;
4. derives camera-relative movement;
5. applies jump/gravity;
6. applies active Guard Facing Assist, otherwise faces the locked target when movable and locked, otherwise faces non-zero movement;
7. moves the `CharacterController`.

`PlayerCombat` requests attack facing through `PlayerMovement.FaceDirection()` and attack lunge through `MoveDuringAttack()`. Active Guard Facing Assist also calls `FaceDirection()` from the explicit Assist -> Locked -> Free Movement branch. `PlayerAnimator` never writes player Transform rotation. Current assist uses the ordinary `rotationSpeed`; exact deadline interpolation is not implemented.

## Lock-On and Camera

`PlayerTargeting` searches the configured layer inside `lockOnRange`, keeps one `CurrentTarget`, and clears it when cancelled, inactive, or beyond `lockOnBreakRange`. Sprint plus movement while locked cancels Lock-On only when Sprint is permitted.

`PlayerCameraController` treats Lock-On as a camera/targeting mode. It switches Cinemachine priorities and input-axis ownership and updates a weighted look target between player and enemy. It does not create a second action system.

## Presentation Soft Recovery

Soft recovery is presentation state inside `PlayerAnimator`, not a coarse gameplay state. It begins after gameplay cleanup returns to `Free` while an authored animation tail remains. Movement or a new legal action can interrupt the visual tail without restoring old damage, targeting, lunge, or action state.

## Enemy Runtime Flow

| Component | Implemented responsibility |
| --- | --- |
| `EnemyAI` | Chooses chase versus attack from distance and the current enemy attack phase. |
| `EnemyMovement` | Applies enemy facing and `CharacterController` movement. |
| `EnemyAttack` | Owns `Ready -> Startup -> HitWindow -> Recovery -> Ready`, telegraph/Animator triggering, a saved `PlayerHitReceiver` target, one Startup `AttackThreatContext`, and one scheduled hit-time `HitContext` delivery. |
| `EnemyAnimator` | Writes enemy movement speed to the Animator. |
| `EnemyHealth` | Subtracts integer damage and disables the GameObject at zero or below. |

Current enemy damage flow is:

```text
EnemyAI
-> EnemyAttack.TryStartAttack(PlayerHitReceiver)
-> timed EnemyAttack.OpenHitWindow()
-> EnemyAttack.ApplyDamage(PlayerHitReceiver)
-> construct HitContext(DamageAmount, Source, IncomingDirection)
-> PlayerHitReceiver.ReceiveHit(HitContext)
   -> handled Blocking coverage success: stop
   -> otherwise PlayerHealth.TakeDamage(int)
```

Current pre-hit threat flow is:

```text
EnemyAttack.TryStartAttack(PlayerHitReceiver)
-> construct AttackThreatContext(Source, fixed IncomingDirection, ExpectedImpactTime)
-> PlayerHitReceiver stores by Source and selects earliest valid threat
-> Block begins after preview, or preview arrives during Startup/Hold
-> PlayerBlock snapshots pre-assist facing and fixed direction
-> PlayerMovement applies Assist facing until expected impact
-> EnemyAttack.OpenHitWindow removes the preview and sends the real HitContext
```

Current Guard result and presentation flow is:

```text
PlayerBlock.ResolveGuardHit(HitContext)
-> validate Startup/Hold and Guard Coverage
-> return Unhandled, Ordinary, or Perfect GuardResult
-> PlayerHitReceiver
   ├─ Unhandled -> PlayerHealth.TakeDamage(int)
   └─ Ordinary/Perfect -> PlayerGuardPresentation.PresentGuardResult()
      ├─ Ordinary -> one Normal Guard Impact + 3-layer DSP-scheduled cue + log
      └─ Perfect -> one Perfect Guard Impact + 4-layer DSP-scheduled cue + log
```

Range is checked before attack Startup. The later Hit Window damages the saved target without a new overlap, range, or line-of-sight confirmation, so the current prototype is a scheduled hit attempt rather than physical hitbox confirmation.

## Implemented Architecture Invariants

- Actual gameplay state and permission decisions do not belong in Animator transitions.
- Player Transform facing remains code-owned; Apply Root Motion stays off.
- Lock-On remains orthogonal to the coarse player action state.
- `PlayerActionController` remains the only coarse action-state owner.
- Attack natural finish and cancellation share one cleanup boundary.
- Same-frame mutually exclusive requests do not rely on `MonoBehaviour.Update()` execution order.
- Guard Hold movement permission never implies Sprint permission.
- Do not introduce a second gameplay FSM, numeric Priority system, general Request Queue, pre-emptive `PlayerMotor`, or large Damage/Ability Framework without a concrete need.

## Approved but Not Implemented

`HitContext`, `PlayerHitReceiver`, Startup/Hold Guard Coverage, the core pre-hit Attack Threat Facing Assist route, minimal Perfect Guard classification, the explicit `GuardResult -> PlayerGuardPresentation` boundary, and independent Guard VFX/SFX routes are implemented. The current enemy's `40-50` degree core turn/guard path, empty-Guard no-turn check, Release-cancellation check, ordinary/perfect classification, unhandled damage route, and distinct presentation feedback are learner-reported runtime verified.

The next presentation work is Guard Hitstop. Its owner, clock/restoration boundary, durations, and overlap behavior must be defined before implementation; Camera Impulse, pooling, and gameplay consequences remain separate.

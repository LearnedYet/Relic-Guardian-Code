# Relic Guardian Implemented Architecture

Documentation updated against inspected source and saved assets: 2026-09-23. Runtime acceptance is recorded in CURRENT_STATE.md.

This file is the compact architecture map for behavior that is currently implemented. Actual code, Unity assets, current Editor state, and Git status remain authoritative. Approved but unimplemented feature designs belong in their feature-design documents and must not be treated as runtime facts.

## Runtime Foundation

- Unity version: `6000.3.19f1`.
- Player and enemy displacement use `CharacterController`.
- Apply Root Motion remains disabled.
- Project-owned gameplay scripts live under `Assets/RelicGuardian/`.
- `Assets/LocalLicensed/` contains ignored local presentation dependencies; actual saved asset settings remain evidence for local configuration, while approved gameplay design belongs in project documents.

## Player Component Ownership

| Component | Implemented responsibility |
| --- | --- |
| `PlayerInputReader` | Records movement/look values, held Sprint/Block state, and one-use Dodge/Attack/Jump/Lock-On/Block requests. It does not decide whether an action is legal. |
| `PlayerActionController` | Sole owner of the coarse player action state and deterministic Dodge/Block/Attack/Jump request arbitration. |
| `PlayerCombat` | Owns the four-step Basic Attack sequence, attack targets, windows, queue/restart state, attack facing requests, lunge requests, enemy damage requests, and complete attack cleanup. |
| `PlayerBlock` | Owns the internal Block `Startup`, `Hold`, and `Release` phases, Ordinary movement-lock deadline and Hold release gate, phase-aware Hold movement permission, directional Guard Coverage decisions, and production of the explicit `GuardResult`. |
| `PlayerDodge` | Owns one grounded Dodge execution's direction snapshot, scaled-time gameplay/movement deadlines, progress-based distance request, natural finish, I-Frame/Perfect-window timing and production of the explicit `DodgeResult`. |
| `PlayerMovement` | Sole owner of player `CharacterController` movement and actual player Transform-facing application. Other gameplay components request facing or displacement through it. |
| `PlayerTargeting` | Owns the current Lock-On target, nearest-target acquisition, toggle/cancel behavior, and break-distance validation. Lock-On is orthogonal to the coarse action state. |
| `PlayerCameraController` | Selects Free/Lock-On Cinemachine camera priorities, input-axis ownership, and the weighted Lock-On camera target. |
| `PlayerAnimator` | Writes Animator parameters and triggers code-driven presentation changes. It does not decide gameplay permission, damage, coverage, or action state. |
| `PlayerHitReceiver` | Single entry for defendable incoming hits. It resolves same-frame action requests, asks the active Dodge or Block owner for an explicit result, returns distinct handled `HitResult` values, forwards unhandled hits to `PlayerHealth`, and routes Guard results once to Guard presentation. |
| `PlayerGuardPresentation` | Consumes GuardResult; owns result-specific VFX/SFX selection, requests Ordinary reaction from PlayerAnimator and Perfect-only Hitstop from the shared owner. |
| `CombatAudioPlayer` | Reusable presentation component that owns preconfigured `AudioSource` channels, maps one `CombatAudioData` layer array to them, stops prior scheduled playback, and schedules valid layers from one DSP-time base. It does not classify hits or own combat permission. |
| `HitstopController` | Sole writer/restorer of global `Time.timeScale` for Hitstop and Slow Motion. Each uses an unscaled deadline; Hitstop takes precedence while active, Slow Motion multiplies the pre-effect scale when Hitstop is absent, and the original scale returns after both expire or the owner disables. |
| `PlayerHealth` | Stores player health and subtracts integer damage forwarded by `PlayerHitReceiver`. It has no clamp, death flow, or Guard decision logic. |
| `PlayerAttackPresentation` | Owns transient Trail playback and indexed Whoosh/Windup cue selection. Uses a separate AttackAudio instance; persistent WeaponAura is independent. |
| `PlayerAttackData` | Serializable per-step Basic Attack configuration for damage, target range, lunge speed, and lunge distance. |
| `CombatAudioLayer` / `CombatAudioData` | Serializable presentation data for one Clip/Volume/Pitch/Delay layer and one Master-Volume-plus-layers cue. They contain no playback or gameplay decisions. |

## Coarse Player Action State

`PlayerActionController` owns one coarse state:

```text
Free
├─ accepted grounded Attack -> Attacking
├─ accepted grounded Block  -> Blocking
└─ accepted grounded Dodge  -> Dodging

Attacking
├─ natural/cancel cleanup -> Free
├─ legal Block cancel     -> Blocking
└─ legal Dodge cancel     -> Dodging

Blocking
└─ completed Release -> Free

Dodging
└─ code-owned deadline -> Free
```

Current coarse states are `Free`, `Attacking`, `Blocking`, and `Dodging`. Guard phases and Dodge presentation recovery are not additional coarse states.

`ResolveActionRequests()` is the single request-arbitration boundary. Multiple components may call it in one frame, but its `Time.frameCount` gate resolves requests only once. The current fixed order is Dodge, then Block, then Attack, then Jump. This request order is separate from authored cancellation permission: current ordinary Basic Attack admits Block and Dodge replacement through its shared cleanup; Blocking and Dodging do not admit each other.

## Player Permission Model

| State or phase | Normal movement | Jump | Sprint |
| --- | --- | --- | --- |
| `Free` | Yes | Yes when grounded and unlocked | Yes |
| `Attacking` | No | No | No |
| Block `Startup` | No | No | No |
| Block held `Hold` | Only after Ordinary movement-lock expiry | No | No |
| Block `Release` | No | No | No |
| `Dodging` | No ordinary movement; only Dodge displacement | No | No |

Attack lunge and Dodge travel are explicit requests to `PlayerMovement.MoveDuringAttack()` and `MoveDuringDodge()` and do not reopen ordinary movement permission. `CanMove` and `CanSprint` remain separate so movable Guard Hold never enables Sprint or its Lock-On cancellation path.

## Input and Action Data Flow

```text
Unity Input System
-> PlayerInputReader records values/requests
-> PlayerActionController.ResolveActionRequests()
-> accepted action owner (PlayerCombat, PlayerBlock, or PlayerDodge)
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
-> confirmed in-range target receives HitContext through EnemyHitReceiver
-> EnemyHealth applies damage once and EnemyHitPresentation expresses the confirmed result
-> FinishAttack or Block cancellation uses EndAttack()
-> coarse state returns to Free and presentation soft recovery begins
```

Animation Events include the attack-step index. Events from an outgoing or cancelled step are ignored when the current coarse state or index no longer matches. Attack cleanup is shared by natural finish, Block cancellation and Dodge cancellation.

## Base Dodge Flow

```text
one-use Dodge request
-> PlayerActionController validates grounded entry/cancellation
-> PlayerDodge snapshots one world direction
-> unlocked input may request one immediate facing snap
-> PlayerAnimator converts direction to DodgeX / DodgeZ and selects presentation
-> PlayerDodge converts normalized elapsed progress into per-frame distance
-> PlayerMovement.MoveDuringDodge applies CharacterController displacement
-> code-owned deadline returns the coarse state to Free
-> zero-input presentation may continue as interruptible soft recovery
```

Unlocked direction uses the existing camera-relative basis and falls back to opposite current facing with no input. Locked direction is built from horizontal target-forward/target-right and falls back away from the target with no input. The snapshot does not steer after entry. Gameplay duration and displacement remain independent from Clip length.

Current incoming-hit Dodge flow is:

```text
PlayerHitReceiver.ReceiveHit
-> resolve same-frame action requests
-> while Dodging, PlayerDodge compares elapsed Dodge time with the I-Frame
   -> outside I-Frame: DodgeResult.Unhandled; continue to Guard/Health
   -> inside I-Frame and Perfect Window: DodgeResult.Perfect -> HitResult.PerfectDodge
   -> inside I-Frame only: DodgeResult.Ordinary -> HitResult.OrdinaryDodge
```

The saved Scene uses I-Frame `0.05..0.45s` and nested Perfect Window `0.05..0.3s`. The broad eligibility check runs before Perfect classification, so a misconfigured Perfect interval cannot grant immunity outside the I-Frame. Both handled Dodge results avoid damage; only Perfect routes from `PlayerHitReceiver` to `PlayerDodgePresentation`. That component snapshots the Dodge start pose into independent static MeshFilter/MeshRenderer objects: active modular SkinnedMeshRenderers are CPU-baked, active rigid MeshRenderers are copied, and LOD-controlled rigid meshes use only LOD0. Each part receives an independent transparent runtime material and an `AfterimageFade` component; timed cleanup destroys the generated GameObject, Mesh and Material. Every accepted `PlayerDodge.BeginDodge()` requests the one-layer Start cue through presentation. Only the Perfect result requests its separate bound two-layer confirmation cue and shared-owner Slow Motion; independent Scene-local `CombatAudioPlayer` instances preserve the Start tail. Presentation does not decide immunity or write `Time.timeScale` directly. Current Slow Motion duration/scale and audio mix were learner-accepted on 2026-09-23; simultaneous Hitstop overlap and disable recovery remain unverified. Movement Trail, Distortion and Dodge Counter are not implemented.

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

During Hold, AllowsMovement requires held Block and an expired Ordinary movement-lock deadline. Update delays Hold release until that deadline; BeginBlock and EnterRelease clear it. StartupDecisionPoint still directly releases when input is no longer held, without checking this deadline. `PlayerAnimator.PlayBlockHold()` selects unlocked `Guard_Free_Locomotion` or locked `Guard_Locked_Locomotion`. An active Hold presentation refreshes once when authoritative Lock-On mode changes.

Startup and Hold can handle hits inside the adjustable horizontal Guard Coverage angle. Release, invalid horizontal direction, and coverage failure remain unhandled and continue to health. A real enemy Startup preview may begin fixed-direction Facing Assist during Startup/Hold; the matching hit still uses the saved pre-assist facing for coverage.

`BeginBlock()` opens the minimal Perfect Guard Window during Startup. The authored `Block_Start` Event closes it, and Hold/Release entry closes it defensively. Only after coverage succeeds does `ResolveGuardHit()` return `GuardResult.Perfect` while the window is open or `GuardResult.Ordinary` otherwise; failed Guard resolution returns `GuardResult.Unhandled`. `PlayerHitReceiver` routes handled results to `PlayerGuardPresentation`. Ordinary and Perfect each spawn and explicitly clean their own local Guard Impact Prefab, then submit their independent layered cue to `CombatAudioPlayer`. Perfect requests shared Hitstop; Ordinary does not. Ordinary also requests PlayerAnimator's independent full-body Override Guard Reaction layer. Its Empty/Ordinary_Guard_Hit states cover the visible pose while Base Layer Guard states continue; automatic exit and PlayBlockEnd clear the reaction. Presentation never grants movement permission.

## Player Movement and Facing

`PlayerMovement.Update()`:

1. resolves the frame's action requests;
2. filters movement input through `CanMove`;
3. applies Free/Locked Sprint rules;
4. derives camera-relative movement;
5. applies jump/gravity;
6. applies active Guard Facing Assist, otherwise faces the locked target when `CanFaceLockedTarget` and locked, otherwise faces non-zero movement;
7. moves the `CharacterController`.

`PlayerCombat` requests attack facing through `PlayerMovement.FaceDirection()` and attack lunge through `MoveDuringAttack()`. `PlayerDodge` requests unlocked input-facing through `SnapFacing()` and exact incremental travel through `MoveDuringDodge()`. Active Guard Facing Assist still has priority over Locked facing, then Free Movement facing. `PlayerAnimator` never writes player Transform rotation. Current assist uses the ordinary `rotationSpeed`; exact deadline interpolation is not implemented.

## Lock-On and Camera

`PlayerTargeting` searches the configured layer inside `lockOnRange`, keeps one `CurrentTarget`, and clears it when cancelled, ineligible through EnemyHitReceiver.IsValidTarget, or beyond `lockOnBreakRange`. Sprint plus movement while locked cancels Lock-On only when Sprint is permitted.

`PlayerCameraController` treats Lock-On as a camera/targeting mode. It switches Cinemachine priorities and input-axis ownership and updates a weighted look target between player and enemy. It does not create a second action system.

## Presentation Soft Recovery

Soft recovery is presentation state inside `PlayerAnimator`, not a coarse gameplay state. It begins after gameplay cleanup returns to `Free` while an authored animation tail remains. Movement or a new legal action can interrupt the visual tail without restoring old damage, targeting, lunge, or action state.

## Enemy Runtime Flow

| Component | Implemented responsibility |
| --- | --- |
| `EnemyAI` | While the coarse state is Chase, calculates horizontal target direction/distance, gives legal attack admission priority, and otherwise selects internal `Run / Approach / Retreat / Strafe / Wait` spacing behavior. It supplies separate movement and facing directions but never applies Transform movement itself. |
| `EnemyStateController` | Owns coarse `Chase / Attacking / Staggered / Dead`, admits attack requests, accepts natural-finish notification, and owns ordinary surviving-hit duration, post-reaction protection and Global Attack Cooldown deadlines without exposing direct state writes. |
| `EnemyMovement` | Sole normal enemy Transform-facing and `CharacterController` displacement writer. `Move(moveDirection, facingDirection, speedMultiplier)` separates travel from facing, enabling retreat/strafe while looking at the target. It exposes actual local horizontal velocity for presentation and retains exact-distance attack movement plus recoil boundaries. |
| `EnemyAttack` | Holds serialized `MeleeAttackOption[] attackOptions`. Each entry pairs shared `MeleeAttackData` with this enemy's Weight. Two-pass selection first sums entries that are non-null, positive-Weight, horizontally range-legal and individually Ready, then rolls/subtracts Weight to choose one asset and lock it in runtime-only `currentAttackData` for the whole execution. A per-component Dictionary maps attack assets to scaled absolute ready times; accepted start consumes the selected attack's cooldown before Startup, while later Miss/cancel/Perfect Guard does not refund it. It owns internal `Ready -> Startup -> HitWindow -> Recovery -> Ready`, telegraph/Animator triggering, the saved target/threat, animation-relative tracking/footwork, hit-time validation, one `HitContext` delivery and idempotent cleanup. Cleanup clears the selected data and begins Global Attack Cooldown once. Shared asset configuration never owns per-enemy target, Weight, phase, timers or selection state. |
| `EnemyAnimator` | Converts actual local horizontal velocity into damped `Speed / MoveX / MoveZ` Animator parameters and exposes presentation-only HitReaction/Death requests. Damping changes visual response only; it grants no gameplay permission. |
| `EnemySpacingData` | Shared ScriptableObject configuration for attack-admission boundary, distance bands, behavior timing and movement-speed multipliers. It contains no current behavior, timer deadline, strafe side, target or attack state. |
| `EnemyHealth` | Subtracts integer damage and exposes IsAlive; object lifetime is no longer changed here. |
| `EnemyHitReceiver` | Rejects ineligible hits through CanReceiveHit; applies health, routes lethal damage to EnterDead (or disables state-less FarTarget), presents the accepted hit, then requests surviving reaction. Static IsValidTarget is shared by PlayerCombat and PlayerTargeting. |
| `EnemyHitPresentation` | Owns confirmed-hit VFX/SFX references, anchor placement, per-instance scale and cleanup. It spawns an independent Blood effect and a temporary CombatAudioPlayer Prefab so lethal deactivation does not own the feedback lifetime. |

Current spacing flow is:

```text
EnemyAI while Chase
-> compute horizontal direction and distance
-> inside outer attack range: face gate, then TryStartAttack first
   -> accepted: stop ordinary movement; EnemyStateController enters Attacking
   -> rejected: close-range Retreat or Strafe/Wait fallback
-> outside outer attack range: Run/Approach distance-band decision
-> EnemyMovement.Move(moveDirection, facingDirection, multiplier)
-> CharacterController displacement plus movement-owned facing
-> EnemyAnimator reads actual local velocity and damps directional parameters
```

`Retreat` begins only inside its enter distance, continues until its larger exit distance, then calls the same timed Wait entry used by the fallback cycle. `Strafe` chooses a side once per entry, runs for one fixed configured duration and alternates with a randomized Wait. Because attack admission is checked before these fallback behaviors, their timers affect movement rhythm only and never add an independent attack cooldown.

Current enemy damage flow is:

```text
EnemyAI
-> EnemyStateController.TryStartAttack(PlayerHitReceiver)
-> admitted EnemyAttack.TryStartAttack(PlayerHitReceiver)
-> reject null/inactive target and scan attackOptions in Inspector order
-> choose first asset whose MinimumRange <= horizontal distance <= MaximumRange
-> lock that asset as currentAttackData for this execution
-> timed EnemyAttack.OpenHitWindow()
-> EnemyAttack.ApplyDamage(PlayerHitReceiver)
-> EnemyAttack.IsImpactValid(saved target)
-> construct HitContext(DamageAmount, Source, IncomingDirection)
-> PlayerHitReceiver.ReceiveHit(HitContext)
   -> handled Blocking coverage success: stop
   -> otherwise PlayerHealth.TakeDamage(int)
```

Current enemy natural-finish flow is:

```text
EnemyAttack.FinishRecovery()
-> EnemyAttack.CancelAttack() clears target/threat/timer/animation request/telegraph and restores internal Ready
-> EnemyStateController.FinishAttack()
-> coarse Attacking returns to Chase only if it is still the current state
```

EnemyAttack.OnDisable also invokes the same execution cleanup without choosing a coarse destination. This lets future Staggered/Dead transitions retain destination authority instead of cleanup forcing Chase.

Current Attack1 animation-relative movement flow is:

```text
EnemyAttack starts Base Layer.Attack1SwordShield
-> during [TrackingStartTime, TrackingEndTime), ask EnemyMovement.Turn(saved target direction)
-> calculate previous/current normalized movement progress over [MovementStartTime, MovementEndTime]
-> movementDistance * progress delta
-> EnemyMovement.MoveDuringAttack(transform.forward, frameDistance)
-> CharacterController.Move(normalized direction * exact incremental distance)
```

Tracking runs before displacement and has its own configured start/end interval; it is not required to begin with movement. Attack1 currently uses `0.033..0.067s` for tracking and `0.033..0.333s` for movement. Attack2's observed direction commit at frame 9 before movement frame 10 is the concrete reason for this separation. Later footwork retains the last applied facing. Cancellation resets the shared animation-relative timer, so it cannot resume stale tracking or displacement.

Current confirmed Player Attack hit flow is:

```text
PlayerCombat.OpenHitWindow(int)
-> preserve current step and saved-target-in-range confirmation
-> construct HitContext(CurrentAttackData.Damage, player Transform, source-to-victim direction)
-> EnemyHitReceiver.ReceiveHit(HitContext)
   -> EnemyHealth.TakeDamage(int)
   -> optional EnemyHitPresentation.PresentHit()
      -> independent FX_hit_03_Blood instance and timed cleanup
      -> independent temporary two-channel CombatAudioPlayer and timed cleanup
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
      ├─ Ordinary -> matching impact/audio + independent player reaction
      └─ Perfect -> matching impact/audio + shared Hitstop request
```

Range and horizontal facing angle are checked before attack Startup. When only range passes, EnemyAI stops translation and asks EnemyMovement to turn in place. At the later Hit Window, `EnemyAttack.IsImpactValid` rejects a null or disabled saved receiver and requires current horizontal distance within `ImpactRange` plus current direction within `MaximumImpactFacingAngle` of the attacker's committed `transform.forward`. A failed check returns before `HitContext` creation/`ReceiveHit`, while the already-entered HitWindow continues into Recovery and Global Attack Cooldown normally. This remains target-confirmed combat rather than weapon-collider or line-of-sight confirmation; Player death eligibility does not exist yet.

## Implemented Architecture Invariants

- Actual gameplay state and permission decisions do not belong in Animator transitions.
- Player Transform facing remains code-owned; Apply Root Motion stays off.
- Lock-On remains orthogonal to the coarse player action state.
- `PlayerActionController` remains the only coarse action-state owner.
- `EnemyStateController` remains the only coarse enemy-state owner; `EnemyAttackPhase` is internal execution state and does not grant AI permission.
- Attack natural finish and cancellation share one cleanup boundary.
- Same-frame mutually exclusive requests do not rely on `MonoBehaviour.Update()` execution order.
- Guard Hold movement permission never implies Sprint permission.
- Do not introduce a second gameplay FSM, numeric Priority system, general Request Queue, pre-emptive `PlayerMotor`, or large Damage/Ability Framework without a concrete need.

## Attack Motion Presentation

PlayerCombat validates indexed Trail, Whoosh and Windup Events against the current Attacking state and attack index, then forwards them to PlayerAttackPresentation. StartAttackStep and shared EndAttack close the transient Trail; presentation Awake/OnDisable also close it. The independent WeaponAura is not controlled by these windows.

Attack motion audio uses a separate CombatAudioPlayer instance from Guard audio. Each Play call stops that instance's previous channels before DSP scheduling the new cue. Attack4 Windup and main Whoosh use separate authored Events; other attacks use one Whoosh cue. Cancellation rejects future mismatched Events but does not explicitly stop already scheduled/playing motion audio. Index validation does not uniquely distinguish two executions of the same attack step; do not claim a general execution-token guarantee.

## Configuration and Planning References

VFX assets, placement and Trail Event values: COMBAT_VFX_RESOURCE_TRACKING.md. Audio clips/mixes and sound Event values: COMBAT_SFX_RESOURCE_TRACKING.md. Guard timing, reaction and shared soft-recovery parameters: GUARD_REACTION_DESIGN.md. These are configuration/design references; actual code/assets still take precedence.

Enemy future state and consequence plans are in ENEMY_COMBAT_AGENT_DESIGN.md. The coarse Enemy state owner, reliable EnemyAttack cleanup, ordinary HitReaction/protection, Global Attack Cooldown and start-time range/facing admission are implemented; retained-corpse Death is implemented with basic runtime acceptance; Strong Combo and Poise are not. See CURRENT_STATE.md for remaining Death boundary tests.

## Retained-corpse Death boundary

EnemyHitReceiver checks eligibility before damage, then immediately calls EnterDead on lethal damage when a state controller exists. EnterDead sets Dead, cancels EnemyAttack and calls EnemyAnimator.PlayDeath. EnemyAI stops decisions/movement outside Chase; existing reaction/finish guards cannot restore Dead to Chase. Accepted lethal hit presentation still runs independently. FarTarget has no state controller and retains a receiver-owned disable fallback.

CanReceiveHit checks enabled/active receiver, initialized health, positive health and optional non-Dead state. IsValidTarget also requires an enabled Collider with a receiver on the same object. Both candidate searches, attack range confirmation and facing/lunge use it. Lock-On getters compute live eligibility before Update clears the backing reference. Corpse physical collision is unchanged. Exact same-frame lethal/impact precedence is not implemented as a separate arbitration boundary.

## Perfect Guard Stagger implementation (2026-09-10)

PlayerHitReceiver.ReceiveHit returns HitResult.Damaged, OrdinaryGuard or PerfectGuard after its existing resolution/presentation path. EnemyAttack consumes PerfectGuard only while still HitWindow/Attacking, then requests EnemyStateController.TryStartPerfectGuardStagger. The controller enters Staggered, sets the existing end deadline using perfectGuardStaggerDuration, cancels attack and starts slow presentation. It bypasses ordinary reaction cooldown, but requires Attacking. Existing timed exit returns to Chase and starts reaction protection; global attack cooldown continues independently.

EnemyAnimator owns only isPlayingPerfectGuardStagger presentation state: entry/repeated surviving hits directly play Base Layer.PerfectGuardStagger; gameplay expiry calls FinishPerfectGuardStagger, clears the flag and blends to Idle over 0.1 seconds. Death clears the flag and directly selects Death. Separate state Speed 0.7 does not change global or per-enemy time. Gameplay duration and animation speed are separately tuned.

## Attack Hitstop and paused movement (2026-09-10)

EnemyHitPresentation.PresentHit requests the existing shared HitstopController after spawning VFX/SFX; victim deactivation does not own time restoration. PlayerMovement.Update still resolves action requests during pause, then skips movement processing when deltaTime <= 0. MoveDuringAttack rejects zero-time/nonpositive-distance moves. This preserves grounding evidence during Hitstop without removing grounded Block admission or adding input buffering.

## Hit recoil implementation (2026-09-10)

EnemyMovement owns configurable distance/duration, direction, elapsed time and active flag. LateUpdate requests the difference of successive 2t-t*t positions through CharacterController.Move. BeginRecoil replaces remaining motion; CancelRecoil and OnDisable clear it. Normal Move/Turn/Stop skip during recoil or zero deltaTime. EnemyStateController.TryStartHitReaction(Vector3) requests recoil only on admitted ordinary reaction or existing Staggered; deadlines are unchanged by repeated hits. TryStartAttack rejects IsRecoiling, and EnterDead cancels it immediately. HitContext direction is passed unchanged by EnemyHitReceiver; recoil flattens and normalizes it. No global/local time writer or root motion is added.

## Independent melee attack assets, range selection and cooldowns

`MeleeAttackData` is a ScriptableObject configuration type with Project creation path `Relic Guardian/Enemy/Melee Attack Data`. `Goblin_Attack1.asset` and `Goblin_Attack2.asset` independently expose Selection, Cooldown, Phase Timing, Animation, Motion and Impact values through read-only accessors. Tracking and movement remain separate intervals over one animation-relative execution timer. Shared assets store immutable-per-execution move configuration only; mutable ready-time deadlines remain in a Dictionary owned by each `EnemyAttack` component. `MeleeAttackOption` is a serializable per-owner list entry, not another asset or executor; it pairs one shared data reference with contextual Weight.

`EnemyAttack` treats an attack with no deadline entry as Ready, otherwise requires `Time.time` to reach the saved deadline. Eligibility also requires a non-null option/data reference, positive Weight and horizontal range legality. The first pass sums eligible Weight; zero total rejects admission. The second pass rolls `Random.Range(0f, totalWeight)`, subtracts eligible shares and locks the first asset that reaches zero or below. Accepted start writes `Time.time + CooldownDuration` before Startup; later Miss, Perfect Guard interruption and cancellation do not refund it. The selected data is never changed while Startup/HitWindow/Recovery is active. NearTarget saves Attack2/Attack1 Weights `3 / 1` and cooldowns `4s / 0s`. Range, per-attack cooldown and weighted selection are runtime-verified. Future ranged attacks still require a separate execution/data family.

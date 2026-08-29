# Relic Guardian Roadmap

## Overall Progress

- [x] Unity project created
- [x] Development environment configured
- [x] Project documentation initialized
- [x] Git repository configured

---

## Phase 1 - Foundation

- [x] Project folder structure
- [x] Starter Assets imported (reference only; do not modify official source code)
- [x] Third Person Controller
- [x] Camera system (FreeLook orbit, player tracking, and camera-relative movement tested)
- [x] Character movement (horizontal movement, turning, gravity, and grounded reset tested)
- [x] Jump
- [x] Sprint

### Current Checkpoint

- Custom player scripts live in `Assets/RelicGuardian/Player/Scripts/`.
- `PlayerInputReader` reads `Move` and `Look` and exposes a one-use Jump request through `ConsumeJump()`.
- `PlayerMovement` contains tested camera-relative horizontal movement, smooth turning, gravity, grounded velocity reset, and grounded jumping.
- `PlayerAnimator` synchronizes locomotion `Speed` and `MotionSpeed`; pose blending and animation playback were verified together without frozen sliding.
- `PlayerMovement.IsGrounded` exposes the existing `CharacterController` state, and `PlayerAnimator` synchronizes it to the Animator's `Grounded` bool parameter. Its `true -> false -> true` sequence was verified across jump and landing.
- `PlayerMovement.IsJumping` preserves accepted-jump state across frames until landing, and `PlayerAnimator` synchronizes it to the Animator's `Jump` bool parameter. `JumpStart`, the parameter lifecycle, and landing reset were verified.
- `PlayerMovement.IsFalling` derives the airborne descending state from Grounded and vertical velocity, and `PlayerAnimator` synchronizes it to `FreeFall` without duplicate stored state.
- The `Jump` Button action is bound to Space. Jumping, blocking an airborne second jump, landing, and jumping again were verified in Play Mode with no Console errors or warnings.
- A Cinemachine 3.1.7 FreeLook Camera tracks `PlayerCameraRoot`; mouse orbit, player follow, camera-relative WASD, turning, and Jump were verified together.
- The complete `JumpStart -> FreeFall -> JumpLand` chain, moving Jump, blocked airborne second Jump, repeat Jump after landing, and camera-relative movement were verified together with a clean Console.
- The Week 1 short review verified request-versus-state reasoning, derived-state reasoning, Animator parameter types, Console input data flow, and the complete Jump animation component chain.
- The custom `Player` Input Action map now includes an `Attack` Button action bound to the left mouse button.
- `PlayerInputReader` stores a one-use Attack request through `OnAttack()` and exposes it through `ConsumeAttack()`.
- `SwordAndShieldSlash` is integrated into the project-owned `RelicGuardianPlayer.controller`; the official Starter Assets Controller remains unmodified.
- The `BasicAttack` Animator state uses playback speed `1.5`, and its Animation Event explicitly returns the action state to `Free`.
- `PlayerActionController` is attached to the reusable player Prefab and owns the current `Free`/`Attacking` coarse action state.
- Grounded Basic Attack, repeated-request rejection, explicit attack-end recovery, movement/turn/Jump blocking and recovery, gravity continuity, airborne request rejection without buffering, and Attack/Jump mutual exclusion were verified in Play Mode.
- Same-frame Attack/Jump has no fixed winner because `PlayerCombat` and `PlayerMovement` both use script execution order `0`; the current checkpoint guarantees mutual exclusion only.
- `PlayerCombat` owns the Basic Attack Hit Window through `isHitWindowOpen`, exposes it read-only through `IsHitWindowOpen`, and receives explicit open/close signals from the animation.
- `SwordAndShieldSlash` opens the Hit Window at Frame 18 and closes it at Frame 30, before the existing attack-end Event near Frame 40.
- One grounded attack produced exactly one open Event followed by one close Event. Temporary verification logs were removed, the final scripts compiled, the attack recovered normally, and the Console remained clean.
- Added the `HitTarget` Layer, an Inspector-configured `LayerMask`, and a `Physics.OverlapSphere` candidate query with prototype radius `2`.
- `PlayerCombat` selects the nearest candidate by horizontal center distance when a grounded Basic Attack is accepted and stores it in `currentAttackTarget` before starting the animation.
- Runtime checks verified one in-range candidate, one range-valid side candidate, and two simultaneous candidates; the two-target test selected `NearTarget` at horizontal distance `1.2` instead of `FarTarget` at `1.6`.
- The current design is soft targeting: the circular query finds candidates, the current Basic Attack selects one target, and future attacks may use different selection policies. The query is not circular area damage.
- `PlayerMovement.FaceDirection(Vector3 direction)` now owns explicit facing requests and applies one `Quaternion.Slerp` rotation step after rejecting a zero direction.
- At accepted attack start, `PlayerCombat` enables `isAttackFacingActive` only when the saved target exists, requests facing every frame toward that same target, and disables tracking when `OpenHitWindow()` begins.
- Runtime checks verified smooth startup facing toward the nearest target, no recovery-phase tracking, unchanged saved-target damage, and a clean final Console.
- At `OpenHitWindow()`, `PlayerCombat` re-queries the current candidates and confirms only the exact `currentAttackTarget` saved at attack start. A missing or escaped target produces no confirmation, and another candidate is never substituted.
- `CloseHitWindow()` clears the confirmed target. Temporary runtime logs verified the confirmed and missed paths before being removed; the final script compiles with no diagnostics, and the no-log attack/recovery regression ended with an empty Console.
- Added a minimal `EnemyHealth` component with serialized prototype `currentHealth = 3` and `TakeDamage(int damageAmount)`. `NearTarget` and `FarTarget` each own an independent instance.
- When `OpenHitWindow()` confirms the saved target, `PlayerCombat` obtains that target's `EnemyHealth` and calls `TakeDamage(1)`. It still never retargets or damages another candidate.
- Runtime verification confirmed an isolated `3 -> 2` damage call with the saved value restored after Play Mode, then one real attack changed `NearTarget` from `3 -> 2` while `FarTarget` remained `3`. The final Console was empty.
- A separate Unity audit project verified the P09 male Humanoid character, correct materials with lilToon `2.3.3`, and the selected `M_katana_Blade@Attack_3Combo_1_Move` animation with visible forward Transform displacement.
- The main project has the pinned lilToon `2.3.3` dependency with zero post-install Console errors. The selected attack FBX remains outside the main project.
- P09 is imported locally under `Assets/P09_Modular_Humanoid/`; its approximately `483 MB` licensed asset folder is ignored by Git and must be restored from the exact package on another workstation. Its bundled lilToon `1.x.x` and MagicaCloth installers were not run. The post-import Console is empty.
- `P09_Human_Variant_Female` now provides the visible player model beneath the preserved `RelicGuardianPlayer` root. The prototype `Geometry` and `Skeleton` branches are inactive, while gameplay components, `PlayerCameraRoot`, and the single active Animator remain root-owned.
- The P09 visual and its player-Prefab reference changes are local-only; the code-focused repository intentionally keeps the previous tracked prototype Prefab.
- The root Animator uses `P09_BodyAvatar`, the nested P09 Animator is disabled, Apply Root Motion remains off, and ordinary movement passed Play Mode verification. Minor retargeting clipping is deferred until animation replacement.
- The local P09 weapon boundary is now explicit: constrained `RightHandWeaponSocket` owns the reusable hand attachment, while `P09Sword02Visual` is its replaceable identity-transform child. The existing idle, movement, and Basic Attack passed Play Mode checks with the sword following the hand; damage remains independent of the model.
- The ignored local `KatanaAnimationOverrides` preserves the project-owned Animator state machine and replaces only `SwordAndShieldSlash` with the validated Humanoid katana clip. Frames approximately `11`, `20.6`, and `37` preserve the Hit Window and action-end callback contract; Root Motion remains off and the full regression passed with an empty Console.
- `PlayerHealth` now defines the player-owned prototype health receiver with serialized value `3` and `TakeDamage(int damageAmount)`. It is attached to the local player root, and a non-persistent component test verified `3 -> 2`; attack timing and death logic are not included yet.
- `EnemyAttack` now defines an enemy-owned damage boundary with value `1`, and the automatic Hit Window calls it once against the target saved for the current attack. A temporary component test verified player health `3 -> 2` only when the Hit Window opened.
- `EnemyAttackPhase` represents `Ready`, `Startup`, `HitWindow`, and `Recovery`. `EnemyAttack.TryStartAttack(PlayerHealth target)` accepts only a non-null target while `Ready`, saves that target, and then enters `Startup`.
- `EnemyAttack.OpenHitWindow()` accepts only `Startup -> HitWindow` and applies one damage result to the saved target during that successful transition. Repeated open attempts do not apply another result.
- `EnemyAttack.CloseHitWindow()` now accepts only `HitWindow -> Recovery`. A temporary component test verified rejected early/repeated calls and the complete accepted sequence through `Recovery`.
- The learner added `EnemyAttack.FinishRecovery()`, accepting only `Recovery -> Ready`; it now also clears the completed attack's saved target. A temporary component test verified the complete repeatable phase loop, target cleanup, and a second accepted attack start with a different target.
- `EnemyAttack.phaseElapsedTime` now resets on every accepted transition and accumulates with `Time.deltaTime` only outside `Ready`. Runtime checks verified accepted-reset, rejected-preserve, `Ready` remaining at `0`, and `Startup` accumulation.
- Serialized durations now drive the complete automatic phase loop: `Startup (0.5s) -> HitWindow (0.2s) -> Recovery (0.4s) -> Ready`. A temporary component test verified every reset, `Ready` remaining at `0`, and a second successful attack start.
- `EnemyAI` now acts as the first real gameplay caller. It uses Inspector-assigned `EnemyAttack` and `PlayerHealth` references, calculates their `Vector3.Distance` each frame, and requests an attack only while `distanceToTarget <= attackRange`.
- Opposite runtime tests at an approximately `1.2`-unit separation verified no damage with `attackRange = 0.5` and one `3 -> 2` player-health change with `attackRange = 2` after the existing Startup. `EnemyAI.cs` and the final Console both reported zero errors and zero warnings.
- Per-frame in-range requests do not reset the attack because `EnemyAttack.TryStartAttack()` accepts only while `Ready`. Target search, chase, and attack cancellation after leaving range remain separate later concepts.
- `EnemyAttack` now owns an Inspector-assigned `startupTelegraph` object reference. A successful attack entry activates the red placeholder for `Startup`, and the successful Hit Window entry deactivates it. Static validation, a non-persistent phase test, and the learner's real-scene visual check all verified the expected visible interval.
- The `3,030,509,498`-byte Heroic Fantasy Creatures Vol.1 archive was imported only into the isolated AssetLab for inspection. The main project contains the narrowed Generic-Rig, URP/Lit sword-and-shield Goblin model/Prefab, required materials and textures, plus only the selected in-place Idle, Attack, Walk, and Run animations.
- Health clamping, production death presentation, combo extension beyond Attack2, Dodge, hitstop, and effects remain deferred.
- Basic Attack now uses a code-driven limited-distance startup lunge toward the same target saved at attack start. It does not retarget; if the target escapes the lunge and Hit Window range, the attack misses. `PlayerMovement` remains responsible for `CharacterController` displacement, and opposite in-range/no-target Play Mode tests passed.
- The active local presentation is now `Attack_4Combo_1_Inplace`, and looping `Idle_ver_B` is the local equipped-weapon Idle override. The learner approved the final attack-to-idle transition after matching Root Transform Rotation; Apply Root Motion remains off.
- `PlayerAttackData` now provides per-step damage, target range, lunge speed, and lunge distance. The Prefab has two configured prototype entries, and `PlayerCombat` advances through them with reusable indexed initialization.
- The complete reusable indexed Attack1-4 combo is runtime-verified. Attack1-3 have Combo and Restart windows, all Animation Events carry step identity, all four Hit Windows apply one damage result, invalid timing is rejected, and Attack4 restores movement/Jump with clean final state. The first Guard design is fixed, but Block gameplay, Dodge, and general recovery cancellation remain unimplemented.
- The first usable lock-on mode is implemented: `V` toggles nearest-target lock, inactive or `12m`-distant targets release automatically, locked movement faces the authoritative target, combat prioritizes it without fallback, and two Cinemachine cameras blend through priorities. The accepted prototype lock camera tracks `PlayerCameraRoot`, looks at a weighted `LockOnCameraTarget`, permits limited manual orbit, and recenters automatically.
- Locked locomotion now uses the complete non-Root Katana `Jogging_8Way_verB` family through a project-owned 2D Blend Tree and tracked placeholder Clips. Player-local X/Z direction values and `0.1s` Animator damping drive the accepted eight-direction presentation.
- Free locomotion uses the forward Katana jog for normal travel and `Run_ver_B` for held Sprint. Shift plus movement while locked cancels lock and enters free Sprint; Shift alone preserves lock. Root Motion remains disabled.
- Free/lock camera handoff now uses incoming-position inheritance, outgoing-lock-camera freezing, and state-aware input-axis ownership. The learner accepted the final unlock response, free sensitivity, and blend time.
- Both locomotion modes enter the same four-step Basic Attack chain. The first locked combat rule rejects Jump while locked; locked rejection, unlocked grounded acceptance, and the locked combo return have all passed the deferred runtime regressions.
- Block input, centralized Block/Attack/Jump arbitration, the coarse `Blocking` state, and the phase-aware `PlayerBlock` Startup/Hold/Release lifecycle are implemented and runtime-verified.
- `PlayerCombat` routes natural finish and Block cancellation through one `EndAttack()` cleanup boundary. Stale Events are rejected after cancellation.
- `PlayerAnimator` presents Guard through code-driven `Block_Start`, `Block_Loop`, and `Block_End` states and implements interruptible soft recovery for Guard and Attack visual tails.
- Guard rotation and normal `0.45s` exit blending are accepted. Attack4's ignored local `FinishAttack(3)` Event was moved earlier to create a tested soft tail.
- Guard Hold unlocked Idle/Forward, locked 8-Way, and active-Hold Lock-On presentation refresh are runtime-verified. Startup facing assistance, Perfect Guard, and Damage / Defense Resolution remain pending.

---

## Phase 2 - Combat

- [ ] Basic attack
  - [x] Integrate one grounded Basic Attack input and animation without modifying Starter Assets.
  - [x] Add minimal player-action coordination, then migrate the coarse attack state from `BasicAttack` to `Attacking` without changing the four-step attack flow.
  - [x] Define `PlayerActionController` as the authoritative owner for the current player action.
  - [x] While `Attacking` is active, block horizontal movement, turning, and Jump while gravity and grounded handling continue.
  - [x] Define and verify an explicit attack-end Animation Event before returning to `Free`.
  - [x] Reject repeated and airborne grounded-Basic-Attack requests without buffering.
  - [x] Define and verify the Basic Attack Hit Window.
  - [x] Add circular candidate acquisition and verified nearest single-target selection at attack start.
  - [x] Smoothly turn toward the saved target during startup through a reusable movement-owned facing method, then stop at the Hit Window.
  - [x] Re-check and confirm only the saved target when the Hit Window opens, without retargeting.
  - [x] Add a minimal enemy-health receiver and apply one damage result to the confirmed target.
  - [x] Validate one P09 Humanoid character and one movement attack clip in an isolated Unity project.
  - [x] Install pinned lilToon `2.3.3` in the main project without importing the character package's old nested installer.
  - [x] Replace only the prototype visual-model boundary with P09 female while preserving the existing player root and gameplay components; verify ordinary movement.
  - [x] Establish a reusable local right-hand weapon socket and one replaceable P09 sword visual without moving damage or equipment logic onto the model.
  - [x] Integrate the verified katana attack clip locally through an Animator Override Controller while preserving the existing three-event gameplay contract and keeping Root Motion off.
  - [x] Choose `PlayerMovement` plus `CharacterController` as the future finite startup-lunge owner while preserving the saved target and no-retarget rule.
  - [x] Implement and runtime-verify the bounded startup lunge, stopping it at the Hit Window or maximum travel distance and leaving no-target attacks stationary.
  - [ ] Add a target stop-distance refinement if later collision/overlap testing shows it is needed.
- [ ] Combo attack
  - [x] Preview mixed candidate sequences on the P09 female character and choose the final four light-attack clips.
  - [x] Import the four selected non-looping Humanoid clips into the ignored local main-project asset boundary.
  - [x] Connect and regression-test only `Attack_4Combo_1_Inplace` as the new Attack1 while preserving one damage result, recovery, and Root Motion off.
  - [x] Extract the existing single-attack configuration into minimal inline `PlayerAttackData` and verify the one-entry behaviour remains unchanged.
  - [x] Add `currentAttackIndex`, reusable `StartAttackStep(int)`, and independent Hit/Combo Window runtime boundaries while keeping only Attack1 active.
  - [x] Author Attack1 `OpenComboWindow` and `CloseComboWindow` events separately from the Hit Window.
  - [x] Add `isAttackQueued` and runtime-verify that Attack input queues only while the authored Combo Window is open.
  - [x] Add and runtime-verify the authored `ComboTransitionPoint` runtime-ready boundary.
  - [x] Add Animator `AttackIndex`, a tracked Attack2 placeholder/state, local Attack2 override, indexed transitions, data, and Hit/finish Events.
  - [x] Consume queued input at or after the transition point through the reusable indexed path and runtime-verify the two-hit acceptance paths.
  - [x] Add attack-step identity parameters to all Animation Events and reject mismatched stale events in `PlayerCombat`.
  - [x] Add explicit late-recovery Restart Windows for Attack1-3; their input resets the chain through the reusable Attack1 path.
  - [x] Build and verify the reusable indexed attack flow and Combo Window with Attack1 and Attack2 before extending it to all four selected clips.
  - [x] Configure Attack3/4 data, tracked placeholders, local overrides, Animator states/transitions, and indexed Animation Events without adding copied combat-flow methods.
  - [x] Run and record the complete four-hit acceptance pass, including no-follow-up endings, four damage windows, Restart from Attack1-3, invalid timing, final recovery, and a clean Console/state snapshot.
  - [x] Add and independently verify one centralized cancellation-cleanup boundary for the current four-step Basic Attack. Natural finish and `TryCancelAttack()` share `EndAttack()`; future skills retain explicit interruption restrictions.
- [x] Enemy health
  - [x] Store prototype current health and receive a supplied damage amount.
  - [x] Define and runtime-verify the zero-health boundary and minimal inactive-object death response.
- [ ] Damage system
  - [x] Apply one fixed prototype damage result to the saved, Hit Window-confirmed target.
  - [x] Define reusable per-step damage, target range, lunge speed, and lunge distance through inline `PlayerAttackData`; Attack1 and Attack2 are configured with prototype values.
- [ ] Minimal enemy attack loop
  - [x] Add a player-owned health receiver boundary.
  - [x] Connect one enemy-owned damage call to the player receiver and verify `3 -> 2`.
  - [x] Define `Ready`, `Startup`, `HitWindow`, and `Recovery`, and accept only `Ready -> Startup` at the attack entry.
  - [x] Add `OpenHitWindow()` with only `Startup -> HitWindow`.
  - [x] Add `CloseHitWindow()` with only `HitWindow -> Recovery`.
  - [x] Add the guarded recovery-completion transition from `Recovery` to `Ready`.
  - [x] Add and verify the non-`Ready` phase elapsed-time foundation.
  - [x] Add configurable `Startup`, `HitWindow`, and `Recovery` durations and verify the complete automatic timed loop back to `Ready`.
  - [x] Carry one non-null `PlayerHealth` target through an accepted timed attack, apply damage once when the Hit Window opens, and clear the target after recovery.
  - [x] Add and verify one Inspector-wired range gate that requests the timed attack only while the player target is within `attackRange`.
  - [x] Add one telegraphed enemy attack with a separate startup, Hit Window, and recovery.
  - [x] Integrate the minimal sword-and-shield Goblin visual and drive one project-owned Idle/Attack state machine from the accepted attack entry.

---

## Phase 3 - Combat Feel

- [ ] Hitstop
- [ ] Knockback
- [ ] Hit VFX
- [ ] Hit SFX
- [ ] Camera Shake

---

## Phase 4 - Advanced Combat

- [ ] Basic Block
  - [x] Choose held input, grounded `Free` entry, rejected-request non-buffering, and separate `PlayerBlock` coordination.
  - [x] Define Block as higher action priority than the current four-step Basic Attack: Block wins their same-frame Free-state conflict and may interrupt an active Basic Attack throughout Startup, Hit Window, and Recovery.
  - [x] Keep future skill interruption restrictions explicit. Higher Block priority does not override a skill that has no legal Block-cancel transition.
  - [x] Define cancellation damage behavior: cancellation before `OpenHitWindow()` prevents damage; damage already applied is never rolled back.
  - [x] Keep `Blocking` as one coarse `PlayerActionState` while `PlayerBlock` owns internal Startup, Hold, and Release phases; Animator remains presentation-only.
  - [x] Define Startup as short and non-moving with required facing correction, Hold as movable but unable to Sprint, and Release as short non-moving recovery.
  - [x] Define early Startup release as a request to enter Release only at an authored decision/exit point, not an immediate hard cut to `Free`.
  - [x] Place the future Perfect Guard Window inside Startup and prefer authored Animation Events, without implementing damage resolution yet.
  - [x] Preserve existing free and lock-on movement/facing behavior during Hold; Guard never changes camera mode.
  - [x] Define unlocked Startup facing assistance as one temporary target search in the forward `120` degrees (`+/-60` degrees) without changing `PlayerTargeting.CurrentTarget`; locked Startup reuses the current authoritative target.
  - [x] Define future ordinary Guard coverage as the forward `180` degrees (`+/-90` degrees), while deferring attack-source data and Damage / Defense Resolution until that feature begins.
  - [x] Accept the isolated `SwordAnimationPack` Block and Block-walk presentation as visually compatible with the current Katana set; main-project integration and runtime verification remain pending, with Root Motion off.
  - [x] Establish that simultaneous Attack/Block acceptance must not depend on `MonoBehaviour.Update()` order; their fixed first-version result is Block winning over Basic Attack.
  - [x] Add only Block input representation: a `Pass Through` action, persistent held state, a one-use press edge, and one mouse-right-button binding so rejection is not buffered.
  - [x] Add and verify centralized Basic Attack cancellation cleanup for windows, queue, targets, facing, lunge, travelled distance, and attack index.
  - [x] Approve `PlayerActionController` as the unique Block/Attack/Jump decision point, with one idempotent resolution per frame and no `HasBlockRequest`, distributed request peeking, Script Execution Order dependency, numeric Priority system, or general Request Queue.
  - [x] Migrate Attack and Jump raw-request consumption into the central one-per-frame decision point and regress the existing attack, combo, restart, Jump, movement, lunge, and Console behavior.
  - [x] Connect the six approved deterministic Block results and the minimum `Free / Attacking -> Blocking -> Free` coarse transitions through the central arbiter.
  - [x] Add `PlayerBlock` Startup/Hold/Release lifetime and authored decision/exit points without damage behavior.
  - [x] Present the lifecycle through code-driven `CrossFadeInFixedTime()` using `Block_Start`, `Block_Loop`, and `Block_End`; preserve Root Motion off and the existing four-step combo.
  - [x] Add interruptible Guard/Attack soft recovery after gameplay Finish Events while preserving natural visual tails when no follow-up is accepted.
  - [x] Add phase-specific translation and Sprint permission while preserving existing unlocked and locked Hold movement/facing behavior.
  - [x] Add unlocked Guard Hold Idle/Forward presentation without creating an eight-way unlocked strafe mode.
  - [x] Add locked Guard Hold 8-Way presentation using the existing `MoveX` / `MoveZ` parameters.
  - [x] Regress unlocked/locked Hold movement, Release, Sprint rejection, Lock-On retention, and the Console after locomotion presentation integration.
  - [x] Audit `Turn_Block_90_L/R` and `Turn_Block_180_L/R` Loop, Root Transform Rotation, direction, and P09 presentation.
  - [x] Run and then cleanly remove a minimum unlocked-Hold Turn presentation experiment after runtime exposed its mismatch with interruptible smooth movement-facing.
  - [ ] Reconsider fixed-angle Turn Clips only if a later turn-in-place design defines committed facing, input interruption, and Transform/animation synchronization.
  - [x] Refresh Hold presentation once when Lock-On mode changes during an already-active Hold.
  - [ ] Add the one-shot Startup facing assistance without mutating the authoritative lock target.
  - [ ] Add the authored Perfect Guard Window lifetime without successful-Guard resolution.
  - [x] Run focused lifecycle, movement, camera-mode, early-release, and Console regressions.
  - [ ] Later design and implement the forward-arc Damage / Defense Resolution boundary without prebuilding a general Hit Framework.
- [ ] Dodge
- [ ] Perfect Dodge
- [x] Lock-on (first usable version)
  - [x] Add nearest-target acquisition, `V` toggle, authoritative current target, and automatic break conditions.
  - [x] Add free/locked movement-facing modes while preserving CharacterController movement and Root Motion off.
  - [x] Give the locked target combat priority without fallback to a different soft target.
  - [x] Add separate free and lock Cinemachine cameras with priority blending and player-relative locked orientation.
  - [x] Add limited manual lock-camera orbit, recentering, and a weighted camera LookAt target.
  - [x] Add locked directional locomotion animations using selectively imported in-place Katana clips.
  - [ ] Add multi-target switching, lock UI, camera occlusion, and final composition polish when required.

---

## Phase 5 - Enemy AI

- [ ] State Machine
- [ ] Patrol
- [ ] Chase
  - [x] Import and preview non-Root-Motion sword-and-shield Walk and Run clips; reserve Walk for patrol and Run for chase.
  - [x] Add and manually verify `Speed`-driven `Idle <-> Run` Animator transitions.
  - [x] Add a root `CharacterController` locally and create the reusable tracked `EnemyMovement.Move(Vector3 direction)` displacement boundary.
  - [x] Add movement-owned smooth facing, attach `EnemyMovement`, and connect out-of-range chase requests from `EnemyAI`.
  - [x] Expose actual horizontal speed from `EnemyMovement` for presentation consumers.
  - [x] Synchronize the Goblin Animator `Speed` parameter from actual movement and runtime-verify chase-to-attack behaviour.
  - [x] Add an explicit zero-movement boundary and prevent chase from resuming before the active enemy attack returns to `Ready`.
- [ ] Attack
- [ ] Death

---

## Phase 6 - Demo

- [ ] Level
- [ ] Boss
- [ ] UI
- [ ] Polish

---

## Phase 7 - Optimization

- [ ] Object Pool
- [ ] ScriptableObject
- [ ] Addressables
- [ ] Basic Profiling

---

## Phase 8 - Portfolio

- [ ] GitHub
- [ ] README
- [ ] Gameplay Video

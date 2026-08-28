# Relic Guardian Current State

Last reviewed against the local workspace: 2026-08-28

This is the short current-state entry point. Actual code, Unity assets, current Editor state, and Git status remain authoritative when they conflict with this file.

## Project Environment

- Local project: `C:\Unity\Project\My project`
- Unity: `6000.3.19f1`
- Render pipeline: URP `17.3.0`
- Input System: `1.19.0`
- Cinemachine: `3.1.7`
- Movement: `CharacterController`
- Apply Root Motion: off
- Target platform: Windows

## Git Boundary

- Local full-project repository: `C:\Unity\Project\My project`; current line: local `main`.
- GitHub code/document mirror: `LearnedYet/Relic-Guardian-Code`; current line: remote `main`.
- Pre-checkpoint local full-project commit: `4719b71 Record Guard design and attacking migration`.
- Pre-checkpoint confirmed GitHub mirror sync: `5db2fed Sync Guard planning and ChatGPT project context`.
- The local full-project repository and GitHub mirror have different history shapes. Resolve each current tip in its own repository; never compare their hashes as one ancestry or directly pull/merge mirror `main` into the full Unity workspace.
- The mirror stores flattened project-owned C# files at its root and selected reproducible Unity configuration under `UnityConfig/`; it is not a complete Unity-project clone.

The following protected mixed Unity assets remain intentionally modified locally and must not be reset, restored, overwritten, or staged without a separate explicit review:

- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/Scenes/SampleScene.unity`

Their local wiring includes licensed presentation and the current Scene-level `PlayerBlock` component. They remain outside the focused code/document checkpoint.

`Assets/LocalLicensed/` is ignored and must never be committed or uploaded. It contains the P09 presentation, Katana/Sword animation assets, Animator Override mappings, and local Animation Event/import tuning. The locally verified Attack4 `FinishAttack(3)` time is therefore documented but not mirrored as an asset change.

## Implemented Player Action Architecture

- `PlayerActionController` is the sole coarse gameplay-state owner. Current states are `Free`, `Attacking`, and `Blocking`.
- `ResolveActionRequests()` is the unique Block / Attack / Jump request-arbitration point. An idempotent `Time.frameCount` gate makes all callers observe one result per frame without relying on `MonoBehaviour.Update()` order.
- The fixed first-version order is Block, then Attack, then Jump. Accepted grounded Block wins same-frame conflicts and suppresses horizontal movement on that frame. Rejected requests are consumed without buffering.
- `PlayerInputReader` only records input. Block uses persistent `IsBlockHeld` plus the one-use `ConsumeBlock()` press edge, with a `Pass Through` right-mouse-button Input Action.
- `PlayerCombat` owns the four-step attack flow. Natural finish and Block cancellation share `EndAttack()`, which clears windows, queue/restart state, targets, facing, lunge, distance, and index before releasing `Attacking`.
- Animation Events carry attack-step identity; stale events are rejected when the coarse state or step index no longer matches.

## Implemented Guard Lifecycle

- `PlayerBlock` owns the internal `Startup`, `Hold`, and `Release` phases while `PlayerActionController` owns the single coarse `Blocking` state.
- Grounded Block enters from `Free`. It can also cancel the current four-step Basic Attack before entering `Blocking`.
- `BeginBlock()` starts Startup. `StartupDecisionPoint()` enters Hold if Block is still held or Release if it was released. Releasing during Hold enters Release. `FinishRelease()` returns to `Free`.
- `PlayerAnimator` presents `Block_Start`, `Block_Loop`, and `Block_End` using code-driven `CrossFadeInFixedTime()`; there are no Animator-authored gameplay permission decisions.
- The local copied Clips are `Block_Start.anim`, `Block_Loop.anim`, and `Block_End_NoRootTurn.anim`; the Animator state remains named `Block_End`. Their facing correction was runtime-accepted after matching the Guard root-rotation offsets at `-66`.
- Local Animation Events are `StartupDecisionPoint` at `0.4s` in `Block_Start` and `FinishRelease` at `0.75s` in `Block_End_NoRootTurn`.
- The current Scene tuning is Guard crossfade `0.03s`, normal Block End -> Locomotion exit `0.45s`, and soft-recovery interruption crossfade `0.05s`.
- Apply Root Motion remains disabled. The current Guard lifecycle is stationary because `CanMove` still allows ordinary translation only in `Free`.

## Implemented Soft Recovery

- Soft recovery is a presentation lifetime, not a new `PlayerActionState`: it begins after a gameplay `Finish...` Event has returned the action to `Free` while the authored Clip still has a visual tail.
- With no accepted follow-up, the authored tail continues naturally. Movement, a new Basic Attack, or Block can interrupt that visual tail without reopening old damage, lunge, or targeting state.
- `PlayerAnimator` records both whether soft recovery is active and whether its transition has actually started, preventing the flag from clearing during the Animator's one-frame transition-reporting delay.
- Normal Guard exit retains the accepted `0.45s` visual blend. Movement/action interruption uses the separate short `softRecoveryInterruptCrossFadeDuration`.
- Attack uses its existing `FinishAttack` -> visual clip end interval as soft recovery. No separate Attack exit-duration field or general request queue was added.
- The local licensed Attack4 Clip `Attack_3Combo_3_Inplace` now uses `FinishAttack(3)` at normalized time `0.59016937`; its `OpenHitWindow(3)` and `CloseHitWindow(3)` remain at `0.31615335` and `0.39201885`.
- The retained `debugBodyYawOffset` audit reads `Animator.bodyRotation` only inside `OnAnimatorIK()`, avoiding the Unity 6 warning caused by reading it in `Update()`. It is intentionally test support, not gameplay permission logic.

## Latest Runtime Verification

The learner reported the final combined behavior as normal on 2026-08-28:

- Block Startup / Hold / Release and early Startup release complete correctly.
- Block cancels Basic Attack; cancelled stale attack Events do not restore or corrupt attack state.
- Same-frame Block / Attack / Jump results match the six accepted deterministic rules.
- Guard Start no longer visibly turns away and back after local Clip rotation correction.
- Guard End -> Idle/locked locomotion is visually acceptable at the `0.45s` normal exit blend.
- Movement and new Attack interrupt soft recovery; the earlier Guard-exit turn-before-attack symptom is gone.
- Single attacks, the full four-step combo, Attack4 ending, movement/restart, Block cancellation, locked/unlocked return, and Console checks passed.
- Attack4's earlier `FinishAttack(3)` creates a usable soft tail without observed damage or input regressions.

The current workspace source also previously compiled with zero errors and zero warnings after the soft-recovery lifecycle correction. No gameplay file changed during this documentation checkpoint.

## Deferred Guard Work

- Phase-aware Hold movement is not implemented. The intended later rule remains: unlocked Hold reuses camera-relative movement and movement-facing; locked Hold reuses lock-on directional movement and target-facing; Sprint stays disabled while Blocking.
- Startup facing assistance is not implemented. The approved later rule remains locked authoritative target or one temporary unlocked target within forward `120` degrees without mutating `PlayerTargeting.CurrentTarget`.
- Perfect Guard Window data, ordinary forward `180`-degree Guard coverage, attack-source direction, Damage / Defense Resolution, Block Hit, Guard Break, Parry, and Counter remain unimplemented.
- Directional `Walk_Block_*` presentation remains deferred until phase-aware Hold movement exists.
- Dodge remains after the next Guard gameplay concept. Do not add a numeric Priority system, general Request Queue, large Coordinator, Ability Framework, hierarchical FSM, or pre-emptive `PlayerMotor`.

## Exact Next Development Step

Add only phase-aware Guard movement permission: Startup and Release remain stationary; Hold allows ordinary movement but never Sprint. Preserve camera-relative movement and movement-facing while unlocked, and preserve directional movement plus target-facing while locked. Keep this permission change separate from directional Guard Clips, Startup facing assistance, Perfect Guard, Damage / Defense Resolution, Parry/Counter, Guard Break, and Dodge.

## Files to Read Next

- `AGENTS.md`
- `Docs/CURRENT_STATE.md`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerBlock.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerTargeting.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAnimator.cs`

Inspect the protected Prefab and Scene only when current local wiring or runtime values are necessary. Never overwrite them from a tracked baseline.

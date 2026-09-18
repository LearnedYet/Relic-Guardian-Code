# Relic Guardian Current Handoff

Updated: 2026-09-18. The grounded base Dodge is implemented and learner-runtime-tested. The current camera tuning is saved. A small Guard locked-movement/camera presentation twitch remains known and is deliberately deferred.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap. Actual code, saved Unity assets, current Editor state and Git status remain authoritative. Use `Docs/PLAYER_COMBAT_EXTENSION_DESIGN.md` for the approved future Perfect Dodge/Counter direction; use `Docs/CURRENT_STATE.md` for the sole active Exact Next Step.

The learner remains the author of key gameplay and presentation code unless they explicitly request takeover for a bounded scope. No commit or push accompanies this Handoff.

## Current Implemented Dodge Checkpoint

- `RelicGuardianPlayer.inputactions` contains the `<Mouse>/backButton` `Dodge` Button binding. `PlayerInputReader` records a one-use `dodgeRequested` edge through `OnDodge()` / `ConsumeDodge()`.
- `PlayerActionState.Dodging` exists. `PlayerActionController.ResolveActionRequests()` deterministically evaluates `Dodge -> Block -> Attack -> Jump` once per frame.
- Grounded Free may enter Dodge. An active ordinary Basic Attack may also cancel into Dodge through `PlayerCombat.TryCancelAttack()` and the shared attack cleanup boundary. Future Guard Counter/Dodge Counter actions do not exist yet and therefore have no cancellation implementation.
- `PlayerDodge` owns one execution's direction snapshot, scaled-time lifetime and progress-based code displacement. Saved Scene values are `dodgeDuration = 0.6`, `dodgeMovementDuration = 0.6`, and `dodgeDistance = 3`.
- `PlayerMovement` remains the only `CharacterController` displacement and actual-facing writer through `MoveDuringDodge()` and `SnapFacing()`; Apply Root Motion remains disabled.
- Unlocked input Dodge uses camera-relative direction and snaps actual facing before animation selection. Unlocked zero-input Dodge goes backward from current facing. Locked input Dodge uses target-relative directions; locked zero-input Dodge moves away from the target. Direction is captured once at entry.
- `PlayerAnimator.PlayDodge()` converts the world direction to local `DodgeX / DodgeZ`, selects `Dodge_Combat` for zero-input recovery or `Dodge_Combat_To_Run` when input is held, and starts the configured CrossFade. `Dodge_Combat` currently plays at Animator state Speed `1.25`.
- No-input Dodge finishes gameplay and leaves its remaining authored tail as interruptible soft recovery. Input Dodge uses the to-Run presentation and returns naturally to locomotion. The learner accepted the current transitions after matching gameplay duration to the animation timing.
- `CanFaceLockedTarget` permits target-facing during Free, Blocking and Dodging. Guard Facing Assist still has first priority in `PlayerMovement`.
- The learner runtime-tested the current basic Dodge directions, ordinary movement handoff and Attack-to-Dodge cancellation as working. The final code-order experiment that recalculated local movement after rotation produced no visible improvement and was fully reverted.

## Saved Camera Baseline

`Assets/Scenes/SampleScene.unity` was saved on 2026-09-18 with the learner-approved current Lock-On baseline:

- `enemyLookWeight = 0.35`;
- Cinemachine Rotation Composer `Aim Damping = (0.2, 0.2)`;
- `Screen Position = (0, 0)`;
- `Dead Zone = (0, 0)`;
- Orbital Follow remains `LockToTargetWithWorldUp`, recenters on `TrackingTarget`, uses horizontal range `-30..25`, and does not wrap.

Earlier experiments that changed the camera follow target or rotated a separate follow object were rejected because they caused side composition or visible jitter. The original Follow/LookAt structure remains: Follow `PlayerCameraRoot`, LookAt `LockOnCameraTarget`. No extra `LockOnCameraFollowTarget` remains and `PlayerCameraController.cs` has no experimental change from those attempts.

## Known Deferred Issue

When Block is held while moving around a locked target, the character/camera presentation can twitch briefly during direction or animation transitions. The learner chose to defer it because this movement case is infrequent.

Evidence and limits:

- Moving the local-direction conversion after facing did not improve the visible result; that experiment was reverted and must not be treated as a fix.
- The camera can still contribute because Guard Facing Assist temporarily recenters toward the enemy while the normal Lock-On composer has damping, then releases back to the delayed composition.
- `Guard_Locked_Locomotion` blends a `2.733s` Block loop with roughly `1.067s` movement clips. Directional movement clips use Root Transform Rotation `Bake Into Pose`, while `Block_Start`, `Block_Loop`, and `Block_End_NoRootTurn` were observed without the same setting. Cycle offsets are all `0`. These animation-phase/import differences are plausible contributors but were not changed or runtime-proven.
- Do not resume by stacking more simultaneous camera, code-order and animation-import changes. Pick one controlled comparison and record the result.

## Not Implemented

- Dodge has no I-Frame, Perfect Window, `DodgeResult`, incoming-hit avoidance, Slow Motion, afterimage/trail/distortion/SFX, Dodge Counter opportunity or `PlayerDodgePresentation` yet.
- Guard Counter, Dodge Counter, Sprint Attack, Player HitStun/HitReaction and Player Death remain later slices.
- Air Dodge and Fast Dodge remain deferred. Imported licensed animation files stay under ignored `Assets/LocalLicensed/`.

## Exact Next Step

Do not resume the deferred Guard twitch automatically. First choose one independent feature slice. The recommended Player progression is the smallest Dodge gameplay-result slice: define and implement only the I-Frame/Perfect-window timing and incoming-hit result boundary, then runtime-test ordinary Dodge avoidance before adding Perfect Dodge presentation, Slow Motion or Dodge Counter. If the learner instead chooses another feature, update `Docs/CURRENT_STATE.md` before changing behavior.

## Protected Local State

The working tree intentionally contains mixed learner work. Preserve every existing change and do not broadly replace or stage these paths:

- `Assets/RelicGuardian/Enemy/Data/Move/Goblin_Spacing.asset`
- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.inputactions`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionState.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAnimator.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerCombat.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerInputReader.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerDodge.cs` and `.meta`
- `Assets/Scenes/SampleScene.unity`
- current modified/untracked documentation shown by `git status`.

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` are ignored, local-only and must never be committed or uploaded. The prior Dodge-animation-import Handoff is archived at `Docs/Archive/HANDOFF_2026-09-17_PLAYER_DODGE_ANIMATION_IMPORT.md`.

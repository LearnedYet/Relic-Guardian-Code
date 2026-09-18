# Relic Guardian Current Handoff

Updated: 2026-09-17. The minimum Player combat-extension architecture and the concrete Basic/Perfect Dodge direction are approved. Thirty-two selected local Dodge AnimationClips are imported and validated. Dodge gameplay has not begun.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap. Actual code, Unity assets, Editor state and Git status remain authoritative. Read `Docs/PLAYER_COMBAT_EXTENSION_DESIGN.md` for the approved boundary and full Dodge contract; `Docs/CURRENT_STATE.md` owns the Exact Next Step.

The learner remains the author of key gameplay and presentation code. Do not interpret `继续`, `好` or completion of a prediction as authorization for Codex to implement the next behavior.

## Exact Next Step

Begin with input representation only:

1. In `Assets/RelicGuardian/Player/RelicGuardianPlayer.inputactions`, the learner adds a `Dodge` Button action with a temporary Left Ctrl binding.
2. In `PlayerInputReader`, the learner adds a one-use `dodgeRequested` request, `OnDodge()` callback and `ConsumeDodge()` method, following the existing one-use request pattern.
3. Inspect the saved files and compile. Stop before adding `Dodging`, `PlayerDodge`, movement or Animator behavior.

Before asking the learner to create these identifiers, explain their responsibility, need, kind/scope, lifetime, C# type and every English word in the name as required by `AGENTS.md`.

## Approved Dodge Gameplay Contract

- Entry is grounded `Free -> Dodging` only.
- Same-frame priority is `Dodge -> Block -> Attack -> Jump`.
- Priority does not grant cancellation. Do not add Attack-to-Dodge or Block-to-Dodge cancellation in v1.
- Without Lock-On, valid input uses the existing camera-relative Free Movement basis; no input moves opposite current actual facing.
- With Lock-On, valid input is target-relative: forward toward target, backward away, left/right around the target and diagonals as combinations; no input moves away from target.
- Capture direction once at Dodge start. Later input does not alter that Dodge trajectory.
- Dodge distance, duration, I-Frame, Perfect Window and Slow Motion values remain runtime-tunable; final numbers are undecided.
- Gameplay lifetime and displacement are code-owned and must not use AnimationClip length as the sole boundary.

## Approved Ownership

- `PlayerActionController` remains the coarse `PlayerActionState` and deterministic same-frame arbitration owner.
- A new `PlayerDodge` will own Dodge execution lifetime, direction snapshot, windows, completion, cleanup and the later Dodge Counter opportunity.
- `PlayerMovement` remains the sole `CharacterController` displacement and actual-facing writer; Apply Root Motion stays off.
- `PlayerAnimator` owns animation selection and transitions only.
- `PlayerHitReceiver` remains the incoming-hit entry and asks active Dodge for its gameplay result before forwarding an unhandled hit.
- `PlayerDodgePresentation` will contain replaceable Afterimage, Shallow Trail, Distortion and SFX hooks and may request Slow Motion through the sole time-control boundary.
- Presentation never decides admission, I-Frames, Perfect classification, damage avoidance, Counter opportunity, gameplay completion or state.
- Natural action completion and action-owned cleanup during replacement remain separate responsibilities. Do not create a general interruption framework.

## Approved Perfect Dodge Presentation

The fixed result is: an enemy attack about to hit passes through the Player's original position after a successful Perfect Dodge; one current-pose human afterimage remains there, a very shallow trail follows the movement, a subtle distortion confirms the success location, brief Slow Motion is requested, a dedicated success SFX plays, and a future Dodge Counter opportunity opens.

Avoid large explosions, shockwaves, strong magical effects, Perfect Guard-style hard collision and long heavy afterimage chains. Final VFX/SFX are not imported or implemented yet; only replaceable Presentation slots/hooks will be added when that implementation slice begins.

## Imported Local Animation Set

The isolated AssetLab source was:

`C:/Unity/Project/RelicGuardianAssetLab/Assets/SwordAnimationPack/Animation/Humanoid/06_Dodge`

Thirty-two `.anim` files plus their `.meta` files were copied into ignored local-only main-project folders below `Assets/LocalLicensed/SwordAnimationPack/Dodge`:

- `01_Dodge`: eight grounded free-direction Dodge clips;
- `02_Dodge_Combat`: eight grounded combat-direction Dodge clips;
- `05_Dodge_to_Run`: eight ordinary transition clips;
- `06_Dodge_Combat_to_Run`: eight combat transition clips.

Air, Fast, FBX duplicate and unrelated animation-pack assets were excluded. GUIDs were preserved and no main-project GUID collision was found. Unity recognized exactly 32 Humanoid clips. Dodge clips are 1.333 seconds at 60 FPS; to-Run clips are 0.667 seconds at 60 FPS. All imported copies were changed to non-looping; the AssetLab source was not changed. The Unity Console was zero errors/warnings after import.

Filename mapping is recorded in `PLAYER_COMBAT_EXTENSION_DESIGN.md`. `Dodge_Combat_R_L_45` is only a provisional front-right 45-degree candidate until learner visual confirmation. Dodge-to-Run clips are visual recovery only and never own gameplay state. Air Dodge remains deferred.

## Not Implemented or Verified

- No Dodge Input Action or `PlayerInputReader` Dodge request exists yet.
- No `Dodging` state, `PlayerDodge`, `DodgeResult` or `PlayerDodgePresentation` exists yet.
- No Dodge movement, I-Frame, Perfect Window, Slow Motion, Counter opportunity, Animator state or Player composition wiring exists yet.
- No Dodge Play Mode acceptance has occurred.
- No Player gameplay C#, Input Actions, Animator Controller, Prefab or Scene was changed during the animation-import task.

## Protected Local State

Current dirty files belong to the learner or contain mixed local work. Preserve them and do not broadly replace or stage them:

- `Assets/RelicGuardian/Enemy/Data/Move/Goblin_Spacing.asset`
- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/Scenes/SampleScene.unity`

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` are ignored, local-only and must never be committed or uploaded. The imported Dodge clips live inside that boundary. No commit or push accompanies this Handoff.

The preceding Player architecture Handoff is archived at `Docs/Archive/HANDOFF_2026-09-17_PLAYER_COMBAT_EXTENSION_ARCHITECTURE.md`. Its preceding Enemy spacing Handoff remains at `Docs/Archive/HANDOFF_2026-09-16_ENEMY_SPACING.md`.

# Relic Guardian Current Handoff

Updated: 2026-09-22. The learner is switching conversations to resume script development in the main Relic Guardian project. The isolated Shallow Trail experiment is paused.

This is the 2026-09-22 Handoff snapshot, not the current configuration record. `CURRENT_STATE.md` and `COMBAT_SFX_RESOURCE_TRACKING.md` supersede its Dodge timing/audio/Slow-Motion values and next step after the learner's 2026-09-23 runtime acceptance.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap. Actual code, saved Unity assets, current Editor state and Git status remain authoritative. Use `Docs/CURRENT_STATE.md` for the sole active Exact Next Step, `Docs/PLAYER_COMBAT_EXTENSION_DESIGN.md` for the approved Perfect Dodge stack, and inspect `PlayerDodgePresentation.cs` before teaching its next change.

The learner remains the author of key gameplay and presentation code. Codex took over only the bounded multi-Renderer afterimage correction after the learner explicitly requested it. Teaching has returned to learner authorship, with unfamiliar Unity APIs and methods explained before use. No commit or push accompanies this Handoff.

The learner explicitly asked to resume the earlier script work in a new conversation. Do not resume the Shallow Trail tuning automatically.

## Current Perfect Dodge Checkpoint

- `PlayerDodge.ResolveDodgeHit()` classifies `Unhandled / Ordinary / Perfect` through the saved `0.1..0.45s` I-Frame and nested `0.1..0.2s` Perfect Window.
- `PlayerHitReceiver` routes only `Perfect` to `PlayerDodgePresentation`; both handled Dodge results avoid damage and Unhandled continues to Guard/Health.
- `PlayerDodge` snapshots Dodge start position and rotation. `PlayerDodgePresentation` leaves one static current-pose silhouette at that origin.
- Active modular `SkinnedMeshRenderer` parts are baked separately. Active rigid `MeshRenderer + MeshFilter` equipment is copied separately; LOD-controlled rigid meshes use only LOD0.
- Each generated part receives an independent runtime clone of `PerfectDodgeAfterimage.mat`, disables shadows, adds `AfterimageFade` and owns timed cleanup of the generated GameObject, Mesh and Material.
- The learner runtime-verified the complete player-and-weapon silhouette, continuous synchronized Alpha fade and cleanup with a temporary `1.5s` lifetime, then confirmed the saved `0.35s` value returned and the Unity Console remained clean. Independent C# build passed with zero errors; the pre-existing unused `PlayerAnimator.dodgeExitCrossFadeDuration` warning remains.

## Isolated AssetLab Experiment

The separate project `C:\Unity\Project\RelicGuardianAssetLab` contains project-owned V2, V3 and Final Shallow Trail candidates and a shared comparison scene under `Assets/RelicGuardianAssetLab/PerfectDodge/ShallowTrail/Candidates/`. They are visual experiments only; no candidate was accepted or integrated into the main project. The latest capture still showed a jagged edge. An experimental straight `Width Curve` was applied to the selected V2 waist scene instance, but its visual effect was not validated; do not describe it as a fix. The learner stopped tuning and may buy a more suitable resource pack. The Asset Store originals were not intentionally edited.

## Diagnosed False Leads

- The initial cyan fragment was not proof of a broken skeleton or bad material. Runtime logging showed the singular lookup had selected only `Female_Face_01`.
- `GetComponentInChildren<SkinnedMeshRenderer>()` returned one modular part; plural `GetComponentsInChildren<SkinnedMeshRenderer>()` was required.
- The rigid Katana uses ordinary MeshRenderer/MeshFilter components and was therefore absent until that path was added. Its Prefab contains LOD0/1/2, so blindly copying every MeshRenderer would overlap multiple LOD meshes.

## Not Implemented

- The placeholder has no HDR emission or Bloom.
- The selected Dodge Start and Perfect confirmation SFX resources are copied, tracked and connected through two independent Scene-local players. Saved volumes `0.8 / 0.52 / 0.63` are learner-chosen; actual Ordinary/Perfect playback and final listening balance are pending the Slow Motion pass. Shallow movement Trail, subtle Distortion, centrally-owned Slow Motion and Dodge Counter opportunity remain unimplemented.
- Guard Counter, Sprint Attack, Player HitStun/HitReaction and Player Death remain later slices.
- The locked Guard movement/camera twitch remains known and deliberately deferred.

## Exact Next Step

Resume learner-authored Perfect Dodge Slow Motion. First inspect `HitstopController`, the sole current `Time.timeScale` writer, and teach a small overlap-safe extension with unscaled deadlines and exact original-scale recovery; then connect a Perfect-only request through `PlayerDodgePresentation`. Do not change Dodge classification or let presentation directly set global time. Explain every new identifier and unfamiliar API before asking the learner to edit. Runtime-test Ordinary exclusion, Perfect activation/recovery, Hitstop overlap and a clean Console; in the same Play Mode pass, listen to Start plus the bound Perfect pair using the saved values and `0 / 0.01 / 0.02s` delays from `COMBAT_SFX_RESOURCE_TRACKING.md`. Keep Shallow Trail resource selection paused unless the learner explicitly returns to it.

## Protected Local State

The working tree intentionally contains mixed learner work. Preserve every current change shown by `git status`, especially:

- `.agents/skills/relic-guardian-github-mirror/scripts/inspect-mirror-stage.ps1`;
- `Assets/RelicGuardian/Enemy/Data/Move/Goblin_Spacing.asset`;
- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`;
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`;
- `Assets/RelicGuardian/Player/Scripts/HitResult.cs`, `PlayerDodge.cs`, `PlayerHealth.cs`, and `PlayerHitReceiver.cs`;
- new `DodgeResult.cs`, `PlayerDodgePresentation.cs`, their metadata, and `Assets/RelicGuardian/Player/Materials/`;
- `Assets/Scenes/SampleScene.unity`;
- all currently modified maintained documents.

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` remain ignored local-only content and must never be committed or uploaded. The preceding Handoff is archived at `Docs/Archive/HANDOFF_2026-09-21_PERFECT_DODGE_AFTERIMAGE.md`.

# Relic Guardian Current Handoff

Updated: 2026-09-21. Grounded Dodge, its incoming-hit result boundary and the first Perfect-only current-pose afterimage layer are implemented and learner-runtime-verified. The next activity is resource screening, not another gameplay-behavior expansion.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap. Actual code, saved Unity assets, current Editor state and Git status remain authoritative. Use `Docs/CURRENT_STATE.md` for the sole active Exact Next Step, `Docs/PLAYER_COMBAT_EXTENSION_DESIGN.md` for the approved Perfect Dodge stack, and `Docs/COMBAT_VFX_RESOURCE_TRACKING.md` for selected/local-only presentation resources.

The learner remains the author of key gameplay and presentation code. Codex took over only the bounded multi-Renderer afterimage correction after the learner explicitly requested it. Teaching has returned to learner authorship, with unfamiliar Unity APIs and methods explained before use. No commit or push accompanies this Handoff.

## Current Perfect Dodge Checkpoint

- `PlayerDodge.ResolveDodgeHit()` classifies `Unhandled / Ordinary / Perfect` through the saved `0.1..0.45s` I-Frame and nested `0.1..0.2s` Perfect Window.
- `PlayerHitReceiver` routes only `Perfect` to `PlayerDodgePresentation`; both handled Dodge results avoid damage and Unhandled continues to Guard/Health.
- `PlayerDodge` snapshots Dodge start position and rotation. `PlayerDodgePresentation` leaves one static current-pose silhouette at that origin.
- Active modular `SkinnedMeshRenderer` parts are baked separately. Active rigid `MeshRenderer + MeshFilter` equipment is copied separately; LOD-controlled rigid meshes use only LOD0.
- Each generated part receives an independent runtime clone of `PerfectDodgeAfterimage.mat`, disables shadows and owns timed cleanup of the generated GameObject, Mesh and Material.
- The learner runtime-verified the complete player-and-weapon silhouette and reported no new Unity Console error or warning. Independent C# build passed with zero errors; the pre-existing unused `PlayerAnimator.dodgeExitCrossFadeDuration` warning remains.

## Diagnosed False Leads

- The initial cyan fragment was not proof of a broken skeleton or bad material. Runtime logging showed the singular lookup had selected only `Female_Face_01`.
- `GetComponentInChildren<SkinnedMeshRenderer>()` returned one modular part; plural `GetComponentsInChildren<SkinnedMeshRenderer>()` was required.
- The rigid Katana uses ordinary MeshRenderer/MeshFilter components and was therefore absent until that path was added. Its Prefab contains LOD0/1/2, so blindly copying every MeshRenderer would overlap multiple LOD meshes.

## Not Implemented

- Gradual Alpha fade is not implemented. `AfterimageFade.cs` does not exist; its fields/APIs were explained only.
- The placeholder has no HDR emission or Bloom.
- Shallow movement Trail, subtle Distortion, dedicated Perfect Dodge SFX, centrally-owned Slow Motion and Dodge Counter opportunity remain unimplemented.
- Guard Counter, Sprint Attack, Player HitStun/HitReaction and Player Death remain later slices.
- The locked Guard movement/camera twitch remains known and deliberately deferred.

## Exact Next Step

Prepare one isolated resource-screening pass for a restrained shallow movement Trail along the Dodge direction. Define its visual role separately from the working origin afterimage, then inspect candidates for license/source, dependency closure, render pipeline compatibility, scale, color, duration, loop/cleanup behavior and whether the effect stays subordinate to the character silhouette. Reject large crescents, explosions, teleport rings, long afterimage chains and Guard-impact-style collisions.

Do not import or connect a candidate before validation. Keep Distortion and SFX as later independent screening passes; Slow Motion is a time-control/code task rather than a VFX-resource task.

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

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` remain ignored local-only content and must never be committed or uploaded. The preceding Handoff is archived at `Docs/Archive/HANDOFF_2026-09-18_GROUNDED_DODGE.md`.

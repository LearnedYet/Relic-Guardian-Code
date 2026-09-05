# Relic Guardian Current Handoff

Last updated: 2026-08-31.

This file contains only the latest cross-conversation Handoff. The prior 2026-08-30 Handoff is preserved at `Docs/Archive/HANDOFF_2026-08-30_PREHIT_FACING_ASSIST.md`.

## Context Entry Points

For a new task or post-compaction recovery, read `AGENTS.md`, `Docs/CURRENT_STATE.md`, `Docs/ARCHITECTURE.md`, this file, then use `Docs/CONTEXT_INDEX.md` to select one bounded route. Run `git status --short --branch` before relying on documented state.

## Actual Implementation State

- `PlayerActionController` remains the sole coarse-state owner for `Free`, `Attacking`, and `Blocking`.
- `PlayerBlock` owns Startup/Hold/Release, Hold movement permission, Guard Coverage, pre-hit Facing Assist, and the minimal ordinary-versus-perfect Guard classification.
- `BeginBlock()` opens the Perfect Guard Window during Startup. The local authored `Block_Start.anim` Event closes it at `0.16666667s`; Hold and Release entry also close it.
- After coverage succeeds, `TryHandleHit()` logs `Perfect Guard` while the window is open or `Ordinary Guard` otherwise, then returns handled before health damage. No presentation consequence or reusable result contract exists yet.
- Pre-hit Facing Assist still stores one fixed direction and saved pre-assist facing; `PlayerMovement` remains the sole player Transform-facing owner.
- The current enemy attack remains a scheduled hit attempt without Hit Window-time physical confirmation.

## Runtime and Asset Verification

- The learner reported both ordinary Guard and Perfect Guard classification working in Play Mode, with handled hits preventing damage and a clean Console.
- `Block_Start.anim` currently persists `ClosePerfectGuardWindow` at `0.16666667s` and `StartupDecisionPoint` at `0.4s`.
- Four Combat VFX packages were visually validated in `RelicGuardianAssetLab`. Only the selected assets, their dependency closure, required complete support-script/importer groups, and the final Guard scene were copied into the main project's ignored `Assets/LocalLicensed/CombatVFX/` boundary with GUIDs preserved.
- The pruned copy is based on `79` Unity assets and contains `199` files including generated metadata, using approximately `62.90 MB`. The main project imported it with zero Console errors and zero warnings; selected Prefabs/HDR materials resolve, and the Guard validation scene has no missing scripts or broken Prefabs. No main-project combat VFX hookup or final in-camera tuning has been runtime verified.
- Exact package names, versions, selected candidates, paths, and restoration steps are in `Docs/COMBAT_VFX_RESOURCE_TRACKING.md`.

## Exact Next Concept

Define the smallest explicit boundary that carries the internal Ordinary/Perfect classification into presentation, then connect one feedback layer at a time. Before any key gameplay or presentation code is edited, explicitly establish whether the learner or Codex will type it. Keep enemy reaction, Parry/Counter, Guard Break, Dodge, and a general result hierarchy separate.

## Protected Git State

- Existing changes to `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`, `Assets/Scenes/SampleScene.unity`, the Animator Controller, Guard/Enemy scripts, package manifests, and maintained documents are protected and must not be overwritten or broadly staged.
- `Assets/LocalLicensed/CombatVFX/` and all other `Assets/LocalLicensed/` content are ignored local licensed assets and must never be committed or uploaded.
- No staging, commit, push, history rewrite, or remote mutation is authorized by this Handoff.

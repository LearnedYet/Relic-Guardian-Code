# Relic Guardian Context Index

This file is a routing index. It tells Codex which small set of files to read for a task; it is not an implementation or design source of truth.

## Core Context Contract

At a new task, after context compaction, or when resuming from a Handoff:

1. Read `AGENTS.md` completely.
2. Read `Docs/CURRENT_STATE.md` completely.
3. Read `Docs/ARCHITECTURE.md` completely.
4. Read the current `Docs/HANDOFF.md` completely.
5. Run `git status --short --branch`.
6. Select exactly the relevant route below and inspect its actual files.

Actual code, Unity assets, current Editor state, and Git status remain authoritative. Use `rg` to locate historical evidence before reading narrow excerpts. Never read the whole `Docs/Archive/` or `Docs/DEV_LOG.md` by default.

## Current Exact Next Route: Ordinary Guard Reaction

Read:

- `Docs/COMBAT_PRESENTATION_FEEDBACK_DESIGN.md`
- `Docs/GUARD_REACTION_DESIGN.md`
- `Docs/GUARD_HIT_RESOLUTION_DESIGN.md`
- `Assets/RelicGuardian/Player/Scripts/GuardResult.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHitReceiver.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerBlock.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerGuardPresentation.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAnimator.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`

Scope boundary: the hit-data seam, Guard classification, distinct Ordinary/Perfect Guard VFX and layered SFX, and Perfect-only `0.07s` Hitstop are implemented and learner-reported runtime verified for the current enemy. `Block_Hit.anim` is imported under the ignored licensed boundary with Loop Time disabled but is not connected. Next implement and verify Ordinary Guard Movement Lock as a gameplay deadline owned by `PlayerBlock`, then separately add an independent Animator reaction layer owned by `PlayerAnimator`. Presentation must not control movement permission. Keep Perfect player reaction, Camera feedback, Attack feedback, enemy reaction, Counter, Guard Break, and general frameworks outside this slice.

## Combat VFX Resource Selection or Local Restoration

Read:

- `Docs/COMBAT_VFX_RESOURCE_TRACKING.md`;
- the Git/licensed-asset sections of `AGENTS.md` and `Docs/DEVELOPMENT_RULES.md`;
- the Git Boundary section of `Docs/CURRENT_STATE.md`;
- actual assets only under the ignored `Assets/LocalLicensed/CombatVFX/` boundary.

Do not load or modify protected gameplay Scenes/Prefabs merely to inspect a resource. Use the local validation scenes first. Never stage, commit, upload, or mirror the licensed assets.

## Player Action, Attack, or Cancellation

Read:

- `Docs/COMBO_ATTACK_ARCHITECTURE.md`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionState.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerInputReader.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerCombat.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAttackData.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAnimator.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`

Inspect animation assets or Animation Events only when the concrete question requires Editor state.

## Guard Lifecycle or Guard Presentation

Read:

- `Docs/GUARD_HIT_RESOLUTION_DESIGN.md` only when the task concerns incoming Guard resolution.
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerBlock.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAnimator.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerTargeting.cs`

For presentation values, inspect the actual Animator/Clip import state without modifying ignored licensed source assets.

## Movement, Sprint, Lock-On, or Camera

Read:

- `Assets/RelicGuardian/Player/Scripts/PlayerInputReader.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerTargeting.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerCameraController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAnimator.cs` when presentation is involved.

## Enemy AI, Movement, Attack, or Health

Read only the involved files from:

- `Assets/RelicGuardian/Enemy/Scripts/EnemyAI.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyMovement.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAttack.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAttackPhase.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAnimator.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyHealth.cs`

Include the receiving player scripts only when the enemy task crosses that boundary.

## Git, GitHub Mirror, or Licensed-Asset Boundary

Read:

- the Git and licensed-asset sections of `AGENTS.md`;
- `Docs/DEVELOPMENT_RULES.md`, using headings/search to select the relevant workflow;
- the Git Boundary section of `Docs/CURRENT_STATE.md`;
- current `git status`, configured remotes, and connectivity/proxy evidence required by the operation.

Read matching excerpts of `Docs/DEV_LOG.md` only when diagnosing a previous failure. Never infer permission to stage, commit, push, or alter remote state from a context-sync request.

## Project Planning or Learning Progress

Project feature planning:

- `Docs/PROJECT_PLAN.md`
- `Docs/ROADMAP.md`

Learning state and internship preparation:

- `Docs/LEARNING_PROGRESS.md`
- `Docs/LEARNING_TRACKER.md`
- `Docs/GAME_CLIENT_LEARNING_PLAN.md`

Read only the track or feature currently being discussed.

## Historical Investigation

Historical sources are not startup context:

- `Docs/Archive/HANDOFF_2026-08-31_GUARD_VFX_RESOURCES.md`
- `Docs/Archive/HANDOFF_HISTORY_THROUGH_2026-08-29.md`
- `Docs/DEV_LOG.md`
- older checkpoint sections in planning/learning documents.

First search for a date, class, method, feature, error text, or decision name with `rg`. Read only the matching section and enough surrounding lines to interpret it. Historical text never overrides actual files or the current entry documents.

## Protected Unity Files

The following mixed local assets require a separate explicit review before staging or modification:

- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/Scenes/SampleScene.unity`

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` remain ignored and must never be committed or uploaded.

# Relic Guardian Context Index

This file maps task types to bounded reading sets. It contains no current-task or next-step record. CURRENT_STATE.md alone maintains the active Exact Next Step; choose a route here from that step or the user's task.

## Core Context Contract

At a new task, after context compaction, or when resuming from a Handoff:

1. Read `AGENTS.md` completely.
2. Read `Docs/CURRENT_STATE.md` completely.
3. Read `Docs/ARCHITECTURE.md` completely.
4. Read the current `Docs/HANDOFF.md` completely.
5. Run `git status --short --branch`.
6. Select exactly the relevant route below and inspect its actual files.

Actual code, Unity assets, current Editor state, and Git status remain authoritative. Use `rg` to locate historical evidence before reading narrow excerpts. Never read the whole `Docs/Archive/` or `Docs/DEV_LOG.md` by default.

## Enemy Receiving and Confirmed Player Attack Hit Feedback

Read:

- `Docs/COMBAT_PRESENTATION_FEEDBACK_DESIGN.md`
- `Docs/ENEMY_COMBAT_AGENT_DESIGN.md` (Authority/Immediate Scope, Receiving and Confirmed Feedback, and the relevant acceptance gate)
- `Docs/COMBAT_VFX_RESOURCE_TRACKING.md` / `Docs/COMBAT_SFX_RESOURCE_TRACKING.md` (selected confirmed-hit resources)
- `Assets/RelicGuardian/Player/Scripts/PlayerCombat.cs`
- `Assets/RelicGuardian/Player/Scripts/HitContext.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyHealth.cs`
- `Assets/RelicGuardian/Player/Scripts/CombatAudioPlayer.cs`
- `Assets/RelicGuardian/Player/Scripts/CombatAudioData.cs`
- `Assets/RelicGuardian/Player/Scripts/CombatAudioLayer.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyHitReceiver.cs` and `EnemyHitPresentation.cs` once created; these are planned files, not current implementation evidence.

Use this route for enemy receiving, confirmed-hit presentation and lethal-feedback lifetime questions. Consult the dedicated feature design for scope and acceptance requirements.

## Combat VFX Resource Selection or Local Restoration

Read:

- `Docs/COMBAT_VFX_RESOURCE_TRACKING.md`;
- the Git/licensed-asset sections of `AGENTS.md` and `Docs/DEVELOPMENT_RULES.md`;
- the Git and Protected Local State section of `Docs/CURRENT_STATE.md`;
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
- `Docs/GUARD_REACTION_DESIGN.md` for reaction, movement-lock and animation timing.
- `Docs/COMBAT_VFX_RESOURCE_TRACKING.md` / `Docs/COMBAT_SFX_RESOURCE_TRACKING.md` for exact presentation configuration.
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

- `Docs/ENEMY_COMBAT_AGENT_DESIGN.md` (approved future direction; select sections for the current stage)
- `Docs/ENEMY_COMBAT_RESOURCE_TRACKING.md` when the task selects, imports, configures or verifies SwordShield Goblin animation/presentation resources.
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
- the Git and Protected Local State section of `Docs/CURRENT_STATE.md`;
- current `git status`, configured remotes, and connectivity/proxy evidence required by the operation.

Read matching excerpts of `Docs/DEV_LOG.md` only when diagnosing a previous failure. Never infer permission to stage, commit, push, or alter remote state from a context-sync request.

## Project Planning or Learning Progress

Project feature planning:

- `Docs/PROJECT_PLAN.md`
- `Docs/ROADMAP.md`
- `Docs/ENEMY_COMBAT_AGENT_DESIGN.md` for the SwordShield Goblin receiving/state/reaction/Strong Combo/spacing roadmap.
- `Docs/ENEMY_COMBAT_RESOURCE_TRACKING.md` for the selected Goblin animation set, source GUIDs and main-project import status.

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

Use AGENTS.md for the durable modification/staging and licensed-asset rules, and current git status plus CURRENT_STATE.md for the protected working-tree snapshot. This route does not grant permission to mutate Unity assets.

# Relic Guardian Current Handoff

Updated: 2026-09-16. The first enemy combat-spacing, directional-locomotion and shared-data slice is runtime-verified and ready for a focused checkpoint.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap; `CURRENT_STATE.md` owns the Exact Next Step. The learner remains the author of key gameplay and presentation code unless they explicitly request takeover for a bounded scope. Unity MCP is connected to `My project@f22d513a32eb5447`, but the instance hash may change after restart.

Do not extend this checkpoint from a generic `继续`. First choose one independent next slice: detection/target-loss and obstacle expectations, a concrete locomotion issue observed in the real camera, or the approved Strong Attack prerequisites beginning with Player HitStun, functional Dodge and death-safe recovery. Do not bundle these paths.

## Current implementation

- `EnemyAI` keeps coarse-state checks and internal `Run / Approach / Retreat / Strafe / Wait` decisions. Inside attack range it checks facing and requests a legal attack before fallback spacing, so Wait/Retreat timing never becomes another attack gate.
- `EnemyMovement.Move(moveDirection, facingDirection, speedMultiplier)` owns normal facing/displacement. Retreat moves away while facing the player; Strafe moves tangentially while facing the player. Apply Root Motion remains disabled.
- `EnemyMovement.CurrentLocalHorizontalVelocity` supplies real movement evidence. `EnemyAnimator` damps `Speed / MoveX / MoveZ` with `locomotionDampTime = 0.12s`; the local licensed Goblin controller uses a directional Blend Tree.
- `EnemySpacingData` owns categorized shared Attack Admission, Distance Bands, Behavior Timing and Speed Multipliers. `Goblin_Spacing.asset` saves range/angle `2.2m / 15°`, Retreat `1.0m / 1.6m`, fixed Strafe `1.75s`, Wait `1.5..2.0s`, speed multipliers `1 / 0.5 / 0.25 / 0.35`, and Approach/Run thresholds `1.5m / 5m`.
- Per-enemy mutable state remains on `EnemyAI`: current behavior, behavior-end deadline and strafe side. The asset stores no target, phase, timer or execution state.

## Runtime evidence and limits

- Learner checks passed Run/Approach, Retreat, left/right Strafe, Wait pacing, directional presentation, attack-first interruption and the final ScriptableObject migration regression.
- Independent solution build and final Unity Console checks ended with zero errors and zero warnings.
- Detection/target loss, patrol, obstacle-aware navigation and integrated Strong Combo remain unimplemented. The accepted test covers the current always-engaged target in the existing Scene, not a complete navigation/agent acceptance suite.

## Protected local state

Keep these files unstaged unless a future task explicitly scopes them:

- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/Scenes/SampleScene.unity`

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` remain local-only and must never be committed or uploaded. The Scene contains the local `Goblin_Spacing.asset` assignment plus older mixed licensed wiring, so it remains outside focused commits. The earlier unrelated `Assets/RelicGuardian/Enemy/Data/Move/NewMonoBehaviourScript.cs` was never committed and is absent from the final working tree.

## Git synchronization

- Previous local weighted-selection checkpoint: `5e2e26d Complete weighted enemy attack selection`.
- Local spacing feature checkpoint: `44948c3 Add enemy combat spacing and directional movement`.
- Flattened GitHub spacing feature checkpoint: `e03ac4e Sync enemy combat spacing and movement`; its normal push was verified at `e03ac4e5aba6a4de2a1b4e40aa2c19e5926a8c27` before the record-only documentation follow-up.
- Both checkpoints used explicit allowlists. The Unity Scene, local licensed Animator/resources, protected Player files and Unity `.meta`/data assets in the mirror remained excluded according to their respective repository boundaries; the earlier unrelated template script was never included.
- Local full-project history and flattened GitHub mirror history remain separate. Never pull or merge the mirror `main` directly into this full Unity workspace.

The preceding Handoff is archived at `Docs/Archive/HANDOFF_2026-09-14_WEIGHTED_ATTACK_SELECTION.md`.

# Relic Guardian Current State

Last reviewed against the local workspace: 2026-08-27

This is the short current-state entry point for a new Codex task. It intentionally excludes most historical detail. When this file conflicts with actual code, Unity, or Git state, inspect those sources and update this file.

## Project Environment

- Local project: `C:\Unity\Project\My project`
- Unity: `6000.3.19f1`
- Render pipeline: URP `17.3.0`
- Input System: `1.19.0`
- Cinemachine: `3.1.7`
- Movement: `CharacterController`
- Apply Root Motion: off
- Target platform: Windows

The project root currently has no `README.md`. Do not require it as a startup file unless one is later created.

## Current Git Boundary

- Local full-project branch: `main`
- Local full-project HEAD: `4719b71 Record Guard design and attacking migration`
- Last confirmed code-and-document GitHub synchronization: `23a854d Sync locked locomotion and free sprint code`
- The GitHub repository is code/document focused and has a different history shape from this full Unity workspace. Do not directly pull or merge remote `main` into this workspace.

The following protected mixed Unity assets are intentionally modified locally and must not be reset, restored, overwritten, or staged without a separate explicit review:

- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/Scenes/SampleScene.unity`

The Prefab and Scene contain mixed local licensed presentation, camera, player, enemy, and runtime wiring. The project-owned Animator Controller, including the shared `Locked Locomotion -> BasicAttack` entrance and graph-layout edits, is committed locally at `4719b71` and is no longer a dirty working-copy file.

Current focused in-progress changes also include the first two Block input fields in `PlayerInputReader.cs` plus the Guard planning and learning documents. They remain uncommitted and are not runtime-verified Block gameplay.

`Assets/LocalLicensed/` remains ignored and must never be committed or uploaded. It contains the local P09 presentation, Katana animation assets, Animator Override mappings, Goblin presentation assets, and other licensed dependencies.

## Implemented Current Gameplay

### Player Foundation

- Camera-relative free movement, smooth facing, gravity, grounded reset, Jump, and airborne animation flow are implemented through `CharacterController`.
- The first locked-combat rule rejects Jump while `PlayerTargeting.IsLockedOn` is true.
- Normal free travel uses speed `3`; held Sprint uses speed `6`.
- Sprint is bound to Left Shift as a `Pass Through` Input Action so both press and release update `PlayerInputReader.IsSprintHeld`.

### Lock-On and Locomotion

- `V` toggles the nearest valid lock-on target.
- `PlayerTargeting` owns the authoritative target, clears inactive targets, and breaks lock beyond the configured range.
- Locked movement keeps the player facing the target while displacement remains camera-relative.
- Locked locomotion uses project-owned `MoveX` and `MoveZ` parameters plus a 2D Simple Directional Blend Tree.
- `PlayerMovement` converts world movement to player-local direction with `transform.InverseTransformDirection()`.
- The tracked Controller uses eight placeholder Clips; ignored local overrides map them to non-Root Katana `Jogging_8Way_verB` cardinal and diagonal animations.
- Shift plus nonzero movement while locked cancels lock and enters free Sprint. Shift alone preserves lock. Releasing Shift does not restore the previous lock.

### Camera

- Separate free and lock Cinemachine cameras are selected by priority.
- Only the input-axis controller belonging to the current targeting mode is enabled.
- FreeLook uses incoming-position inheritance; the lock camera freezes when blending out.
- The learner accepted the current free-camera sensitivity and unlock blend response.
- Saved tuning documented at the latest checkpoint includes Brain blend time `1s`, free gains `1.8/-1`, and lock gains `0.3/-0.1`.
- Multi-target switching, lock UI, camera occlusion, extreme framing, and production camera polish are deferred.

### Player Action and Combat Architecture

- `PlayerActionController` is the authoritative coarse action-state owner.
- Current coarse states are `Free` and `Attacking`. The earlier `BasicAttack` enum member and controller method names were migrated to coarse attack terminology without changing the four-step attack flow.
- `PlayerCombat` implements one reusable four-step indexed light-attack flow using `PlayerAttackData[]`.
- Attack steps share initialization, target selection, facing, bounded code-driven lunge, Hit Windows, Combo Windows, transition points, Restart Windows, step-identity guards, damage requests, and centralized finish cleanup.
- Locked and free locomotion enter the same four-step attack chain. Do not create `LockedBasicAttack` or duplicate locked combat logic.
- Locked combat gives the locked target priority. An out-of-attack-range locked target produces no attack target and does not fall back to another nearby soft target.
- General recovery cancellation remains deferred until a concrete Block or Dodge interaction requires it.

### Enemy Prototype

- The ordinary enemy prototype can chase, face, stop, enter a timed attack, display a startup telegraph, damage `PlayerHealth`, and return through recovery.
- `EnemyAttackPhase` contains `Ready`, `Startup`, `HitWindow`, and `Recovery`.
- `EnemyHealth` subtracts damage and currently uses `gameObject.SetActive(false)` as the minimum zero-health consequence.
- Player health clamping, production player death, enemy death animation, hit reaction, effects, and a generalized enemy action-state system are not implemented.

## Current Ownership Map

- `PlayerInputReader`: input values, one-use requests, and held Sprint state.
- `PlayerActionController`: coarse player-action permission and lifecycle.
- `PlayerMovement`: player `CharacterController` displacement, gravity, Jump acceptance, movement-facing application, and reported movement state.
- `PlayerTargeting`: authoritative lock-on target and lock lifecycle.
- `PlayerCameraController`: free/lock camera selection, target composition, and camera-input ownership.
- `PlayerAnimator`: locomotion, airborne, lock-mode parameters, and attack presentation triggers.
- `PlayerCombat`: attack sequence coordination, attack targeting policy, windows, lunge requests, damage requests, and cleanup.
- `PlayerAttackData`: per-step attack configuration only, never execution state.
- `EnemyAI`: ordinary enemy high-level chase-versus-attack request.
- `EnemyMovement`: enemy displacement, facing, stopping, and actual horizontal-speed reporting.
- `EnemyAttack`: enemy timed attack phase and player-damage request.
- `PlayerHealth` / `EnemyHealth`: health mutation and their own health consequences.

## Latest Verification

The learner completed the deferred opposite regressions on 2026-08-27:

1. Locked Space produced no Jump.
2. Unlocked grounded Space still completed the Jump and landing flow.
3. Locked Attack entered the shared four-step combo and returned correctly to locked locomotion.

After `BasicAttack -> Attacking`, `TryStartBasicAttack() -> TryStartAttack()`, and `FinishBasicAttack() -> FinishAttack()` were migrated, the learner re-verified the unlocked four-step combo, movement/Jump recovery, the late Restart Window, and the locked four-step return. Standard validation reported zero diagnostics for the three changed scripts, and the final Console query contained zero errors and zero warnings.

## Approved First Guard / Block Direction

This is approved design, not implemented or runtime-verified gameplay. Actual coarse action code still contains only `Free` and `Attacking`; there is no Block Input Action, `Blocking` enum member, or `PlayerBlock` component yet. `PlayerInputReader` now contains only the private `blockRequested` and `isBlockHeld` fields; no public property, callback, consume method, or Input Action has been connected.

- `PlayerActionController` remains the sole owner of the coarse Gameplay FSM. The intended near-term states are `Free`, `Attacking`, and `Blocking`; later candidates are `Dodging`, `Staggered`, and `Dead`.
- `PlayerBlock` will own the internal `Guard Startup -> Guard Hold -> Guard Release` lifecycle. These phases do not become global `PlayerActionState` members, and Animator remains presentation-only.
- Guard is held input. It may enter from grounded `Free`, and the current four-step Basic Attack may be interrupted into Guard throughout Startup, Hit Window, and Recovery. A rejected Block press is not buffered.
- At the gameplay-design level, Block has higher action priority than Basic Attack. Block wins an otherwise simultaneous Free-state Block/Attack request and may pre-empt an active Basic Attack. Future skills may explicitly deny Block interruption; priority chooses among legal transitions and does not override a skill's authored restriction.
- Cancelling before `OpenHitWindow()` prevents that attack step's damage. Cancelling after damage has already been applied never rolls it back. `PlayerCombat` must close all windows and clear queue, target, facing, lunge, distance, and attack-index state before `PlayerActionController` completes `Attacking -> Blocking`.
- Startup is short, blocks ordinary translation, allows only required target-facing correction, and contains an authored Perfect Guard Window. Releasing during Startup waits for an appropriate decision or exit point before Release.
- Hold allows movement but not Sprint. Unlocked Hold retains camera-relative free movement and movement-facing; locked Hold retains lock-on directional movement and target-facing. Guard never changes camera mode.
- Release is a short non-moving recovery and then returns to `Free`.
- Unlocked Startup may choose one temporary facing-assist target within the player's forward `120` degrees (`+/-60` degrees). It must never write that target into `PlayerTargeting.CurrentTarget`. Locked Startup uses the existing authoritative target directly.
- Future ordinary Guard resolves only attacks arriving within the player's forward `180` degrees (`+/-90` degrees). Attack-source data and Damage / Defense Resolution remain deferred until that behavior is implemented.
- New discrete actions should use code-driven `Animator.CrossFadeInFixedTime()` presentation. The existing four-step combo is not migrated as part of Block.

The isolated `SwordAnimationPack` preview was accepted as visually compatible with the current Katana set. The approved first-version lifecycle Clips are `Block_Start` for Startup, `Block_Loop` for stationary Hold, and `Block_End` for Release. The approved locked Hold locomotion candidates are `Walk_Block_Loop_F_0_RM`, `Walk_Block_Loop_F_L_45_RM`, `Walk_Block_Loop_F_R_45_RM`, `Walk_Block_Loop_F_L_90_RM`, `Walk_Block_Loop_F_R_90_RM`, `Walk_Block_Loop_B_180_RM`, `Walk_Block_Loop_B_L_45_RM`, and `Walk_Block_Loop_B_R_45_RM`.

`Block_Hit`, `Block_Hit_Break`, `Turn_Block_*`, directional `Walk_Block_Start_*` / `Walk_Block_Stop_*`, and the extra `Walk_Block_B_L_90_RM` / `Walk_Block_B_R_90_RM` Clips remain optional or deferred rather than first-version requirements. Choose successful-Guard, Guard-break, Perfect Guard, and turn presentation only when their gameplay triggers exist. These assets currently exist only in the isolated `C:\Unity\Project\RelicGuardianAssetLab` test project and have not been integrated into the main project or runtime-verified there. Clips marked `_RM` must still be used with Apply Root Motion disabled and code-driven displacement.

## Exact Next Development Step

Proceed one concept at a time in this order:

1. Complete only the Block input representation in `PlayerInputReader` and the Input Actions asset. The private persistent-held and one-use-request fields now exist; continue with their read-only exposure, press/release callback, one-use consumption, and a `Pass Through` action so a rejected press cannot start automatically later while the key remains held.
2. Add and verify one centralized Basic Attack cancellation-cleanup boundary in `PlayerCombat`; do not connect Block yet.
3. Connect the minimum Block-over-Basic-Attack rule: Block wins the same-frame Free-state conflict and may cancel the active four-step Basic Attack before `PlayerActionController` enters `Blocking`. Keep future skill restrictions explicit and do not build a general numeric Priority system.
4. Add `PlayerBlock` ownership of Startup, Hold, Release, their decision/exit points, and the final return to `Free` without damage behavior.
5. Connect `Block_Start`, `Block_Loop`, and `Block_End` through code-driven CrossFade and authored Animation Events; preserve Root Motion off.
6. Add phase-specific translation and Sprint permission while preserving the existing unlocked and locked Hold movement/facing behavior.
7. Add the one-shot Startup facing assistance: current authoritative target when locked, temporary forward `120`-degree search when unlocked.
8. Add only the authored Perfect Guard Window lifetime, still without Hit / Block / damage resolution.
9. Run focused Play Mode regressions and check the Console before treating Guard movement and lifecycle as complete.
10. Design the forward `180`-degree Damage / Defense Resolution boundary later as a separate feature.

Do not let Block interrupt future skills implicitly, and do not add parry/counter, a general numeric Priority system, Ability Framework, hierarchical FSM framework, Dodge, or `PlayerMotor` during these first Guard steps.

## Files to Read for the Next Session

Read completely before the next Block-input step:

- `AGENTS.md`
- `Docs/CURRENT_STATE.md`
- `Assets/RelicGuardian/Player/Scripts/PlayerInputReader.cs`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.inputactions`

Before connecting the later `Blocking` lifecycle, also read:

- `Assets/RelicGuardian/Player/Scripts/PlayerActionState.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerAnimator.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerTargeting.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerCombat.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAttack.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAttackPhase.cs`
- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`

Do not redesign `PlayerCombat` merely to define Block input or lifetime. Revisit it only if a later explicit cancellation feature is approved.

Inspect the protected Prefab and Scene only when their current component references or runtime values are needed. Never overwrite them from a tracked baseline.

## Historical-Document Warning

- `Docs/HANDOFF.md` contains multiple chronological checkpoints. Its final `Basic Attack Interruption Priority Revision` section is the latest Guard plan; older Block and movement next steps are historical.
- `Docs/COMBO_ATTACK_ARCHITECTURE.md` begins with the correct implemented four-step status, but some later migration-plan text still describes the older two-step or unimplemented Restart state. Use actual `PlayerCombat.cs` and the newest handoff as authority.
- Older `Docs/DEV_LOG.md` and `Docs/LEARNING_PROGRESS.md` entries are evidence and learning history, not current implementation instructions.

## Updating This File

Update this file when the current implementation, verification gaps, next task, protected dirty files, Git boundary, or relevant-file list changes materially.

Replace stale current-state statements instead of appending a chronological history. Put detailed dated history in `Docs/HANDOFF.md` and `Docs/DEV_LOG.md`.

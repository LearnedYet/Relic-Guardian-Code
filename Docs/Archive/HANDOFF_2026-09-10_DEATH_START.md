# Relic Guardian Current Handoff

> Superseded progress notice, 2026-09-10: the resume instructions below describe the earlier Handoff boundary. Death code, basic runtime acceptance and HitWindow/Recovery interruption checks have since passed. The learner closed this slice and deferred additional precision tests. Follow CURRENT_STATE.md for the next feature; do not repeat the enum edit or automatically resume deferred tests. Learner remains author and requests review after a small functional batch.

Updated: 2026-09-10. The Enemy reaction/attack-pacing checkpoint is complete; the retained-corpse terminal Death slice has been introduced but no Death code has been written.

## Resume

Read [CURRENT_STATE.md — Exact Next Step](CURRENT_STATE.md#exact-next-step) for the sole maintained active step, then select the Enemy AI/Movement/Attack/Health route in [CONTEXT_INDEX.md](CONTEXT_INDEX.md). Preserve the complete startup sequence and authority order in AGENTS.md and the relic-guardian-context skill.

The learner remains the author of key gameplay and presentation code and defaults to performing Scene/Prefab/Inspector/Animator/Event configuration. The learner was taught the responsibility, type, lifetime and name of the `Dead` `EnemyState` enum member, but `EnemyState.cs` was inspected afterward and remains unchanged. Resume by asking the learner to add `Dead` after `Staggered`; inspect the actual file and compile before continuing. Do not interpret the prior `继续`, the interrupted `\+` input, or this Handoff request as authorization for Codex to write Death gameplay code.

## Current Implemented Baseline

- `EnemyStateController` owns coarse `Chase / Attacking / Staggered`, attack admission/natural finish, ordinary surviving-hit timing, post-reaction protection and Global Attack Cooldown.
- Enemy attack Startup admission requires range plus a maximum horizontal facing angle; when only range passes, the enemy stops translation and turns in place.
- EnemyAttack natural finish, interruption and disable share idempotent cleanup. One effective cleanup removes its saved threat/target, resets attack execution and starts Global Attack Cooldown once.
- Repeated ordinary hits during Staggered restart GetHit presentation without extending the gameplay deadline; ordinary hits do not interrupt Attacking.
- The learner runtime-accepted the combined reaction/counterattack rhythm, turn-in-place admission and the corrected Idle-to-Run transition with Exit Time disabled. The final reported Console was clean.
- Confirmed lethal-hit VFX/SFX already use independent spawned lifetimes, so they can remain visible/audible when logical Death is introduced.

## Death Slice Boundary

`EnemyHealth` still subtracts health and calls `gameObject.SetActive(false)` at zero. `EnemyState.cs` still contains only `Chase`, `Attacking` and `Staggered`. Retained-corpse Death, active-attack interruption, movement/threat cleanup, Death Animator presentation, rejection of later hits, and player targeting/Lock-On exclusion remain unimplemented and unverified.

Build the slice in learner-sized steps. First add `Dead` vocabulary. Then add a single terminal death-entry boundary to `EnemyStateController`; route lethal damage to it only after the ownership/call order is clear, and remove prototype deactivation only when the retained-corpse state path is ready. Dead must win over reaction, cancel a live attack safely, stop decisions and movement, never return through a timer/callback, and preserve the independently spawned lethal feedback. Target and hit exclusion belong inside this same Death slice.

Keep Perfect Guard Stagger, multi-attack selection, hit-window geometry validation, Attack Hitstop, camera feedback, Strong Combo and later spacing decisions outside this slice.

## Checkpoint and Working Tree

The latest local full-project checkpoint remains `9da9b25 Add enemy reactions and attack pacing`; the subsequent local record commit is `665c99b`. The flattened GitHub mirror feature is `f1fe243`, and remote `main` was verified at record commit `10e7c3608d0a0c75ee1a21e3425e742abd90d276`. No commit or push is authorized by this Handoff request.

At Handoff creation, the only pre-existing protected dirty files were:

- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/Scenes/SampleScene.unity`

Preserve them. NearTarget wiring and licensed Goblin Animator configuration remain in local Unity state; `Assets/LocalLicensed/` and its `.meta` boundary remain ignored and must never be committed or uploaded.

## Historical Reference

The replaced reaction/attack-pacing Handoff is preserved at `Docs/Archive/HANDOFF_2026-09-09_ENEMY_REACTION_ATTACK_PACING.md`. Detailed prior checkpoints remain in `Docs/DEV_LOG.md` and older files under `Docs/Archive/`; retrieve them only through targeted search.

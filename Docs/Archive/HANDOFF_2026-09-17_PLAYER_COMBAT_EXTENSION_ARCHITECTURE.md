# Relic Guardian Current Handoff

Updated: 2026-09-17. The learner selected Player base-combat completion as the next direction and approved the minimum architecture for Dodge, Perfect Dodge, Counters, Sprint Attack, Player HitStun/HitReaction and Player Death. No part of that future sequence is implemented by this Handoff.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap; actual code/assets remain authoritative and `CURRENT_STATE.md` owns the Exact Next Step. Read `Docs/PLAYER_COMBAT_EXTENSION_DESIGN.md` for the approved future boundary.

The learner remains the author of key gameplay and presentation code. The immediate next task is design, not implementation: let the learner define the visible and control feel of base Dodge before creating identifiers or changing code.

## Approved direction

- `PlayerActionController` remains the only coarse action-state and same-frame input-arbitration owner.
- Intended later coarse states are `Dodging`, `HitStunned` and `Dead`; Perfect outcomes and attack variants are not coarse states.
- A future independent `PlayerDodge` owns Dodge lifetime, direction snapshot, gameplay windows, completion and the later Dodge Counter opportunity.
- `PlayerMovement` remains the sole `CharacterController` displacement and actual-facing writer; Apply Root Motion stays off.
- `PlayerHitReceiver` remains the single incoming-hit entry and will route active Dodge before unhandled damage.
- Basic Attack, Sprint Attack, Guard Counter and Dodge Counter share one `PlayerCombat` executor. Add the minimum attack-entry identity only when the first non-Basic attack is connected.
- `PlayerBlock` and `PlayerDodge` retain their own Counter opportunities; `PlayerActionController` admits the attack and `PlayerCombat` executes it.
- Input priority and authored cancellation permission remain separate. Recommended first Dodge order is `Dodge -> Block -> Attack -> Jump`, while first-version Dodge is admitted only from grounded `Free`.
- Before a new interrupting transition needs it, separate attack-owned cleanup from the controller-owned destination-state decision. Do not build a general interruption framework.

## Immediate design questions

Before coding base Dodge, the learner should choose:

1. free-mode and Lock-On direction behavior;
2. zero-input fallback direction;
3. duration, distance and speed shape;
4. facing during and after Dodge;
5. repeated-input/recovery behavior;
6. desired animation and camera feel.

Keep ordinary Dodge and Perfect Dodge as separate implementation/test slices. Do not add Counter, HitStun or Death while first establishing the base Dodge contract.

## Current implementation remains unchanged

- Player Basic Combo, Guard, Lock-On, Sprint and deterministic `Block -> Attack -> Jump` arbitration remain the current implemented baseline.
- Enemy weighted multi-attack selection, per-attack cooldowns, spacing, directional movement, reaction, Perfect Guard Stagger and retained-corpse Death remain as recorded in `CURRENT_STATE.md` and `ARCHITECTURE.md`.
- The architecture review was static plus live Editor composition inspection. Unity was idle and its Console showed zero errors/warnings; no new Play Mode acceptance was performed.

## Current composition warning

The live `SampleScene` Player uses the connected Player Prefab, but `PlayerBlock`, `PlayerHitReceiver`, `PlayerGuardPresentation`, `HitstopController` and `PlayerAttackPresentation` are Scene-added components. The Prefab alone is therefore not a complete reusable Player. This does not block the current single-Scene Dodge slice; do not combine a broad Prefab/Scene migration with Dodge while protected mixed assets and local presentation wiring remain in place.

## Protected local state

Keep these pre-existing files unstaged unless a future task explicitly scopes them:

- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/Scenes/SampleScene.unity`

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` remain local-only and must never be committed or uploaded. Local full-project history and flattened GitHub mirror history remain separate.

The preceding Handoff is archived at `Docs/Archive/HANDOFF_2026-09-16_ENEMY_SPACING.md`.

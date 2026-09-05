# Relic Guardian Current Handoff

Last updated: 2026-09-05.

This file contains only the latest cross-conversation Handoff. The prior 2026-08-31 Handoff is preserved at `Docs/Archive/HANDOFF_2026-08-31_GUARD_VFX_RESOURCES.md`.

## Context Entry Points

For a new task or post-compaction recovery, read `AGENTS.md`, `Docs/CURRENT_STATE.md`, `Docs/ARCHITECTURE.md`, this file, then use `Docs/CONTEXT_INDEX.md` to select one bounded route. Run `git status --short --branch` before relying on documented state.

## Actual Implementation State

- `PlayerActionController` remains the sole coarse-state owner for `Free`, `Attacking`, and `Blocking`; `PlayerBlock` owns Guard phases, Coverage, pre-hit Facing Assist, and production of `GuardResult.Unhandled`, `Ordinary`, or `Perfect`.
- `PlayerHitReceiver` is the result-routing boundary: `Unhandled` reaches `PlayerHealth`, while Ordinary/Perfect reach `PlayerGuardPresentation` once. Presentation does not decide Guard legality or damage.
- `PlayerGuardPresentation` owns independent Ordinary/Perfect Guard VFX resources and lifetimes plus independent `CombatAudioData`. Each handled branch spawns only its matching effect and submits only its matching cue.
- `CombatAudioLayer` stores one Clip/Volume/Pitch/Delay record; `CombatAudioData` stores Master Volume plus a variable layer array. `CombatAudioPlayer` owns four preconfigured channels, stops prior scheduled playback, maps valid layers, and uses one DSP-time base plus per-layer delay.
- `HitstopController` is the sole current `Time.timeScale` writer/restorer for Hitstop. Perfect Guard requests `0.07s`; Ordinary and unhandled hits do not request it. The owner uses an unscaled absolute deadline, later-deadline overlap, exact prior-scale restoration, normal/disable cleanup, and disabled-request rejection.
- `SampleScene` contains the local `GuardImpactAnchor` and `CombatAudio` hierarchy under the player. Ordinary uses the final Normal Guard Impact and three SFX layers; Perfect uses the final Perfect Guard Impact and four SFX layers with a `0.030s` fourth-layer accent.
- The local Animator state speeds are `Block_Start = 2` and `Block_End = 1.5`. The actual `Block_Start.anim` close/decision Events are at clip times `0.3s` and `0.4s`.
- The learner-selected `Block_Hit.anim` is connected to the full-body Override `Guard Reaction` layer with Speed `2`, Exit Time `0.9`, and fixed `0.08s` exit blending. Ordinary alone requests it; automatic exit and `PlayBlockEnd()` return the layer to `Empty`.
- Subtle 1 is connected Scene-locally as the independent always-on `WeaponAura`. Subtle 2 is connected to the shared blade endpoints as `AttackTrail`; Attack1-4 use independent normalized windows `0.18911798 -> 0.41242826`, `0.2189475 -> 0.37530434`, `0.17673774 -> 0.34686896`, and `0.3032368 -> 0.4152874`. `PlayerAttackPresentation` owns VFX playback, while `PlayerCombat` validates indices `0/1/2/3` and guarantees new-step/end/cancel cleanup. The full four-step runtime checks passed. Attack4 deliberately remains in the same ordinary basic-attack Trail tier.
- `FX_hit_03_Blood.prefab` plus its two independent Blood materials are imported under the ignored selected AttackHits boundary and selected as the primary ordinary-Attack confirmed-hit VFX candidate. All dependencies resolve, but it is not connected or real-camera tuned.
- Attack Motion audio is connected through a separate Scene-local two-channel `AttackAudio` player. Attack1-3 use accepted one-layer indexed Whoosh cues at normalized times `0.23050807`, `0.21698608`, and `0.20805433`. Attack4 uses `PlayWeaponWindup(3)` at `0.05673332` and `PlayWeaponWhoosh(3)` at `0.3182363` with no fixed delayed layer. The selected two confirmed-hit clips remain unconnected.
- Apply Root Motion remains off. `PlayerMovement` remains the sole player Transform-facing owner, and the enemy attack remains a scheduled hit attempt without Hit Window-time physical confirmation.

## Runtime and Asset Verification

- On 2026-09-02 the learner runtime-verified distinct Ordinary and Perfect Guard VFX/SFX, one matching feedback group per hit without crossover or duplicate playback, preserved no-damage Guard handling, and a clean Console.
- A separate unblocked-hit regression damaged the player once and produced no Guard VFX/SFX or Console exception.
- `CombatAudioPlayer.OnDisable()` cleanup is implemented but was not recorded as a separate focused Runtime test.
- `Assembly-CSharp.csproj` compiled with zero warnings and zero errors after the Guard Hitstop connection.
- The learner runtime-verified Perfect-only `0.07s` Hitstop, automatic recovery, Ordinary/unhandled exclusion, preserved damage and VFX/SFX routes, post-recovery control, disabled-owner recovery/rejection, and a clean Console. Overlap is code-reviewed but not independently runtime-tested for the current single-hit enemy.
- The learner runtime-accepted `Block_Start = 2` and `Block_End = 1.5` in the combined Guard flow. The actual local `ClosePerfectGuardWindow` Event is at clip time `0.3s`; older `0.16666667s` documentation is superseded.
- Licensed Combat VFX/SFX remain under ignored `Assets/LocalLicensed/`. Exact resources and accepted values are in `Docs/COMBAT_VFX_RESOURCE_TRACKING.md` and `Docs/COMBAT_SFX_RESOURCE_TRACKING.md`.
- Local full-project feature checkpoint: `472d946 Complete Guard reactions and attack motion feedback`. GitHub feature-mirror checkpoint: `e484ca1 Sync Guard reactions and attack motion feedback`; the push was verified at full hash `e484ca14e425aba9757fc85377b1d70b4c68b6b3` before the record-only documentation follow-up.

## Exact Next Concept

Ordinary Guard Movement Lock and the independent player reaction are implemented and learner-reported runtime verified. `PlayerBlock` owns the scaled `0.45s` deadline inside the existing `Blocking` lifecycle, extends `AllowsMovement`, retains the later deadline on overlap, and delays Release until the committed impact boundary. Ordinary alone starts the lock and reaction; Perfect remains excluded. The learner remains the default author for key gameplay and presentation code.

Guard expansion remains paused. Next connect only the selected confirmed Attack Hit VFX/SFX after the existing gameplay confirmation. Do not combine enemy reaction, Perfect player reaction, Camera feedback, Counter, or Guard Break into the same slice.

## Protected Git State

- The protected Unity working state retains `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`, `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`, `Assets/Scenes/SampleScene.unity`, and the index-only `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs` entry. Do not reset, overwrite, or broadly stage them.
- `HitstopController`, `PlayerAttackPresentation`, their project-owned integrations, maintained documentation, and the archived prior Handoff are included in local feature checkpoint `472d946` and GitHub feature mirror `e484ca1`.
- The current record-only documentation follow-up updates checkpoint references after that verified feature push.
- The Scene contains the locally wired Guard VFX/SFX references and therefore must remain outside GitHub unless a separate reproducible replacement plan is approved.
- `Assets/LocalLicensed/CombatVFX/`, `Assets/LocalLicensed/CombatSFX/`, the imported `Assets/LocalLicensed/SwordAnimationPack/Guard/Block_Hit.anim`, and all other `Assets/LocalLicensed/` content are ignored licensed assets and must never be committed or uploaded.
- The GitHub repository is the separate flattened code/document mirror. Do not pull its `main` into the full Unity workspace.
- No further staging, commits, pushes, history rewrites, or remote mutations are authorized beyond the learner-requested checkpoint synchronization recorded here.

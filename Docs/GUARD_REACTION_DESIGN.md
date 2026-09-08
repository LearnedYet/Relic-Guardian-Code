# Guard Reaction Design

Status: implemented and learner-reported runtime verified on 2026-09-03 for Perfect Guard Hitstop plus Ordinary Guard Movement Lock and player reaction.

## Current Boundary

The implemented Guard result path remains:

```text
PlayerBlock.ResolveGuardHit(HitContext)
-> GuardResult.Unhandled, Ordinary, or Perfect
-> PlayerHitReceiver
   |- Unhandled -> PlayerHealth
   `- Ordinary/Perfect -> PlayerGuardPresentation
```

Ordinary and Perfect Guard already have distinct VFX and layered SFX. Perfect alone requests the shared `HitstopController` for `0.07s`; Ordinary has no Hitstop. Ordinary player reaction is connected. Camera Impulse, FOV change, camera pull, enemy reaction/Stagger and Counter remain future work.

## Saved Guard Timing and Historical Acceptance

Static audit: 2026-09-06. These values come from saved Scene overrides, code defaults, the player Controller and local Guard Clips; this audit is not a fresh runtime test.

| Parameter | Current saved value / source |
| --- | --- |
| Block_Start / Block_End state Speed | 2 / 1.5, Controller |
| ClosePerfectGuardWindow | 0.35 Clip seconds, Block_Start.anim |
| StartupDecisionPoint | 0.4 Clip seconds, Block_Start.anim |
| FinishRelease | 0.75 Clip seconds, Block_End_NoRootTurn.anim |
| Ordinary Movement Lock | 0.45 game seconds, Scene and code default |
| Guard crossfade | 0.03 seconds, Scene override; code default 0.12 |
| Block exit crossfade / soft-recovery interrupt | 0.45 / 0.05 seconds, Scene and code |
| Reaction start/clear crossfade | 0.03 seconds, code default |
| Ordinary_Guard_Hit Speed / Exit Time / fixed exit blend | 2 / 0.9 normalized / 0.08 seconds |
| Perfect-only Hitstop | 0.07 unscaled seconds, Scene and code |
| Coverage / Facing Assist half-angle | 90 / 60 degrees, Scene |
| Block_Start / Block_End_NoRootTurn rotation offset | -66 degrees, local Clip settings |

Historical acceptance: 2026-09-02 recorded ClosePerfectGuardWindow at 0.3 Clip seconds (an older 0.16666667-second close is superseded); the saved 0.35-second close is later tuning and has no separate runtime acceptance claim in this audit. The 2026-09-03 reaction checkpoint accepted Speed 2, Exit Time 0.9, 0.08s blend and Scene lock 0.45s. Historical soft-exit tuning and forward Guard walk offset -36 degrees are retained in DEV_LOG.md (Guard rotation/Hold sections); other directional clips were not individually accepted by that checkpoint.

Clip seconds, normalized Exit Time and game/unscaled seconds are distinct. State Speed changes the real-time Event boundary. Exact VFX/SFX parameters live in their resource tracking documents.

## Selected Ordinary Guard Reaction Asset

The learner selected `Block_Hit` from the local licensed Sword Animation Pack after previewing it on the P09 character in `RelicGuardianAssetLab`.

- AssetLab source: `Assets/SwordAnimationPack/Animation/Humanoid/08_Hit/12_Block/Block_Hit.anim`
- Formal local path: `Assets/LocalLicensed/SwordAnimationPack/Guard/Block_Hit.anim`
- Preserved GUID: `7db438147f2220249abbe5611214ea2e`
- Authored length: approximately `0.8333334s`
- Formal-copy Loop Time: disabled
- Animation Events: none
- Additive reference pose: none
- Integration state: connected and runtime accepted through the independent full-body Override `Guard Reaction` layer

The Clip begins with a visible Guard impact and later recovers to a Guard pose. Playback Speed and the gameplay Movement Lock duration are tuning values to verify in the real combat camera; they are not accepted merely from the AssetLab preview.

## Ordinary Guard Reaction Responsibility

Ordinary Guard splits the already-resolved result into independent Gameplay and Presentation work:

```text
Ordinary Guard resolved
|- Gameplay: PlayerBlock begins a short Guard-impact Movement Lock
`- Presentation: PlayerGuardPresentation requests PlayerAnimator to play Block_Hit
```

`PlayerGuardPresentation` must not start, extend, or finish the Movement Lock. Missing presentation references must never remove the gameplay consequence. `PlayerBlock` owns the Movement Lock because it already owns the Blocking phase and the derived Hold movement permission. `PlayerAnimator` owns only the reaction-layer playback.

## Movement Lock Boundary

The Movement Lock is an orthogonal deadline inside the existing `Blocking` action. It is not a new `PlayerActionState`, `BlockPhase`, `GuardHitState`, or Animator-owned gameplay state.

- Entry: only after a legal hit resolves as `GuardResult.Ordinary`.
- Accepted current tuning and serialized code default: `0.45s`.
- Clock: scaled `Time.time`, so Hitstop or a future Pause also pauses the gameplay lock.
- Overlap: do not add durations; retain the later absolute deadline with `max(oldEndTime, Time.time + requestedDuration)`.
- Permission effect: extend `PlayerBlock.AllowsMovement`; do not consume movement input or modify Sprint, Jump, damage, Guard Coverage, or Perfect classification.
- Startup: already cannot move; an Ordinary hit late in Startup may carry the remaining lock into Hold.
- Hold: normal movement resumes automatically when the deadline expires if Block remains held.
- Hold release: Update waits for the lock deadline before EnterRelease; held input still controls movement eligibility. The player retains Hold coverage during this interval.
- Existing Startup boundary: StartupDecisionPoint directly enters Release when Block is not held; it currently does not test the lock deadline. Thus the implemented delay is a Hold rule, not a verified all-phase guarantee. A late-Startup Ordinary hit followed by release needs a focused future check; this documentation task changes no code.
- Reset: BeginBlock and EnterRelease clear the deadline. FinishRelease returns the coarse action to Free.

With the current `PlayerMovement` implementation, making `PlayerActionController.CanMove` false also pauses ordinary locked-target facing for the short lock, while active Guard Facing Assist remains the higher-priority facing branch. The learner accepted the current gameplay timing in the focused runtime test; any later facing split remains a separate change.

Movement Lock should visually cover the committed impact portion, not necessarily the complete visual recovery tail. The full `Block_Hit` Clip may continue returning toward Guard after control resumes. Do not read `AnimationClip.length` from Presentation to decide gameplay permission, and do not let an Animator transition become the only reliable unlock path.

## Animator Layer Direction

The Controller contains Base Layer and the separate Guard Reaction layer so the existing Base Layer continues evaluating `Block_Start`, Guard Hold, and `Block_End`, including `ClosePerfectGuardWindow`, `StartupDecisionPoint`, and `FinishRelease`.

The accepted first layer uses:

- A full-body Override layer with no Avatar Mask because the selected impact uses torso, hips, and legs.
- An `Empty` default state and one non-looping `Ordinary_Guard_Hit` state.
- Each resolved Ordinary hit may restart the reaction from the beginning.
- Entering Release must clear or fade the reaction layer so it does not visually cover `Block_End`.
- Perfect Guard does not request this player reaction in the first version.

The accepted reaction uses Speed `2`, Exit Time `0.9`, and fixed `0.08s` exit blending. The first Speed `1` test exposed sliding because the full-body recovery tail still overrode locomotion after movement unlocked; the final accelerated timing hands the legs back to Base Layer at approximately the accepted `0.45s` control boundary.

## Historical Completion and Future Design References

Movement Lock and the separate reaction layer were completed as two checkpoints on 2026-09-03. The earlier cross-feature development order is superseded by ENEMY_COMBAT_AGENT_DESIGN.md and ROADMAP.md. CURRENT_STATE.md alone maintains the active Exact Next Step. This document retains the Guard reaction contract and timing evidence; it does not maintain a competing next-step list.

## Attack Soft-Recovery Timing Reference

Preserved here with the shared Guard/Attack soft-recovery contract rather than repeated in startup documents. Saved Attack4 FBX importer values audited 2026-09-06: OpenHitWindow(3) 0.31615335, CloseHitWindow(3) 0.39201885, FinishAttack(3) 0.59016937, all normalized; Controller Speed 1.15. The early FinishAttack establishes the previously runtime-accepted visual soft tail. Full attack-flow design remains in COMBO_ATTACK_ARCHITECTURE.md.

## Deferred and Excluded

- No Ordinary Guard Hitstop in the first version.
- No Perfect player Guard reaction in the first version.
- No Camera Impulse, FOV change, camera pull, or Boss-heavy feedback yet.
- No Guard Break, Heavy Guard Reaction, enemy Stagger, Counter Window, or automatic Counter.
- No Base Layer reaction transition that can interrupt the existing Guard lifecycle Events.
- No general Reaction FSM, numeric Priority system, event bus, Damage Framework, or Ability Framework.

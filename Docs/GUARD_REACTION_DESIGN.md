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

Ordinary and Perfect Guard already have distinct VFX and layered SFX. Perfect alone requests the shared `HitstopController` for `0.07s`; Ordinary has no Hitstop. No Camera Impulse, FOV change, camera pull, player reaction, enemy reaction, Stagger, or Counter is implemented.

Current locally accepted Guard timing uses Animator state Speed `2` for `Block_Start` and `1.5` for `Block_End`. The actual local `Block_Start.anim` Events are `ClosePerfectGuardWindow` at clip time `0.3s` and `StartupDecisionPoint` at `0.4s`. These authored Events continue to define the existing Guard lifecycle until a separately approved timing refactor exists.

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

Ordinary Guard will split the already-resolved result into independent Gameplay and Presentation work:

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
- Release: releasing Block during the lock records the existing held-input change but delays `EnterRelease()` until the gameplay deadline. The player remains in the current Blocking/Hold lifecycle and retains Guard coverage during this committed interval.
- Exit/reset: beginning a new Block and completing Release must not inherit a stale prior lock.

With the current `PlayerMovement` implementation, making `PlayerActionController.CanMove` false also pauses ordinary locked-target facing for the short lock, while active Guard Facing Assist remains the higher-priority facing branch. The learner accepted the current gameplay timing in the focused runtime test; any later facing split remains a separate change.

Movement Lock should visually cover the committed impact portion, not necessarily the complete visual recovery tail. The full `Block_Hit` Clip may continue returning toward Guard after control resumes. Do not read `AnimationClip.length` from Presentation to decide gameplay permission, and do not let an Animator transition become the only reliable unlock path.

## Animator Layer Direction

The current Controller has only `Base Layer`. Ordinary Guard Reaction requires a separate layer so the existing Base Layer continues evaluating `Block_Start`, Guard Hold, and `Block_End`, including `ClosePerfectGuardWindow`, `StartupDecisionPoint`, and `FinishRelease`.

The accepted first layer uses:

- A full-body Override layer with no Avatar Mask because the selected impact uses torso, hips, and legs.
- An `Empty` default state and one non-looping `Ordinary_Guard_Hit` state.
- Each resolved Ordinary hit may restart the reaction from the beginning.
- Entering Release must clear or fade the reaction layer so it does not visually cover `Block_End`.
- Perfect Guard does not request this player reaction in the first version.

The accepted reaction uses Speed `2`, Exit Time `0.9`, and fixed `0.08s` exit blending. The first Speed `1` test exposed sliding because the full-body recovery tail still overrode locomotion after movement unlocked; the final accelerated timing hands the legs back to Base Layer at approximately the accepted `0.45s` control boundary.

## Development Order After Ordinary Guard Reaction

1. Implement and runtime-verify only Ordinary Guard Movement Lock. Completed on 2026-09-03 with a current Scene value of `0.45s`, later-deadline overlap, Ordinary-only entry, and delayed Release.
2. Build and runtime-verify the separate player Guard reaction layer using the selected `Block_Hit`. Completed on 2026-09-03.
3. Pause Guard expansion.
4. Add Attack Motion feedback: authored per-attack Weapon Trail windows and separate Whoosh cues. Begin basic-Attack tuning from the imported ice-blue `Subtle 1/2` candidates and the imported Attack1-3/Attack4 motion-audio candidates; reserve `Ice Stylized 3` for a future Perfect Guard Counter and `Ice Water 1/2` for other higher-emphasis attacks.
5. Add confirmed Attack Hit VFX/SFX only after the existing gameplay confirmation, beginning visual tuning from imported `FX_hit_03_Blood` and audio tuning from the selected sword-impact plus flesh/gore layers.
6. Use normal player Attack hits as the first concrete need for a minimal enemy light-reaction receiving boundary.
7. Return to Perfect Guard and decide whether enemy recoil remains presentation-only or becomes Recovery/Stagger gameplay.
8. Design a non-automatic Counter Window and Counter Attack only after the enemy consequence is explicit.

The current outgoing player hit path directly calls `EnemyHealth.TakeDamage(int)` from `PlayerCombat.OpenHitWindow()`. Before Attack Hit feedback and Enemy Reaction are connected, define the smallest victim-side receiving/presentation seam required by that real feature. Do not prebuild a large damage or reaction framework.

## Deferred and Excluded

- No Ordinary Guard Hitstop in the first version.
- No Perfect player Guard reaction in the first version.
- No Camera Impulse, FOV change, camera pull, or Boss-heavy feedback yet.
- No Guard Break, Heavy Guard Reaction, enemy Stagger, Counter Window, or automatic Counter.
- No Base Layer reaction transition that can interrupt the existing Guard lifecycle Events.
- No general Reaction FSM, numeric Priority system, event bus, Damage Framework, or Ability Framework.

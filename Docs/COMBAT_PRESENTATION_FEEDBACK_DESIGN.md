# Combat Presentation Feedback Design

Status: approved direction on 2026-08-31 and revised through 2026-09-04. Guard feedback through Ordinary player reaction and Attack1-4 Motion feedback through Trail plus Whoosh are implemented and learner-reported runtime verified. Confirmed-hit layers remain pending unless a later current-state entry says otherwise.

## Goal

Build clear, layered combat feel on top of the existing Attack, Ordinary Guard, Perfect Guard, and Guard Facing Assist resolution without changing their gameplay rules.

Add only one presentation layer at a time. Verify that layer in Play Mode before beginning the next one.

## Three Separate Boundaries

### Gameplay Resolution

Determines what happened:

- an ordinary attack hit;
- an Ordinary Guard handled the hit;
- a Perfect Guard handled the hit;
- an unhandled hit continued to health damage.

### Presentation Feedback

Expresses an already-decided result:

- VFX;
- SFX;
- Hitstop;
- Camera Impulse;
- Weapon Trail;
- presentation-only Hit Reaction;
- Screen or UI FX.

Presentation feedback must never decide damage, Guard Coverage, Perfect Guard timing, action permission, or whether the hit was handled.

### Gameplay Consequence

Changes combat rules or permissions:

- Enemy Stagger or forced recovery;
- Counter Window;
- Guard Break;
- resource rewards;
- forced hitstun;
- Parry or Counter mechanics.

These are separate future gameplay concepts. They are not part of the base combat-feedback phase.

## Minimum Guard Result to Presentation Boundary

The implemented `PlayerBlock` remains the owner of Guard phase, coverage, and Ordinary-versus-Perfect classification. It produces the result only after the existing legal-hit checks succeed.

`PlayerHitReceiver` remains the hit-routing boundary. It forwards unhandled hits to `PlayerHealth` and should route a handled Guard result toward presentation without moving damage permission into the presentation layer.

A focused player Guard presentation owner is the current smallest candidate for consuming the result. It should own only presentation references, spawn placement and orientation, and spawned-effect cleanup. It must not inspect Guard timing or recalculate whether the hit was legal.

Working identifier candidates, to be explained again before learner code entry:

- `GuardResult`: `Guard` means Guard resolution and `Result` means the outcome already decided by gameplay. It is a short-lived value for one `ReceiveHit` call, with only the minimum distinctions needed by the current route: unhandled, Ordinary, and Perfect.
- `PlayerGuardPresentation`: `Player` identifies the receiving actor, `Guard` narrows the feature, and `Presentation` means visible or audible feedback. It is a component-lifetime owner of presentation configuration and temporary effect instances.

This is smaller than an event bus, general hit-result hierarchy, Damage Framework, Ability Framework, or shared numeric Priority system. Final names and exact signatures remain a learner-first code decision immediately before implementation.

## Guard Impact Placement Policy

The current enemy delivers a scheduled `HitContext`, not a physical collision contact. `HitContext` contains damage, source identity, and incoming direction, but no real contact point.

The first Guard VFX therefore uses a configurable player Guard impact anchor and the incoming direction for orientation. Do not claim this anchor is a physical contact point, and do not expand the hit-detection model merely to connect the first effect. A real contact point can be added later only when a concrete physical hitbox or projectile requirement provides it.

## Approved Development Order

1. Establish and runtime-verify the minimum Guard result-to-presentation route without adding a feedback layer.
2. Connect only Ordinary Guard VFX.
3. Connect a visually distinct Perfect Guard VFX.
4. Add Guard SFX, with different audio content for Ordinary and Perfect rather than volume-only differentiation.
5. Add and runtime-verify Perfect-only Hitstop without changing resolution rules.
6. Add Ordinary Guard player impact reaction in two separate checkpoints: gameplay-owned Movement Lock, then presentation-owned reaction-layer playback.
7. Pause Guard expansion and add Attack Motion feedback: authored weapon Trail windows and separate Whoosh cues.
8. Add confirmed Attack Hit VFX and SFX.
9. Establish the minimum enemy light-hit reaction boundary from real player Attack hits.
10. Return to Perfect Guard enemy recoil/Stagger design, then design a non-automatic Counter Window only after its gameplay consequence is explicit.
11. Consider Camera Impulse, Screen/UI FX, and Boss-heavy feedback only as later separate layers.

## Current Exact Slice

The Guard result-to-presentation boundary and distinct Guard VFX/SFX are implemented and runtime verified. Ordinary uses:

`Assets/LocalLicensed/CombatVFX/Selected/GuardImpacts/Normal Guard Impact.prefab`

Perfect uses the separately selected:

`Assets/LocalLicensed/CombatVFX/Selected/GuardImpacts/Perfect Guard Impact.prefab`

Both VFX branches are implemented and runtime verified as explicitly distinct through `GuardResult`; neither effect is layered into the other. Both selected Prefabs use non-looping, Play On Awake Particle Systems with Stop Action None, so Presentation owns explicit cleanup. Their Particle Renderers are View-aligned; `IncomingDirection` is carried but not used for rotation in the current implementation.

The selected Guard SFX resources and exact layer settings are imported and recorded in `Docs/COMBAT_SFX_RESOURCE_TRACKING.md`. The 3-layer Ordinary and 4-layer Perfect routes use reusable serialized cue data and one DSP-scheduled playback component.

Perfect alone requests `0.07s` from the shared `HitstopController`. Its unscaled deadline, overlap extension, exact prior-scale restoration, normal expiry, disabled-owner recovery, and disabled-request rejection are implemented. Ordinary and unhandled hits do not request Hitstop. The learner runtime-verified the current branch isolation, recovery, existing damage/VFX/SFX behavior, and clean Console; overlap is code-reviewed but not independently runtime-tested for the current single-hit enemy.

Ordinary Guard Reaction and Attack Motion feedback are complete. Scene-local Subtle 1 is the always-on `WeaponAura`; Subtle 2 is the indexed Attack1-4 `AttackTrail`. A separate two-channel `AttackAudio` bank plays one indexed Whoosh for Attack1-3, while Attack4 uses separate Windup and main-swing pose Events rather than a delayed future layer. `PlayerCombat` validates all indexed Events before `PlayerAttackPresentation` selects and submits presentation data. Confirmed Attack Hit VFX/SFX is next.

## Later Feedback Notes

### Guard SFX

- Ordinary Guard: ordinary metal impact or weapon clash.
- Perfect Guard: a separate, sharper and clearly recognizable sound, not merely a louder Ordinary clip.

### Hitstop

Initial guidance and current accepted Guard value:

- light ordinary attack: approximately `0.03-0.06s`;
- Ordinary Guard: no Hitstop in the current version;
- Perfect Guard: accepted current value `0.07s`.

The implemented global `Time.timeScale` owner uses `Time.unscaledTime`, preserves the pre-Hitstop scale only on first entry, keeps the later overlap deadline, and restores normally or on disable. It remains a small execution component and does not classify hits. A future Pause or Slow Motion system must explicitly coordinate global time ownership rather than independently saving and restoring the same value.

### Camera Impulse

- ordinary attack: light;
- Ordinary Guard: light-to-medium;
- Perfect Guard: short and clear;
- no prolonged large-amplitude shake.

Verify the actual Cinemachine `3.1.7` component and Editor UI before giving version-sensitive setup instructions.

### Attack Hit Feedback and Weapon Trail

Whoosh and weapon Trail belong to the authored attack motion. Hit VFX, impact SFX, Hitstop, and Camera Impulse begin only after the existing gameplay hit is confirmed.

The later Trail window may use authored open/close Animation Events, but Trail state never decides a Hit Window or damage result.

### Hit Reaction

A short presentation-only reaction is acceptable only while it does not interrupt an attack, skip or reopen Animation Events, change an enemy phase, or block gameplay permission. Once it does any of those things, it becomes a separately designed Gameplay Consequence.

The approved Ordinary Guard player reaction deliberately contains both sides but keeps them separate: `PlayerBlock` owns a short scaled Movement Lock, while `PlayerGuardPresentation -> PlayerAnimator` owns the independent reaction layer. The selected Clip visually recovers toward Guard; movement may resume at the recoverable point while the visual tail continues. Presentation must never be the authority that grants movement permission.

## Verification Contract

The minimum result route passed its focused runtime check:

- Ordinary and Perfect results reach distinct presentation branches;
- unhandled hits still reach `PlayerHealth` exactly once;
- handled Guard hits still prevent damage exactly once;
- no presentation code decides gameplay;
- Console remains clean.

The Ordinary Guard VFX layer passed its focused runtime check on 2026-09-01:

- Ordinary Guard spawns exactly one Ordinary impact;
- Perfect Guard does not spawn the Ordinary impact;
- Free, rear, Release, invalid-direction, or otherwise unhandled hits do not spawn a Guard impact and still follow existing damage rules;
- the effect appears at the configured anchor with an understandable orientation;
- the spawned instance is cleaned up;
- no stale Animation Event can create a Guard impact because the effect is driven by the hit-time resolved result;
- Console remains clean.

Each later layer repeats the same Ordinary, Perfect, unhandled-hit, duplicate-feedback, cleanup, and Console regression checks before the next layer begins.

## Explicit Non-Goals

- No new damage or Guard rule.
- No Enemy Stagger, Counter Window, Guard Break, forced hitstun, Parry, or Counter.
- No VFX- or audio-driven gameplay decision.
- No physical Hitbox rewrite for the first Guard impact.
- No general event bus, result hierarchy, Damage Framework, Ability Framework, or numeric Priority system.
- No simultaneous implementation of every feedback layer.

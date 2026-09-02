# Combat Presentation Feedback Design

Status: approved direction on 2026-08-31 and reviewed on 2026-09-02. The minimum `GuardResult -> PlayerGuardPresentation` boundary and distinct Ordinary/Perfect Guard VFX and layered SFX are implemented and learner-reported runtime verified. Later feedback layers remain unimplemented unless a later current-state entry says otherwise.

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
5. Add Hitstop as feedback without changing resolution rules.
6. Add short Camera Impulse with increasing but restrained emphasis from ordinary attack to Ordinary Guard to Perfect Guard.
7. Add Attack hit-confirmed VFX and SFX.
8. Add a weapon-motion Trail controlled by its own authored window; it never decides damage.
9. Consider presentation-only Hit Reaction.
10. Consider restrained Screen or UI FX, then tune the complete feedback stack together.

## Current Exact Slice

The Guard result-to-presentation boundary and first Ordinary Guard VFX layer are implemented and runtime verified. Ordinary uses:

`Assets/LocalLicensed/CombatVFX/Selected/GuardImpacts/Normal Guard Impact.prefab`

Perfect uses the separately selected:

`Assets/LocalLicensed/CombatVFX/Selected/GuardImpacts/Perfect Guard Impact.prefab`

Both VFX branches are implemented and runtime verified as explicitly distinct through `GuardResult`; neither effect is layered into the other. Both selected Prefabs use non-looping, Play On Awake Particle Systems with Stop Action None, so Presentation owns explicit cleanup. Their Particle Renderers are View-aligned; `IncomingDirection` is carried but not used for rotation in the current implementation.

The selected Guard SFX resources and exact layer settings are imported and recorded in `Docs/COMBAT_SFX_RESOURCE_TRACKING.md`. The 3-layer Ordinary and 4-layer Perfect routes now use reusable serialized cue data and one DSP-scheduled playback component. Hitstop is the next separate learner-led layer.

## Later Feedback Notes

### Guard SFX

- Ordinary Guard: ordinary metal impact or weapon clash.
- Perfect Guard: a separate, sharper and clearly recognizable sound, not merely a louder Ordinary clip.

### Hitstop

Initial tuning ranges, not accepted final values:

- light ordinary attack: approximately `0.03-0.06s`;
- Ordinary Guard: approximately `0.04-0.07s`;
- Perfect Guard: approximately `0.07-0.12s`.

A global `Time.timeScale` implementation can affect enemy phases, Animator playback, particles, movement, and other clocks. Before implementation, define one owner, start/end boundaries, overlap behavior, and guaranteed restoration. Do not let Hitstop create duplicate or delayed gameplay resolution.

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

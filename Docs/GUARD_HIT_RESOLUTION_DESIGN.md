# Guard Hit Resolution, Pre-Hit Facing Assist, and Perfect Guard Design

Status: revised on 2026-08-31. `HitContext`, `PlayerHitReceiver`, Startup/Hold Guard Coverage, `AttackThreatContext`, pre-hit Facing Assist, and the minimal Perfect Guard Window/classification are implemented and runtime-verified for the current enemy. Presentation consequences remain pending.

This document supersedes every earlier plan that made Guard search for a target, reuse Lock-On for defense, or wait until the hit landed before beginning Facing Assist.

## Goal

When an enemy begins a real authored attack Startup, it announces the fixed incoming direction and expected impact time. If the player Blocks before impact, `Block_Start` and smooth facing begin together; if Startup reaches Hold first, the same assist continues without replaying Block or waiting for the hit. The actual hit still travels through `HitContext` and remains the only damage/Guard-resolution boundary.

Keep this as a small reusable seam for the current enemy and later Boss attack rhythms. Do not create a general Damage Framework, Ability Framework, event bus, interface hierarchy, numeric Priority system, or second gameplay FSM.

## Implemented Data Contracts

### HitContext

`HitContext` is the immutable hit-time value:

- `int DamageAmount`: health damage if the hit is unhandled.
- `Transform Source`: gameplay source identity.
- `Vector3 IncomingDirection`: normalized world-space hit-time direction travelling toward the receiver.

`PlayerHitReceiver.ReceiveHit(HitContext)` resolves same-frame action requests, delegates Blocking hits to `PlayerBlock`, and forwards only unhandled damage to `PlayerHealth`.

### AttackThreatContext

`AttackThreatContext` is the separate immutable pre-hit preview value:

- `Transform Source`: the same source identity later used by `HitContext`.
- `Vector3 IncomingDirection`: normalized world-space direction snapshot created at Startup.
- `float ExpectedImpactTime`: absolute expected impact time on Unity's `Time.time` clock.

It contains no damage. A threat is not a hit and cannot reduce health or resolve Perfect Guard by itself.

## Implemented Runtime Flow

```text
EnemyAttack.TryStartAttack(PlayerHitReceiver)
-> save target and enter Startup
-> construct AttackThreatContext(
     Source = enemy Transform,
     IncomingDirection = enemy toward player,
     ExpectedImpactTime = Time.time + startupDuration
   )
-> PlayerHitReceiver stores the threat by Source
-> player Blocks after preview, or preview arrives while player is in Startup/Hold
-> PlayerBlock selects the earliest valid future threat
-> validate phase, time, coverage angle, and assist angle
-> store fixed direction, source, expected-impact end time, and pre-assist facing
-> PlayerMovement applies Assist -> Locked -> Free Movement facing order
-> EnemyAttack.OpenHitWindow removes the preview
-> construct and deliver the real HitContext
-> matching hit resolves Guard Coverage from saved pre-assist facing
-> clear assist
```

Both same-frame orders work without Script Execution Order configuration:

- preview first: it is stored, then `BeginBlock()` queries it;
- Block first: the later `ReceiveAttackThreat()` sees Blocking and offers the stored earliest threat to `PlayerBlock`.

## Threat Storage and Selection

`PlayerHitReceiver` stores one current threat per `Source` in a small dictionary. A new threat from the same enemy replaces that enemy's prior entry; different enemies can coexist. Selection ignores null or expired entries and returns the valid threat with the earliest `ExpectedImpactTime`.

The current enemy removes its threat at Hit Window. Expired entries are ignored rather than modified during dictionary enumeration. A dedicated cancellation route is deferred until an enemy attack can actually be interrupted or cancelled before impact.

No `AttackId` is needed while one enemy source cannot own overlapping attacks. Add a per-strike identity only when a concrete overlapping multi-hit requirement appears.

## Guard Phase and Direction Policy

- Startup can defend and may use Facing Assist.
- Hold can defend and may continue or begin Facing Assist.
- Release cannot defend and clears Facing Assist.
- Perfect Guard Window is a short authored subset of Startup.
- A legal Startup hit outside Perfect Guard Window remains an ordinary Guard.

Direction policy:

- Empty Guard performs no Physics search and no Lock-On target acquisition.
- Assist begins only from a real `AttackThreatContext`.
- Preview direction is projected horizontally and inverted so the player faces the attack's origin side.
- Assist stores one fixed direction and never follows `Source.position`.
- A zero horizontal direction is invalid and cannot start assist or pass directional Guard Coverage.

## Independent Guard Angles and Pre-Assist Coverage

Serialized values on `PlayerBlock`:

- `facingAssistHalfAngle = 60f`: total `120`-degree assist cone.
- `guardCoverageHalfAngle = 90f`: total `180`-degree Guard cone.

Assist starts only when the pre-turn angle is inside both limits. When the matching real hit arrives, `PlayerBlock` compares its direction against the saved horizontal facing from before automatic rotation. This prevents Facing Assist from enlarging Guard Coverage even though the player's Transform has already turned visually.

Default mapping at preview time:

- `0-60` degrees: eligible for pre-hit Facing Assist and inside Guard Coverage.
- above `60` through `90` degrees: inside Guard Coverage but no Assist.
- above `90` degrees: no Assist and the later unchanged-direction hit is outside Guard Coverage.

## Assist Lifetime and Facing Ownership

`PlayerBlock` owns assist eligibility and state:

- fixed assist direction;
- matching source;
- saved pre-assist horizontal facing;
- absolute expected-impact end time;
- read-only active/direction access for `PlayerMovement`.

Assist ends when expected impact is reached, a matching real hit is handled, a new Block begins, or Block enters Release. Hold does not clear it.

`PlayerMovement` remains the sole component that writes player Transform rotation. Its implemented branch order is:

1. active Guard Facing Assist;
2. otherwise Locked Facing;
3. otherwise Free Movement Facing.

The current version reuses ordinary `FaceDirection()` and `rotationSpeed`. It does not guarantee mathematical arrival exactly at `ExpectedImpactTime`. That becomes optional timing/feel work when the current enemy or later Boss fast/slow attacks provide runtime evidence that fixed speed is insufficient.

## Responsibility Map

- `EnemyAttack`: own attack phase/timing, emit/remove the preview, and construct the real hit. Never inspect player Guard state.
- `PlayerHitReceiver`: store/select incoming previews, receive real hits, route by coarse action state, and forward unhandled damage.
- `PlayerActionController`: remain the sole coarse-state authority.
- `PlayerBlock`: own phases, Guard Coverage, preview-assist eligibility/state, Perfect Guard Window, and ordinary-versus-perfect classification.
- `PlayerMovement`: remain the sole Transform-facing owner.
- `PlayerHealth`: remain the final health-value owner.
- `PlayerTargeting`: remain outside incoming Guard resolution and preview selection.
- `PlayerAnimator`: present outcomes only; never decide damage or Guard permission.

## Boss and Other Enemy Reuse

Different enemy prefabs may keep different serialized Startup durations. A later Boss fast or slow attack supplies its selected attack's own expected impact time through the same context; Guard code does not contain monster-specific timing.

Do not add Boss attack-selection data now. When the Boss owns multiple concrete attacks, give each selected attack one authoritative timing source and derive both Hit Window timing and `ExpectedImpactTime` from it. Do not maintain two unrelated clocks.

Each strike of a future overlapping multi-hit attack may require its own threat identity. That is a later concrete extension, not part of the current single-strike prototype.

## Implemented Perfect Guard Classification and Remaining Presentation

The minimal classification is implemented:

1. `BeginBlock()` opens the window after entering Startup;
2. the authored `Block_Start.anim` Event closes it at `0.16666667s`;
3. entering Hold or Release also closes it defensively;
4. after Guard Coverage succeeds, `TryHandleHit()` classifies an open-window hit as Perfect Guard and any other legal Startup/Hold hit as Ordinary Guard;
5. the current observable consequence is only the corresponding diagnostic log.

The next work is to define and verify one small result-to-presentation boundary, then add ordinary and Perfect Guard feedback layers separately. Do not prebuild a general `HitResult` hierarchy or combine enemy reaction, Parry, Counter, Guard Break, Dodge, and VFX spawning into one step.

## Verification State

Implemented and learner-reported verified for the current enemy:

- approximately `40-50` degree real Startup preview;
- Block_Start and turning begin together;
- turning continues into Hold before impact;
- impact does not restart Block_Start;
- matching hit is guarded without health loss;
- Console is clean;
- `Assembly-CSharp.csproj` builds with zero errors and zero warnings.

Follow-up pre-hit assist regressions passed:

- Block with no active real preview does not rotate;
- entering Release before impact stops assist; this used temporary Play Mode-only `startupDuration = 2` for visibility and did not persist attack tuning.

Perfect Guard classification regressions passed on 2026-08-31:

- a legal handled hit after the authored window closed logged `Ordinary Guard`;
- a legal handled hit while the Startup window was open logged `Perfect Guard`;
- both results prevented health damage and left the Console otherwise clean.

Still unclaimed or deferred:

- `70`-degree preview guards without assist;
- `100`-degree preview does not assist and the unchanged-direction hit damages;
- exact impact-time arrival is not claimed;
- Boss fast/slow reuse is not claimed until the Boss exists.

## Explicit Non-Goals

- No empty-Guard enemy search.
- No Guard use of `PlayerTargeting.CurrentTarget` for assist.
- No source-following rotation.
- No Root Motion facing.
- No second gameplay FSM or `GuardTurning` coarse state.
- No numeric action/facing Priority system.
- No general Facing, Damage, Ability, or result framework.
- No `IDamageable`, event bus, tag/team system, or speculative `AttackId`.
- No Perfect Guard consequences, Parry, Counter, Guard Break, Projectile, Dodge, Boss attack system, or physical hitbox confirmation in the current assist concept.
